using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Data;
using Restaurant.Data.Repository;
using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Restaurant.Data.Tests
{
    public class CategoryRepositoryTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Update_ChangesNameAndDisplayOrder()
        {
            using var context = CreateContext();

            var category = new Category
            {
                Name = "Original",
                DisplayOrder = "1"
            };

            context.Category.Add(category);
            context.SaveChanges();

            var repository = new CategoryRepository(context);

            var updatedCategory = new Category
            {
                Id = category.Id,
                Name = "Updated",
                DisplayOrder = "2"
            };

            repository.Update(updatedCategory);
            context.SaveChanges();

            var result = context.Category.Find(category.Id);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);
            Assert.Equal("2", result.DisplayOrder);
        }
    }
}
