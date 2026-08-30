using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using System.Security.Claims;

namespace SavorHub.Web.Pages.Customer.Reviews
{
    [Authorize]
    public class MyReviewsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public MyReviewsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Review> Reviews { get; set; }

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Reviews = _unitOfWork.Review.GetAll(
                filter: r => r.UserId == userId,
                includeProperties: "MenuItem,MenuItem.Category,MenuItem.FoodType",
                orderby: q => q.OrderByDescending(r => r.Date)
            );
        }

        public IActionResult OnPostDelete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = _unitOfWork.Review.GetFirstOrDefault(
                filter: r => r.Id == id && r.UserId == userId
            );

            if (review == null)
            {
                return NotFound();
            }

            _unitOfWork.Review.Remove(review);
            _unitOfWork.Save();

            TempData["success"] = "Review deleted successfully";
            return RedirectToPage();
        }
    }
}
