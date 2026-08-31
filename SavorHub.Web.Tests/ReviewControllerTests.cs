using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace SavorHub.Web.Tests
{
    public class ReviewControllerTests
    {
        private static ReviewController CreateControllerWithUser(IUnitOfWork unitOfWork, string userId)
        {
            var controller = new ReviewController(unitOfWork);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
                "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            return controller;
        }

        [Fact]
        public void Get_ReturnsNotFound_WhenReviewDoesNotExist()
        {
            var userId = "user-1";

            var reviewRepositoryMock = new Mock<IReviewRepository>();
            reviewRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), null))
                .Returns((Review)null);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Review).Returns(reviewRepositoryMock.Object);

            var controller = CreateControllerWithUser(unitOfWorkMock.Object, userId);

            var result = controller.Get(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_ReturnsJson_WhenReviewExistsForUser()
        {
            var userId = "user-1";
            var review = new Review
            {
                Id = 1,
                UserId = userId,
                MenuItemId = 1,
                Content = "Great",
                Rating = 5
            };

            var reviewRepositoryMock = new Mock<IReviewRepository>();
            reviewRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), null))
                .Returns(review);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Review).Returns(reviewRepositoryMock.Object);

            var controller = CreateControllerWithUser(unitOfWorkMock.Object, userId);

            var result = controller.Get(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(review, jsonResult.Value);
        }

        [Fact]
        public void Put_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            var reviewRepositoryMock = new Mock<IReviewRepository>();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Review).Returns(reviewRepositoryMock.Object);

            var controller = CreateControllerWithUser(unitOfWorkMock.Object, "user-1");

            var review = new Review
            {
                Id = 2,
                UserId = "user-1",
                MenuItemId = 1,
                Content = "Test",
                Rating = 4
            };

            var result = controller.Put(1, review);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Put_ReturnsNotFound_WhenReviewIsMissing()
        {
            var reviewRepositoryMock = new Mock<IReviewRepository>();
            reviewRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), null))
                .Returns((Review)null);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Review).Returns(reviewRepositoryMock.Object);

            var controller = CreateControllerWithUser(unitOfWorkMock.Object, "user-1");

            var review = new Review
            {
                Id = 1,
                UserId = "user-1",
                MenuItemId = 1,
                Content = "Updated",
                Rating = 5
            };

            var result = controller.Put(1, review);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ReturnsErrorJson_WhenReviewDoesNotExist()
        {
            var reviewRepositoryMock = new Mock<IReviewRepository>();
            reviewRepositoryMock
                .Setup(r => r.GetFirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), null))
                .Returns((Review)null);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Review).Returns(reviewRepositoryMock.Object);

            var controller = CreateControllerWithUser(unitOfWorkMock.Object, "user-1");

            var result = controller.Delete(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var successProperty = jsonResult.Value.GetType().GetProperty("success");
            var messageProperty = jsonResult.Value.GetType().GetProperty("message");

            Assert.NotNull(successProperty);
            Assert.NotNull(messageProperty);
            Assert.False((bool)successProperty.GetValue(jsonResult.Value));
            Assert.Equal("Error while deleting", messageProperty.GetValue(jsonResult.Value)?.ToString());
        }
    }
}
