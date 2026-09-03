using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Utilities;
using System.Security.Claims;

namespace SavorHub.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.ManagerRole},{SD.FrontDeskRole}")]
        public IActionResult Get(string? status = null)
        {

            var OrderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            if (status == "cancelled")
            {
                OrderHeaderList = OrderHeaderList.Where(u => u.Status == SD.StatusCancelled || u.Status == SD.StatusRejected);
            }
            else
            {
                if (status == "completed")
                {
                    OrderHeaderList = OrderHeaderList.Where(u => u.Status == SD.StatusCompleted);
                }
                else
                {
                    if (status == "ready")
                    {
                        OrderHeaderList = OrderHeaderList.Where(u => u.Status == SD.StatusReady);
                    }
                    else if (status == "all") {
                        // No filtering, show all orders
                    }
                    else
                    {
                        OrderHeaderList = OrderHeaderList.Where(u => u.Status == SD.StatusSubmitted || u.Status == SD.StatusInProcess);
                    }
                }
            }

            return Json(new { data = OrderHeaderList });
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public IActionResult GetUserOrders(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isStaff = User.IsInRole(SD.ManagerRole) || User.IsInRole(SD.FrontDeskRole) || User.IsInRole(SD.KitchenRole);
            if (currentUserId != userId && !isStaff)
            {
                return Forbid();
            }

            var OrderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.UserId == userId, includeProperties: "ApplicationUser");
            return Json(new { data = OrderHeaderList });
        }
    }
}
