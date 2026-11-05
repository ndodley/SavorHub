using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Utilities;
using System.Globalization;
using System.Text;

namespace SavorHub.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.ManagerRole)]
    public class MenuItemController : Controller
    {
        private const string CsvHeader = "Name,Description,Price,Category,FoodType,Image";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MenuItemController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var menuItemList = _unitOfWork.MenuItem.GetAll(includeProperties: "Category,FoodType");
            return Json(new { data = menuItemList });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.MenuItem.GetFirstOrDefault(u => u.Id == id);
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, objFromDb.Image.TrimStart('\\', '/'));
            // Keep the file if it's the shared placeholder or another menu item still uses it
            // (a CSV upload can point several items at the same image).
            var imageIsShared = SD.IsDefaultMenuItemImage(objFromDb.Image) ||
                _unitOfWork.MenuItem.GetAll().Any(m => m.Id != objFromDb.Id &&
                    string.Equals(SD.NormalizeImagePath(m.Image), SD.NormalizeImagePath(objFromDb.Image), StringComparison.OrdinalIgnoreCase));
            if (!imageIsShared && System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.MenuItem.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful." });
        }




        //CSV/////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("DownloadSampleCSV")]
        public IActionResult DownloadSampleCSV()
        {
            var sampleData = new List<MenuItem>
            {
                new MenuItem { Name = "Sample Item", Description = "Sample Description", Price = 10.99, CategoryId = 1, FoodTypeId = 1, Image = "C:/Users/njd/Downloads/CSV_Uploads/sample.jpg" }
            };

            var csv = new StringBuilder();
            csv.AppendLine(CsvHeader);

            foreach (var item in sampleData)
            {
                var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == item.CategoryId)?.Name ?? "Unknown";
                var foodType = _unitOfWork.FoodType.GetFirstOrDefault(f => f.Id == item.FoodTypeId)?.Name ?? "Unknown";
                csv.AppendLine(CsvUtility.BuildRow(
                    item.Name,
                    item.Description,
                    item.Price.ToString(CultureInfo.InvariantCulture),
                    category,
                    foodType,
                    item.Image));
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "SampleMenuItems.csv");
        }

        // Exports every menu item using the same columns as the sample template.
        // The file can be uploaded again with UploadCSV to restore or update the
        // menu (e.g. after an inventory reset).
        [HttpGet("ExportCSV")]
        public IActionResult ExportCSV()
        {
            var menuItems = _unitOfWork.MenuItem.GetAll(includeProperties: "Category,FoodType");

            var csv = new StringBuilder();
            csv.AppendLine(CsvHeader);

            foreach (var item in menuItems)
            {
                csv.AppendLine(CsvUtility.BuildRow(
                    item.Name,
                    HtmlTextUtility.HtmlToPlainText(item.Description),
                    item.Price.ToString(CultureInfo.InvariantCulture),
                    item.Category?.Name,
                    item.FoodType?.Name,
                    ToWebImagePath(item.Image)));
            }

            // UTF-8 BOM so Excel shows special characters (e.g. accents) correctly.
            var bytes = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
                .ToArray();

            var fileName = $"MenuItems_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }

        // Creates new menu items and updates existing ones (matched by name, case-insensitive),
        // so re-uploading an export never creates duplicates. Rows with problems are skipped
        // and reported back instead of failing the whole upload.
        [HttpPost("UploadCSV")]
        public IActionResult UploadCSV(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            string csvData;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                csvData = reader.ReadToEnd();
            }

            var rows = CsvUtility.Parse(csvData);
            var existingItems = _unitOfWork.MenuItem.GetAll().ToList();
            int created = 0;
            int updated = 0;
            var skipped = new List<string>();

            for (int i = 1; i < rows.Count; i++)
            {
                var values = rows[i];
                var rowLabel = $"Row {i + 1}";

                if (values.Count < 6)
                {
                    skipped.Add($"{rowLabel}: expected 6 columns but found {values.Count}");
                    continue;
                }

                var name = values[0].Trim();
                if (name.Length == 0)
                {
                    skipped.Add($"{rowLabel}: name is required");
                    continue;
                }
                rowLabel = $"{rowLabel} ({name})";

                if (!double.TryParse(values[2].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price <= 0)
                {
                    skipped.Add($"{rowLabel}: invalid price '{values[2]}'");
                    continue;
                }

                var categoryName = values[3].Trim();
                var foodTypeName = values[4].Trim();
                var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Name == categoryName);
                var foodType = _unitOfWork.FoodType.GetFirstOrDefault(f => f.Name == foodTypeName);

                if (category == null)
                {
                    skipped.Add($"{rowLabel}: unknown category '{categoryName}'");
                    continue;
                }
                if (foodType == null)
                {
                    skipped.Add($"{rowLabel}: unknown food type '{foodTypeName}'");
                    continue;
                }

                var existing = existingItems.FirstOrDefault(m =>
                    string.Equals(m.Name?.Trim(), name, StringComparison.OrdinalIgnoreCase));

                // The image column is optional: a blank value keeps an existing item's
                // image, or gives a new item the shared placeholder image.
                string? image;
                var imageValue = values[5].Trim();
                if (imageValue.Length == 0)
                {
                    image = existing?.Image ?? SD.DefaultMenuItemImage;
                }
                else
                {
                    image = ResolveUploadedImage(imageValue, out var imageError);
                    if (image == null)
                    {
                        skipped.Add($"{rowLabel}: {imageError}");
                        continue;
                    }
                }

                var description = HtmlTextUtility.PlainTextToHtml(values[1]);

                if (existing != null)
                {
                    existing.Description = description;
                    existing.Price = price;
                    existing.CategoryId = category.Id;
                    existing.FoodTypeId = foodType.Id;
                    existing.Image = image;
                    _unitOfWork.MenuItem.Update(existing);
                    updated++;
                }
                else
                {
                    var menuItem = new MenuItem
                    {
                        Name = name,
                        Description = description,
                        Price = price,
                        CategoryId = category.Id,
                        FoodTypeId = foodType.Id,
                        Image = image
                    };
                    _unitOfWork.MenuItem.Add(menuItem);
                    existingItems.Add(menuItem);
                    created++;
                }
            }

            _unitOfWork.Save();

            var message = $"Upload complete: {created} created, {updated} updated, {skipped.Count} skipped.";
            if (skipped.Count > 0)
            {
                message += "\n\n" + string.Join("\n", skipped);
            }
            return Ok(message);
        }

        // Stored paths look like "\images\menuItems\x.png" or "/images/menuItems/x.png".
        // Exports always use the portable web form: "/images/menuItems/x.png".
        private static string ToWebImagePath(string? storedImagePath)
        {
            if (string.IsNullOrWhiteSpace(storedImagePath))
            {
                return string.Empty;
            }

            return SD.NormalizeImagePath(storedImagePath);
        }

        // Accepts either a web path to an image already in wwwroot (what ExportCSV writes)
        // or a full path to an image file elsewhere on disk (what the sample template uses).
        // Returns the web path to store, or null with an error message.
        private string? ResolveUploadedImage(string imagePath, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                error = "image is required";
                return null;
            }

            var normalized = imagePath.Replace('\\', '/');
            var menuItemsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "menuItems");

            // Web path to an image that's already in wwwroot - reuse it, don't copy.
            if (normalized.StartsWith("/images/", StringComparison.OrdinalIgnoreCase) ||
                normalized.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
            {
                var relative = normalized.TrimStart('/');
                var physical = Path.Combine(_hostEnvironment.WebRootPath, relative.Replace('/', Path.DirectorySeparatorChar));
                if (!System.IO.File.Exists(physical))
                {
                    error = $"image not found in wwwroot: '{imagePath}'";
                    return null;
                }
                return "/" + relative;
            }

            // Full path to an image somewhere on disk - copy it into wwwroot.
            if (!System.IO.File.Exists(imagePath))
            {
                error = $"image file not found: '{imagePath}'";
                return null;
            }

            Directory.CreateDirectory(menuItemsFolder);
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagePath);
            System.IO.File.Copy(imagePath, Path.Combine(menuItemsFolder, newFileName));
            return $"/images/menuItems/{newFileName}";
        }
    }
}
