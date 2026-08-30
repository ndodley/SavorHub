using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using System.Security.Claims;

namespace SavorHub.Web.Pages.Customer.Menu
{
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
        public string? CurrentUserId { get; set; }
        public bool IsFavorite { get; set; }

        public void OnGet(int id)
        {
            CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCart = new()
            {
                ApplicationUserId = CurrentUserId ?? string.Empty,
                MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,FoodType"),
                MenuItemId = id
            };

            if (CurrentUserId != null)
            {
                IsFavorite = _unitOfWork.Favorite.GetFirstOrDefault(
                    f => f.ApplicationUserId == CurrentUserId && f.MenuItemId == id) != null;
            }

            // Load reviews for this menu item
            Reviews = _unitOfWork.Review.GetAll(r => r.MenuItemId == id, includeProperties: "ApplicationUser")
                .OrderByDescending(r => r.Date).ToList();

            // Calculate average rating
            if (Reviews.Any())
            {
                AverageRating = Reviews.Average(r => r.Rating);
            }
        }

        public IActionResult OnPost()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge();

            ShoppingCart.ApplicationUserId = userId;

            // Clear ALL ModelState (prevents NewReview validation errors from bleeding into this handler)
            // then re-validate only the ShoppingCart fields relevant to adding to cart.
            ModelState.Clear();
            TryValidateModel(ShoppingCart, nameof(ShoppingCart));

            if (ModelState.IsValid)
            {
                ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    filter: u => u.ApplicationUserId == userId &&
                                 u.MenuItemId == ShoppingCart.MenuItemId);

                if (shoppingCartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(ShoppingCart);
                    _unitOfWork.Save();
                    TempData["success"] = "Item added to cart successfully";
                }
                else
                {
                    _unitOfWork.ShoppingCart.IncrementCount(shoppingCartFromDb, ShoppingCart.Count);
                    TempData["success"] = "Cart updated successfully";
                }
                return RedirectToPage("Index");
            }

            OnGet(ShoppingCart.MenuItemId);
            return Page();
        }

        public IActionResult OnPostToggleFavorite(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge();

            var existing = _unitOfWork.Favorite.GetFirstOrDefault(
                f => f.ApplicationUserId == userId && f.MenuItemId == id);

            if (existing == null)
            {
                _unitOfWork.Favorite.Add(new Favorite { ApplicationUserId = userId, MenuItemId = id });
                TempData["success"] = "Added to favorites";
            }
            else
            {
                _unitOfWork.Favorite.Remove(existing);
                TempData["success"] = "Removed from favorites";
            }

            _unitOfWork.Save();
            return RedirectToPage(new { id });
        }

        public IActionResult OnPostAddReview()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge();

            if (string.IsNullOrWhiteSpace(NewReview.Content) || NewReview.Rating < 1 || NewReview.Rating > 5)
            {
                TempData["error"] = "Please provide a valid review with rating between 1-5 and content (10-1000 characters).";
                return RedirectToPage(new { id = NewReview.MenuItemId });
            }

            // Check if user already reviewed this item
            var existingReview = _unitOfWork.Review.GetFirstOrDefault(
                r => r.UserId == userId && r.MenuItemId == NewReview.MenuItemId);

            if (existingReview != null)
            {
                TempData["error"] = "You have already reviewed this item.";
                return RedirectToPage(new { id = NewReview.MenuItemId });
            }

            NewReview.UserId = userId;
            NewReview.Date = DateTime.Now;

            _unitOfWork.Review.Add(NewReview);
            _unitOfWork.Save();

            TempData["success"] = "Review added successfully!";
            return RedirectToPage(new { id = NewReview.MenuItemId });
        }

        public IActionResult OnPostUpdateReview(int id, int reviewId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge();

            var reviewFromDb = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == reviewId && r.UserId == userId && r.MenuItemId == id);
            if (reviewFromDb == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(NewReview.Content) || NewReview.Rating < 1 || NewReview.Rating > 5)
            {
                TempData["error"] = "Please provide a valid review with rating between 1-5 and content (10-1000 characters).";
                return RedirectToPage(new { id });
            }

            reviewFromDb.Rating = NewReview.Rating;
            reviewFromDb.Content = NewReview.Content;
            reviewFromDb.Date = DateTime.Now;
            _unitOfWork.Review.Update(reviewFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Review updated successfully!";
            return RedirectToPage(new { id });
        }

        public IActionResult OnPostDeleteReview(int id, int reviewId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge();

            var reviewFromDb = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == reviewId && r.UserId == userId && r.MenuItemId == id);
            if (reviewFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Review.Remove(reviewFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Review deleted successfully!";
            return RedirectToPage(new { id });
        }
    }
}
