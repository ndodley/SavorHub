using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;
using Restaurant.Utility;

namespace RestaurantFinal.Pages.Admin.Reviews
{
    [Authorize(Roles = SD.ManagerRole)]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IEnumerable<Review> Reviews { get; set; } = [];

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnGet()
        {
            Reviews = _unitOfWork.Review.GetAll(
                includeProperties: "ApplicationUser,MenuItem,MenuItem.Category,MenuItem.FoodType",
                orderby: q => q.OrderByDescending(r => r.Date)
            );
        }
    }
}
