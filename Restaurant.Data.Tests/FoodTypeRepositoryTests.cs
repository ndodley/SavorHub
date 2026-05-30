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
    public class FoodTypeRepositoryTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Update_ChangesName()
        {
            using var context = CreateContext();

            var foodType = new FoodType
            {
                Name = "Original"
            };

            context.FoodType.Add(foodType);
            context.SaveChanges();

            var repository = new FoodTypeRepository(context);

            var updatedFoodType = new FoodType
            {
                Id = foodType.Id,
                Name = "Updated"
            };

            repository.Update(updatedFoodType);
            context.SaveChanges();

            var result = context.FoodType.Find(foodType.Id);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);
        }
    }
}
