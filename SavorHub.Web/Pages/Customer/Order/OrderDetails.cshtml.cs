using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models.ViewModel;
using SavorHub.Utilities;
using System.Security.Claims;

namespace SavorHub.Web.Pages.Customer.Order
{
    [Authorize]
    public class OrderDetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderDetailVM OrderDetailVM { get; set; }

        public IActionResult OnGet(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isStaff = User.IsInRole(SD.ManagerRole) || User.IsInRole(SD.FrontDeskRole) || User.IsInRole(SD.KitchenRole);
            if (orderHeader.UserId != currentUserId && !isStaff)
            {
                return NotFound();
            }

            OrderDetailVM = new OrderDetailVM()
            {
                OrderHeader = orderHeader,
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties: "MenuItem").ToList()
            };
            return Page();
        }
    }
}
