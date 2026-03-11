using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;

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
        }
    }
}
