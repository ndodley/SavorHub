using Microsoft.EntityFrameworkCore;
using SavorHub.Data.Data;
using SavorHub.Data.Repository;
using SavorHub.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SavorHub.Data.Tests
{
    public class ReviewRepositoryTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Update_ChangesContentRatingAndDate()
        {
            using var context = CreateContext();

            var review = new Review
            {
                UserId = "test-user",
                MenuItemId = 1,
                Content = "Original review",
                Rating = 3,
                Date = new DateTime(2024, 1, 1)
            };

            context.Reviews.Add(review);
            context.SaveChanges();

            var repository = new ReviewRepository(context);

            var updatedDate = new DateTime(2024, 2, 1);

            var updatedReview = new Review
            {
                Id = review.Id,
                UserId = review.UserId,
                MenuItemId = review.MenuItemId,
                Content = "Updated review",
                Rating = 5,
                Date = updatedDate
            };

            repository.Update(updatedReview);
            context.SaveChanges();

            var result = context.Reviews.Find(review.Id);

            Assert.NotNull(result);
            Assert.Equal("Updated review", result.Content);
            Assert.Equal(5, result.Rating);
            Assert.Equal(updatedDate, result.Date);
        }
    }
}
