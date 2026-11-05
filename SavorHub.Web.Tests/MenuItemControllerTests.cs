using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Utilities;
using SavorHub.Web.Controllers;
using System.Linq.Expressions;
using System.Text;

namespace SavorHub.Web.Tests
{
    public class MenuItemControllerTests : IDisposable
    {
        private const string CsvHeader = "Name,Description,Price,Category,FoodType,Image";

        // Each test gets its own temporary wwwroot so image files can be checked on disk.
        private readonly string _webRoot;
        private readonly List<Category> _categories = new()
        {
            new Category { Id = 1, Name = "Appetizer", DisplayOrder = "1" },
            new Category { Id = 2, Name = "Entree", DisplayOrder = "2" }
        };
        private readonly List<FoodType> _foodTypes = new()
        {
            new FoodType { Id = 1, Name = "Veg" },
            new FoodType { Id = 2, Name = "Non-Veg" }
        };

        private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
        private readonly Mock<IFoodTypeRepository> _foodTypeRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IWebHostEnvironment> _hostEnvironmentMock = new();

        public MenuItemControllerTests()
        {
            _webRoot = Path.Combine(Path.GetTempPath(), "SavorHubTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(_webRoot, "images", "menuItems"));

            _unitOfWorkMock.Setup(u => u.MenuItem).Returns(_menuItemRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Category).Returns(_categoryRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.FoodType).Returns(_foodTypeRepositoryMock.Object);
            _hostEnvironmentMock.Setup(h => h.WebRootPath).Returns(_webRoot);

            _categoryRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string?>()))
                .Returns((Expression<Func<Category, bool>> filter, string? _) => _categories.AsQueryable().FirstOrDefault(filter)!);
            _foodTypeRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<Expression<Func<FoodType, bool>>>(), It.IsAny<string?>()))
                .Returns((Expression<Func<FoodType, bool>> filter, string? _) => _foodTypes.AsQueryable().FirstOrDefault(filter)!);
        }

        public void Dispose()
        {
            if (Directory.Exists(_webRoot))
            {
                Directory.Delete(_webRoot, recursive: true);
            }
        }

        private MenuItemController CreateController()
        {
            return new MenuItemController(_unitOfWorkMock.Object, _hostEnvironmentMock.Object);
        }

        private MenuItem CreateMenuItem(int id, string name, string description, double price, string image)
        {
            return new MenuItem
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                Image = image,
                CategoryId = 1,
                FoodTypeId = 2,
                Category = _categories[0],
                FoodType = _foodTypes[1]
            };
        }

        private string CreateImageInWebRoot(string fileName)
        {
            System.IO.File.WriteAllText(Path.Combine(_webRoot, "images", "menuItems", fileName), "fake image");
            return $"/images/menuItems/{fileName}";
        }

        private void SetupExistingMenuItems(params MenuItem[] items)
        {
            _menuItemRepositoryMock.Setup(r => r.GetAll(null, null, null)).Returns(items.ToList());
            _menuItemRepositoryMock.Setup(r => r.GetAll(null, null, "Category,FoodType")).Returns(items.ToList());
        }

        private static IFormFile CreateCsvFile(string csv)
        {
            var bytes = Encoding.UTF8.GetBytes(csv);
            return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "menu.csv");
        }

        private static List<List<string>> ReadCsvRows(FileContentResult file)
        {
            var text = Encoding.UTF8.GetString(file.FileContents).TrimStart('﻿');
            return CsvUtility.Parse(text);
        }

        // ---------- ExportCSV ----------

        [Fact]
        public void ExportCSV_ReturnsCsvFile_WithHeaderAndOneRowPerMenuItem()
        {
            SetupExistingMenuItems(
                CreateMenuItem(1, "Fried Shrimp", "<p>Ebi Fried Shrimp</p>", 4.99, @"\images\menuItems\shrimp.png"),
                CreateMenuItem(2, "Buffalo Wings", "<p>Hot wings</p>", 3.99, @"\images\menuItems\wings.png"));

            var file = Assert.IsType<FileContentResult>(CreateController().ExportCSV());

            Assert.Equal("text/csv", file.ContentType);
            Assert.StartsWith("MenuItems_", file.FileDownloadName);
            Assert.EndsWith(".csv", file.FileDownloadName);

            var rows = ReadCsvRows(file);
            Assert.Equal(3, rows.Count);
            Assert.Equal(CsvHeader.Split(','), rows[0]);
            Assert.Equal(new[] { "Fried Shrimp", "Ebi Fried Shrimp", "4.99", "Appetizer", "Non-Veg", "/images/menuItems/shrimp.png" }, rows[1]);
            Assert.Equal("Buffalo Wings", rows[2][0]);
        }

        [Fact]
        public void ExportCSV_StartsWithUtf8Bom_SoExcelReadsSpecialCharacters()
        {
            SetupExistingMenuItems();

            var file = Assert.IsType<FileContentResult>(CreateController().ExportCSV());

            Assert.Equal(Encoding.UTF8.GetPreamble(), file.FileContents.Take(3).ToArray());
        }

        [Fact]
        public void ExportCSV_WritesDescriptionAsPlainText_WithoutHtmlTags()
        {
            SetupExistingMenuItems(
                CreateMenuItem(1, "Fried Shrimp", "<p>Crispy, golden \"jumbo\" shrimp</p><p>Served with sauce</p>", 4.99, @"\images\menuItems\shrimp.png"));

            var rows = ReadCsvRows(Assert.IsType<FileContentResult>(CreateController().ExportCSV()));

            Assert.Equal(6, rows[1].Count);
            Assert.Equal("Crispy, golden \"jumbo\" shrimp\nServed with sauce", rows[1][1]);
            Assert.Equal("Appetizer", rows[1][3]);
        }

        // ---------- UploadCSV ----------

        [Fact]
        public void UploadCSV_ReturnsBadRequest_WhenNoFileIsProvided()
        {
            Assert.IsType<BadRequestObjectResult>(CreateController().UploadCSV(null!));
        }

        [Fact]
        public void UploadCSV_CreatesMenuItem_WhenNameDoesNotExist()
        {
            SetupExistingMenuItems();
            var image = CreateImageInWebRoot("shrimp.png");
            var csv = $"{CsvHeader}\nFried Shrimp,Ebi Fried Shrimp,4.99,Appetizer,Non-Veg,{image}\n";

            var result = CreateController().UploadCSV(CreateCsvFile(csv));

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.StartsWith("Upload complete: 1 created, 0 updated, 0 skipped.", ok.Value as string);
            _menuItemRepositoryMock.Verify(r => r.Add(It.Is<MenuItem>(m =>
                m.Name == "Fried Shrimp" &&
                m.Description == "<p>Ebi Fried Shrimp</p>" &&
                m.Price == 4.99 &&
                m.CategoryId == 1 &&
                m.FoodTypeId == 2 &&
                m.Image == image)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void UploadCSV_UpdatesExistingMenuItem_InsteadOfCreatingDuplicate()
        {
            var image = CreateImageInWebRoot("shrimp.png");
            var existing = CreateMenuItem(1, "Fried Shrimp", "<p>Old</p>", 4.99, image);
            SetupExistingMenuItems(existing);
            var csv = $"{CsvHeader}\nfried shrimp,New description,5.49,Entree,Veg,{image}\n";

            var ok = Assert.IsType<OkObjectResult>(CreateController().UploadCSV(CreateCsvFile(csv)));

            Assert.StartsWith("Upload complete: 0 created, 1 updated, 0 skipped.", ok.Value as string);
            Assert.Equal("<p>New description</p>", existing.Description);
            Assert.Equal(5.49, existing.Price);
            Assert.Equal(2, existing.CategoryId);
            Assert.Equal(1, existing.FoodTypeId);
            _menuItemRepositoryMock.Verify(r => r.Update(existing), Times.Once);
            _menuItemRepositoryMock.Verify(r => r.Add(It.IsAny<MenuItem>()), Times.Never);
        }

        [Fact]
        public void UploadCSV_ReusesImageAlreadyInWebRoot_WithoutCopying()
        {
            SetupExistingMenuItems();
            var image = CreateImageInWebRoot("shrimp.png");
            var csv = $"{CsvHeader}\nFried Shrimp,Shrimp,4.99,Appetizer,Non-Veg,{image}\n";

            CreateController().UploadCSV(CreateCsvFile(csv));

            Assert.Single(Directory.GetFiles(Path.Combine(_webRoot, "images", "menuItems")));
        }

        [Fact]
        public void UploadCSV_CopiesImageIntoWebRoot_WhenGivenFullPathOutsideWebRoot()
        {
            SetupExistingMenuItems();
            var outsideImage = Path.Combine(_webRoot, "outside.jpg");
            System.IO.File.WriteAllText(outsideImage, "fake image");
            var csv = CsvUtility.BuildRow("Name", "Description", "Price", "Category", "FoodType", "Image") + "\n" +
                      CsvUtility.BuildRow("Fried Shrimp", "Shrimp", "4.99", "Appetizer", "Non-Veg", outsideImage) + "\n";

            CreateController().UploadCSV(CreateCsvFile(csv));

            var copied = Assert.Single(Directory.GetFiles(Path.Combine(_webRoot, "images", "menuItems")));
            _menuItemRepositoryMock.Verify(r => r.Add(It.Is<MenuItem>(m =>
                m.Image == "/images/menuItems/" + Path.GetFileName(copied))), Times.Once);
        }

        [Fact]
        public void UploadCSV_SkipsRowAndReportsIt_WhenCategoryIsUnknown()
        {
            SetupExistingMenuItems();
            var image = CreateImageInWebRoot("shrimp.png");
            var csv = $"{CsvHeader}\nFried Shrimp,Shrimp,4.99,Breakfast,Non-Veg,{image}\n";

            var ok = Assert.IsType<OkObjectResult>(CreateController().UploadCSV(CreateCsvFile(csv)));

            var message = Assert.IsType<string>(ok.Value);
            Assert.StartsWith("Upload complete: 0 created, 0 updated, 1 skipped.", message);
            Assert.Contains("unknown category 'Breakfast'", message);
            _menuItemRepositoryMock.Verify(r => r.Add(It.IsAny<MenuItem>()), Times.Never);
        }

        [Fact]
        public void UploadCSV_SkipsRowAndReportsIt_WhenImageIsMissing()
        {
            SetupExistingMenuItems();
            var csv = $"{CsvHeader}\nFried Shrimp,Shrimp,4.99,Appetizer,Non-Veg,/images/menuItems/missing.png\n";

            var ok = Assert.IsType<OkObjectResult>(CreateController().UploadCSV(CreateCsvFile(csv)));

            var message = Assert.IsType<string>(ok.Value);
            Assert.Contains("1 skipped", message);
            Assert.Contains("image not found", message);
        }

        [Fact]
        public void ExportCSV_ThenUploadCSV_RoundTripsWithoutChangesOrDuplicates()
        {
            var image = CreateImageInWebRoot("shrimp.png");
            var existing = CreateMenuItem(1, "Fried Shrimp", "<p>Ebi Fried Shrimp</p>", 4.99, @"\images\menuItems\shrimp.png");
            SetupExistingMenuItems(existing);
            var controller = CreateController();

            var exported = Assert.IsType<FileContentResult>(controller.ExportCSV());
            var exportedCsv = Encoding.UTF8.GetString(exported.FileContents);
            var ok = Assert.IsType<OkObjectResult>(controller.UploadCSV(CreateCsvFile(exportedCsv)));

            Assert.StartsWith("Upload complete: 0 created, 1 updated, 0 skipped.", ok.Value as string);
            Assert.Equal("<p>Ebi Fried Shrimp</p>", existing.Description);
            Assert.Equal(4.99, existing.Price);
            Assert.Equal(image, existing.Image);
            _menuItemRepositoryMock.Verify(r => r.Add(It.IsAny<MenuItem>()), Times.Never);
        }

        [Fact]
        public void UploadCSV_UsesPlaceholderImage_WhenImageColumnIsBlankForNewItem()
        {
            SetupExistingMenuItems();
            var csv = $"{CsvHeader}\nGarlic Bread,Toasted bread,3.49,Appetizer,Veg,\n";

            var ok = Assert.IsType<OkObjectResult>(CreateController().UploadCSV(CreateCsvFile(csv)));

            Assert.StartsWith("Upload complete: 1 created, 0 updated, 0 skipped.", ok.Value as string);
            _menuItemRepositoryMock.Verify(r => r.Add(It.Is<MenuItem>(m =>
                m.Name == "Garlic Bread" && m.Image == SD.DefaultMenuItemImage)), Times.Once);
        }

        [Fact]
        public void UploadCSV_KeepsExistingImage_WhenImageColumnIsBlankOnUpdate()
        {
            var image = CreateImageInWebRoot("soda.png");
            var existing = CreateMenuItem(1, "Soda", "<p>Soft Soda Drinks</p>", 1.99, image);
            SetupExistingMenuItems(existing);
            var csv = $"{CsvHeader}\nSoda,Soft Soda Drinks,2.29,Appetizer,Veg,\n";

            var ok = Assert.IsType<OkObjectResult>(CreateController().UploadCSV(CreateCsvFile(csv)));

            Assert.StartsWith("Upload complete: 0 created, 1 updated, 0 skipped.", ok.Value as string);
            Assert.Equal(image, existing.Image);
            Assert.Equal(2.29, existing.Price);
        }

        // ---------- Other endpoints ----------

        [Fact]
        public void DownloadSampleCSV_ReturnsTemplate_WithExpectedHeaderAndSampleRow()
        {
            var file = Assert.IsType<FileContentResult>(CreateController().DownloadSampleCSV());
            var rows = ReadCsvRows(file);

            Assert.Equal("SampleMenuItems.csv", file.FileDownloadName);
            Assert.Equal(CsvHeader.Split(','), rows[0]);
            Assert.Equal("Sample Item", rows[1][0]);
            Assert.Equal("Appetizer", rows[1][3]);
        }

        [Fact]
        public void Delete_KeepsSharedPlaceholderImageFile()
        {
            var placeholderFile = Path.Combine(_webRoot, "images", "no-image.svg");
            System.IO.File.WriteAllText(placeholderFile, "<svg/>");
            var menuItem = CreateMenuItem(7, "Garlic Bread", "Toasted", 3.49, SD.DefaultMenuItemImage);
            _menuItemRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<Expression<Func<MenuItem, bool>>>(), It.IsAny<string?>()))
                .Returns(menuItem);

            CreateController().Delete(7);

            Assert.True(System.IO.File.Exists(placeholderFile));
            _menuItemRepositoryMock.Verify(r => r.Remove(menuItem), Times.Once);
        }

        [Fact]
        public void Delete_DeletesItemsOwnImageFile_WhenPathUsesForwardSlashes()
        {
            var image = CreateImageInWebRoot("shrimp.png");
            var menuItem = CreateMenuItem(8, "Fried Shrimp", "Crispy", 4.99, image);
            _menuItemRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<Expression<Func<MenuItem, bool>>>(), It.IsAny<string?>()))
                .Returns(menuItem);

            CreateController().Delete(8);

            Assert.False(System.IO.File.Exists(Path.Combine(_webRoot, "images", "menuItems", "shrimp.png")));
        }

        [Fact]
        public void Delete_KeepsImageFile_WhenAnotherMenuItemStillUsesIt()
        {
            var image = CreateImageInWebRoot("soda.png");
            var lemonade = CreateMenuItem(9, "Iced Lemonade", "Lemonade", 2.49, image);
            var soda = CreateMenuItem(10, "Soda", "Soda", 1.99, @"\images\menuItems\soda.png");
            _menuItemRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<Expression<Func<MenuItem, bool>>>(), It.IsAny<string?>()))
                .Returns(lemonade);
            SetupExistingMenuItems(lemonade, soda);

            CreateController().Delete(9);

            Assert.True(System.IO.File.Exists(Path.Combine(_webRoot, "images", "menuItems", "soda.png")));
            _menuItemRepositoryMock.Verify(r => r.Remove(lemonade), Times.Once);
        }

        [Fact]
        public void Delete_RemovesMenuItemAndSaves_WhenItemExists()
        {
            var menuItem = CreateMenuItem(5, "Fried Shrimp", "Crispy", 4.99, @"\images\menuItems\missing.png");
            _menuItemRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<Expression<Func<MenuItem, bool>>>(), It.IsAny<string?>()))
                .Returns(menuItem);

            var result = CreateController().Delete(5);

            Assert.IsType<JsonResult>(result);
            _menuItemRepositoryMock.Verify(r => r.Remove(menuItem), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }
    }
}
