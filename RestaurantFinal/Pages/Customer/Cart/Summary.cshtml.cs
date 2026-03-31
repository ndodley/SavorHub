using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;
using Restaurant.Utility;
using Stripe.Checkout;
using System.Security.Claims;

namespace RestaurantFinal.Pages.Customer.Cart
{
    [Authorize]
    [BindProperties]
    public class SummaryModel : PageModel
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
        private readonly IUnitOfWork _unitOfWork;
        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            OrderHeader = new OrderHeader();
        }
        public void OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(filter: u => u.ApplicationUserId == claim.Value,
                    includeProperties: "MenuItem,MenuItem.FoodType,MenuItem.Category");

                foreach (var cartItem in ShoppingCartList)
                {
                    OrderHeader.OrderTotal += (cartItem.MenuItem.Price * cartItem.Count);
                }
                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
                OrderHeader.PickupName = applicationUser.FirstName + " " + applicationUser.LastName;
                OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            }
        }

        public IActionResult OnPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                filter: u => u.ApplicationUserId == claim.Value,
                includeProperties: "MenuItem,MenuItem.FoodType,MenuItem.Category");

            foreach (var cartItem in ShoppingCartList)
            {
                OrderHeader.OrderTotal += (cartItem.MenuItem.Price * cartItem.Count);
            }

            // Save order as Pending until Stripe confirms payment
            OrderHeader.Status = SD.StatusPending;
            OrderHeader.OrderDate = DateTime.Now;
            OrderHeader.UserId = claim.Value;
            OrderHeader.PickUpTime = Convert.ToDateTime(
                OrderHeader.PickUpDate.ToShortDateString() + " " +
                OrderHeader.PickUpTime.ToShortTimeString());

            _unitOfWork.OrderHeader.Add(OrderHeader);
            _unitOfWork.Save();

            foreach (var item in ShoppingCartList)
            {
                _unitOfWork.OrderDetail.Add(new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = OrderHeader.Id,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                });
            }
            _unitOfWork.Save();

            // Fetch the current user's email to pre-fill Stripe checkout.
            // Pre-filling CustomerEmail prevents Stripe from rendering the email
            // input field, which is what triggers Stripe Link account-matching.
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                u => u.Id == claim.Value);

            // Build Stripe Checkout Session
            var domain = $"{Request.Scheme}://{Request.Host}/";
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={OrderHeader.Id}",
                CancelUrl = domain + "Customer/Cart/Index",
                PaymentMethodTypes = ["card"],
                // Pre-fill email so Stripe skips the email step and does not
                // prompt for Stripe Link authentication.
                CustomerEmail = applicationUser?.Email,
                // 'auto' only collects billing address fields that are required
                // by the card network — in practice this means only ZIP code.
                BillingAddressCollection = "auto",
                LineItems = []
            };

            foreach (var item in ShoppingCartList)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)Math.Round(item.MenuItem.Price * 100, 0),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.MenuItem.Name
                        }
                    },
                    Quantity = item.Count
                });
            }

            var service = new SessionService();
            Session session = service.Create(options);

            OrderHeader.SessionId = session.Id;
            OrderHeader.PaymentIntentId = session.PaymentIntentId;
            _unitOfWork.OrderHeader.Update(OrderHeader);
            _unitOfWork.Save();

            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}

