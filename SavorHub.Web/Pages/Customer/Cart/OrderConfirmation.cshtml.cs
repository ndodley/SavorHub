using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Utilities;
using Stripe.Checkout;

namespace SavorHub.Web.Pages.Customer.Cart
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public int OrderId { get; set; }
       public List<OrderDetails> Items { get; private set; } = [];
        public OrderConfirmationModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult OnGet(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            if (orderHeader.SessionId != null)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
                {
                    orderHeader.Status = SD.StatusSubmitted;
                    orderHeader.TransactionId = session.PaymentIntentId;
                    orderHeader.PaymentIntentId = session.PaymentIntentId;
                    _unitOfWork.OrderHeader.Update(orderHeader);

                    List<ShoppingCart> shoppingCarts =
                        _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.UserId).ToList();
                    _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);

                    _unitOfWork.Save();
                }
                else
                {
                    // Payment not completed — redirect back to cart
                    return RedirectToPage("Index");
                }
            }

            OrderId = id;
          Items = _unitOfWork.OrderDetail
                .GetAll(d => d.OrderId == id, includeProperties: "MenuItem")
                .ToList();
            return Page();
        }
    }
}

