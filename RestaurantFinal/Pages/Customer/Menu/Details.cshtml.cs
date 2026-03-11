using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;
using System.Security.Claims;

namespace RestaurantFinal.Pages.Customer.Menu
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public ShoppingCart ShoppingCart { get; set; }

        [BindProperty]
        public Review NewReview { get; set; }

        public IList<Review> Reviews { get; set; }
        public double AverageRating { get; set; }

        public void OnGet(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            ShoppingCart = new()
            {
                ApplicationUserId = claim.Value,
                MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,FoodType"),
                MenuItemId = id
            };

            // Load reviews for this menu item
            Reviews = _unitOfWork.Review.GetAll(r => r.MenuItemId == id, includeProperties: "ApplicationUser").OrderByDescending(r => r.Date).ToList();

            // Calculate average rating
            if (Reviews.Any())
            {
                AverageRating = Reviews.Average(r => r.Rating);
            }
        }


        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                   filter: u => u.ApplicationUserId == ShoppingCart.ApplicationUserId &&
                    u.MenuItemId == ShoppingCart.MenuItemId);

                if (shoppingCartFromDb == null)
                {

                    _unitOfWork.ShoppingCart.Add(ShoppingCart);
                    _unitOfWork.Save();
                    //HttpContext.Session.SetInt32(SD.SessionCart,
                    //_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == ShoppingCart.ApplicationUserId).ToList().Count);
                }
                else
                {
                    _unitOfWork.ShoppingCart.IncrementCount(shoppingCartFromDb, ShoppingCart.Count);
                }
                return RedirectToPage("Index");
            }
            return Page();
        }

        public IActionResult OnPostAddReview()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(NewReview.Content) || NewReview.Rating < 1 || NewReview.Rating > 5)
            {
                TempData["error"] = "Please provide a valid review with rating between 1-5 and content (10-1000 characters).";
                return RedirectToPage(new { id = NewReview.MenuItemId });
            }

            // Check if user already reviewed this item
            var existingReview = _unitOfWork.Review.GetFirstOrDefault(
                r => r.UserId == claim.Value && r.MenuItemId == NewReview.MenuItemId);

            if (existingReview != null)
            {
                TempData["error"] = "You have already reviewed this item.";
                return RedirectToPage(new { id = NewReview.MenuItemId });
            }

            NewReview.UserId = claim.Value;
            NewReview.Date = DateTime.Now;

            _unitOfWork.Review.Add(NewReview);
            _unitOfWork.Save();

            TempData["success"] = "Review added successfully!";
            return RedirectToPage(new { id = NewReview.MenuItemId });
        }
    }
}
