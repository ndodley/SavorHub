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
    public class OrderHeaderRepositoryTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void UpdateStatus_ChangesStatus_WhenOrderExists()
        {
            using var context = CreateContext();

            var orderHeader = new OrderHeader
            {
                UserId = "test-user",
                OrderDate = DateTime.Now,
                OrderTotal = 25.50,
                PickUpTime = DateTime.Now.AddHours(1),
                PickUpDate = DateTime.Today,
                Status = "Pending",
                PickupName = "Test User",
                PhoneNumber = "1234567890"
            };

            context.OrderHeader.Add(orderHeader);
            context.SaveChanges();

            var repository = new OrderHeaderRepository(context);

            repository.UpdateStatus(orderHeader.Id, "Completed");
            context.SaveChanges();

            var result = context.OrderHeader.Find(orderHeader.Id);

            Assert.NotNull(result);
            Assert.Equal("Completed", result.Status);
        }

        [Fact]
        public void UpdateStatus_DoesNothing_WhenOrderDoesNotExist()
        {
            using var context = CreateContext();

            var repository = new OrderHeaderRepository(context);

            repository.UpdateStatus(999, "Completed");
            context.SaveChanges();

            Assert.Empty(context.OrderHeader);
        }
    }
}
