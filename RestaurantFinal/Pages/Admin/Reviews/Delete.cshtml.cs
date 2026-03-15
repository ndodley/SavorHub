using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;
using Restaurant.Utility;

namespace RestaurantFinal.Pages.Admin.Reviews
{
    [Authorize(Roles = SD.ManagerRole)]
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public Review Review { get; set; } = default!;

        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult OnGet(int id)
        {
            var reviewFromDb = _unitOfWork.Review.GetFirstOrDefault(
                r => r.Id == id,
                includeProperties: "ApplicationUser,MenuItem,MenuItem.Category,MenuItem.FoodType"
            );

            if (reviewFromDb == null)
            {
                return NotFound();
            }

            Review = reviewFromDb;
            return Page();
        }

        public IActionResult OnPost()
        {
            var reviewFromDb = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == Review.Id);
            if (reviewFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Review.Remove(reviewFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Review deleted successfully";
            return RedirectToPage("Index");
        }
    }
}
