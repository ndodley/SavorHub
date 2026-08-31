using System;
using Microsoft.EntityFrameworkCore;
using SavorHub.Data.Data;
using SavorHub.Data.Repository;
using SavorHub.Models;
using Xunit;

namespace SavorHub.Data.Tests
{
    public class ShoppingCartRepositoryTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void IncrementCount_IncreasesCountAndReturnsUpdatedValue()
        {
            using var context = CreateContext();

            var cart = new ShoppingCart
            {
                MenuItemId = 1,
                ApplicationUserId = "test-user",
                Count = 2
            };

            context.ShoppingCart.Add(cart);
            context.SaveChanges();

            var repository = new ShoppingCartRepository(context);

            var result = repository.IncrementCount(cart, 3);

            Assert.Equal(5, result);
            Assert.Equal(5, cart.Count);
        }

        [Fact]
        public void DecrementCount_DecreasesCountAndReturnsUpdatedValue()
        {
            using var context = CreateContext();

            var cart = new ShoppingCart
            {
                MenuItemId = 1,
                ApplicationUserId = "test-user",
                Count = 5
            };

            context.ShoppingCart.Add(cart);
            context.SaveChanges();

            var repository = new ShoppingCartRepository(context);

            var result = repository.DecrementCount(cart, 2);

            Assert.Equal(3, result);
            Assert.Equal(3, cart.Count);
        }
    }
}
