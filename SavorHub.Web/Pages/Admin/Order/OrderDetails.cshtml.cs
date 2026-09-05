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
        // Returns IActionResult (not void) so a bad/stale id - e.g. someone revisiting
        // an old link, or manually editing the query string - can return NotFound()
        // instead of leaving OrderDetailVM.OrderHeader null. Previously this was void
        // and unconditionally built OrderDetailVM from GetFirstOrDefault's result, so a
        // nonexistent id crashed with a NullReferenceException the moment
        // OrderDetails.cshtml dereferenced Model.OrderDetailVM.OrderHeader.Id - the same
        // class of bug as MenuItems/Upsert.cshtml.cs's missing objFromDb check.
        public IActionResult OnGet(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader == null)
            {
                return NotFound();
            }

            OrderDetailVM = new()
            {
                OrderHeader = orderHeader,
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties: "MenuItem").ToList()
            };
            return Page();
        }

        public IActionResult OnPostOrderCompleted(int orderId) // Completed
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderId, SD.StatusCompleted);
            _unitOfWork.Save();
            return RedirectToPage("OrderList");
        }


        public IActionResult OnPostOrderCancel(int orderId) // Cancel
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderId, SD.StatusCancelled);
            _unitOfWork.Save();
            return RedirectToPage("OrderList");
        }
    }
}
