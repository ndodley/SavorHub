using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;
using System.Security.Claims;

namespace RestaurantFinal.Pages.Customer.Menu
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<MenuItem> MenuItemList { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
        public Dictionary<int, (double AverageRating, int ReviewCount)> MenuItemRatings { get; set; }

        public HashSet<int> FavoriteMenuItemIds { get; set; } = [];

        public void OnGet()
        {
            MenuItemList = _unitOfWork.MenuItem.GetAll(includeProperties: "Category,FoodType");
            CategoryList = _unitOfWork.Category.GetAll(orderby: u => u.OrderBy(c => c.DisplayOrder));

            // Get all reviews and calculate ratings for each menu item
            var allReviews = _unitOfWork.Review.GetAll();
            MenuItemRatings = MenuItemList.ToDictionary(
                mi => mi.Id,
                mi =>
                {
                    var reviews = allReviews.Where(r => r.MenuItemId == mi.Id).ToList();
                    var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
                    return (avgRating, reviews.Count);
                }
            );

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                FavoriteMenuItemIds = _unitOfWork.Favorite
                    .GetAll(f => f.ApplicationUserId == userId)
                    .Select(f => f.MenuItemId)
                    .ToHashSet();
            }
        }

        public IActionResult OnPostToggleFavorite(int menuItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var existing = _unitOfWork.Favorite.GetFirstOrDefault(f => f.ApplicationUserId == userId && f.MenuItemId == menuItemId);
            if (existing == null)
            {
                _unitOfWork.Favorite.Add(new Favorite
                {
                    ApplicationUserId = userId,
                    MenuItemId = menuItemId
                });
                TempData["success"] = "Added to favorites";
            }
            else
            {
                _unitOfWork.Favorite.Remove(existing);
                TempData["success"] = "Removed from favorites";
            }

            _unitOfWork.Save();
            return RedirectToPage();
        }
    }
}
