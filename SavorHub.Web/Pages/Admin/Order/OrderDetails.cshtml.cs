using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Models.ViewModel;
using SavorHub.Utilities;

namespace SavorHub.Web.Pages.Admin.Order
{
    [Authorize(Roles = $"{SD.ManagerRole},{SD.FrontDeskRole}")]
    public class OrderDetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailVM OrderDetailVM { get; set; }
        public OrderDetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void OnGet(int id)
        {
            OrderDetailVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties: "MenuItem").ToList()
            };
        }

        public IActionResult OnPostOrderCompleted(int orderId) // Completed
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderId, SD.StatusCompleted);
            _unitOfWork.Save();
            return RedirectToPage("OrderList");
        }

        /*public IActionResult OnPostOrderRefund(int orderId) // Refund
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);

            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStatus(orderId, SD.StatusRefunded);
            _unitOfWork.Save();
            return RedirectToPage("OrderList");
        }*/

        public IActionResult OnPostOrderCancel(int orderId) // Cancel
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderId, SD.StatusCancelled);
            _unitOfWork.Save();
            return RedirectToPage("OrderList");
        }
    }
}
