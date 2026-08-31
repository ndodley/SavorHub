using Microsoft.AspNetCore.Mvc;
using Moq;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Utilities;
using SavorHub.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace SavorHub.Web.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void Get_ReturnsCompletedOrders_WhenStatusIsCompleted()
        {
            var orders = new List<OrderHeader>
            {
                new OrderHeader { Id = 1, Status = SD.StatusCompleted },
                new OrderHeader { Id = 2, Status = SD.StatusCancelled }
            };

            var orderHeaderRepositoryMock = new Mock<IOrderHeaderRepository>();
            orderHeaderRepositoryMock
                .Setup(r => r.GetAll(null, null, "ApplicationUser"))
                .Returns(orders);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.OrderHeader).Returns(orderHeaderRepositoryMock.Object);

            var controller = new OrderController(unitOfWorkMock.Object);

            var result = controller.Get("completed") as JsonResult;

            Assert.NotNull(result);

            var dataProperty = result.Value.GetType().GetProperty("data");
            var returnedOrders = dataProperty?.GetValue(result.Value) as IEnumerable<OrderHeader>;

            Assert.NotNull(returnedOrders);
            Assert.Single(returnedOrders);
            Assert.Equal(SD.StatusCompleted, returnedOrders.First().Status);
        }

        [Fact]
        public void Get_ReturnsCancelledAndRejectedOrders_WhenStatusIsCancelled()
        {
            var orders = new List<OrderHeader>
            {
                new OrderHeader { Id = 1, Status = SD.StatusCancelled },
                new OrderHeader { Id = 2, Status = SD.StatusRejected },
                new OrderHeader { Id = 3, Status = SD.StatusCompleted }
            };

            var orderHeaderRepositoryMock = new Mock<IOrderHeaderRepository>();
            orderHeaderRepositoryMock
                .Setup(r => r.GetAll(null, null, "ApplicationUser"))
                .Returns(orders);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.OrderHeader).Returns(orderHeaderRepositoryMock.Object);

            var controller = new OrderController(unitOfWorkMock.Object);

            var result = controller.Get("cancelled") as JsonResult;

            Assert.NotNull(result);

            var dataProperty = result.Value.GetType().GetProperty("data");
            var returnedOrders = dataProperty?.GetValue(result.Value) as IEnumerable<OrderHeader>;

            Assert.NotNull(returnedOrders);
            Assert.Equal(2, returnedOrders.Count());
            Assert.All(returnedOrders, o =>
                Assert.True(o.Status == SD.StatusCancelled || o.Status == SD.StatusRejected));
        }

        [Fact]
        public void Get_ReturnsReadyOrders_WhenStatusIsReady()
        {
            var orders = new List<OrderHeader>
            {
                new OrderHeader { Id = 1, Status = SD.StatusReady },
                new OrderHeader { Id = 2, Status = SD.StatusCompleted }
            };

            var orderHeaderRepositoryMock = new Mock<IOrderHeaderRepository>();
            orderHeaderRepositoryMock
                .Setup(r => r.GetAll(null, null, "ApplicationUser"))
                .Returns(orders);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.OrderHeader).Returns(orderHeaderRepositoryMock.Object);

            var controller = new OrderController(unitOfWorkMock.Object);

            var result = controller.Get("ready") as JsonResult;

            Assert.NotNull(result);

            var dataProperty = result.Value.GetType().GetProperty("data");
            var returnedOrders = dataProperty?.GetValue(result.Value) as IEnumerable<OrderHeader>;

            Assert.NotNull(returnedOrders);
            Assert.Single(returnedOrders);
            Assert.Equal(SD.StatusReady, returnedOrders.First().Status);
        }

        [Fact]
        public void Get_ReturnsSubmittedAndInProcessOrders_WhenStatusIsNull()
        {
            var orders = new List<OrderHeader>
            {
                new OrderHeader { Id = 1, Status = SD.StatusSubmitted },
                new OrderHeader { Id = 2, Status = SD.StatusInProcess },
                new OrderHeader { Id = 3, Status = SD.StatusCompleted }
            };

            var orderHeaderRepositoryMock = new Mock<IOrderHeaderRepository>();
            orderHeaderRepositoryMock
                .Setup(r => r.GetAll(null, null, "ApplicationUser"))
                .Returns(orders);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.OrderHeader).Returns(orderHeaderRepositoryMock.Object);

            var controller = new OrderController(unitOfWorkMock.Object);

            var result = controller.Get() as JsonResult;

            Assert.NotNull(result);

            var dataProperty = result.Value.GetType().GetProperty("data");
            var returnedOrders = dataProperty?.GetValue(result.Value) as IEnumerable<OrderHeader>;

            Assert.NotNull(returnedOrders);
            Assert.Equal(2, returnedOrders.Count());
            Assert.All(returnedOrders, o =>
                Assert.True(o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess));
        }

        [Fact]
        public void GetUserOrders_ReturnsOrdersForSpecifiedUser()
        {
            var userId = "user-1";
            var orders = new List<OrderHeader>
            {
                new OrderHeader { Id = 1, UserId = userId },
                new OrderHeader { Id = 2, UserId = userId }
            };

            var orderHeaderRepositoryMock = new Mock<IOrderHeaderRepository>();
            orderHeaderRepositoryMock
                .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderHeader, bool>>>(), null, "ApplicationUser"))
                .Returns(orders);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.OrderHeader).Returns(orderHeaderRepositoryMock.Object);

            var controller = new OrderController(unitOfWorkMock.Object);

            var result = controller.GetUserOrders(userId) as JsonResult;

            Assert.NotNull(result);

            var dataProperty = result.Value.GetType().GetProperty("data");
            var returnedOrders = dataProperty?.GetValue(result.Value) as IEnumerable<OrderHeader>;

            Assert.NotNull(returnedOrders);
            Assert.Equal(2, returnedOrders.Count());
            Assert.All(returnedOrders, o => Assert.Equal(userId, o.UserId));
        }
    }
}
