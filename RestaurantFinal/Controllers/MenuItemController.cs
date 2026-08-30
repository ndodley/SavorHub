using Microsoft.AspNetCore.Mvc;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using System.Globalization;
using System.Text;

namespace SavorHub.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : Controller
    {
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
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, objFromDb.Image.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
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
            csv.AppendLine("Name,Description,Price,Category,FoodType,Image");

            foreach (var item in sampleData)
            {
                var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == item.CategoryId)?.Name ?? "Unknown";
                var foodType = _unitOfWork.FoodType.GetFirstOrDefault(f => f.Id == item.FoodTypeId)?.Name ?? "Unknown";
                csv.AppendLine($"{item.Name},{item.Description},{item.Price},{category},{foodType},{item.Image}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "SampleMenuItems.csv");
        }

        [HttpPost("UploadCSV")]
        public IActionResult UploadCSV(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                var csvData = stream.ReadToEnd();
                var lines = csvData.Split('\n');

                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',');

                    var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Name == values[3]);
                    var foodType = _unitOfWork.FoodType.GetFirstOrDefault(f => f.Name == values[4]);

                    if (category == null || foodType == null)
                    {
                        continue; // Skip if category or food type is not found
                    }

                    // Handle image upload
                    var imagePath = values[5].Trim();
                    var imageFileName = Path.GetFileName(imagePath);
                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFileName);
                    var newImagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "menuItems", newFileName);

                    if (!System.IO.File.Exists(newImagePath))
                    {
                        System.IO.File.Copy(imagePath, newImagePath);
                    }

                    var menuItem = new MenuItem
                    {
                        Name = values[0],
                        Description = values[1],
                        Price = double.Parse(values[2], CultureInfo.InvariantCulture),
                        CategoryId = category.Id,
                        FoodTypeId = foodType.Id,
                        Image = $"/images/menuItems/{newFileName}"
                        //Image = values[5]
                    };

                    _unitOfWork.MenuItem.Add(menuItem);
                }

                _unitOfWork.Save();
            }

            return Ok("File uploaded successfully");
        }



    }
}
