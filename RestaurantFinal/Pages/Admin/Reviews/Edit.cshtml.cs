using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Utilities;

namespace SavorHub.Web.Pages.Admin.Reviews
{
    [Authorize(Roles = SD.ManagerRole)]
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        // Only bind the three fields we actually post — avoids ModelState errors
        // from non-nullable navigation properties (ApplicationUser, MenuItem) that
        // are never included in the form submission.
        [BindProperty] public int ReviewId { get; set; }
        [BindProperty] public int Rating { get; set; }
        [BindProperty] public string Content { get; set; } = string.Empty;

        // Read-only, populated on GET and on failed POST for display only
        public Review Review { get; set; } = default!;

        public EditModel(IUnitOfWork unitOfWork)
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
            ReviewId = reviewFromDb.Id;
            Rating = reviewFromDb.Rating;
            Content = reviewFromDb.Content;
            return Page();
        }

        public IActionResult OnPost()
        {
            var reviewFromDb = _unitOfWork.Review.GetFirstOrDefault(
                r => r.Id == ReviewId,
                includeProperties: "ApplicationUser,MenuItem,MenuItem.Category,MenuItem.FoodType"
            );

            if (reviewFromDb == null)
            {
                return NotFound();
            }

            if (Rating < 1 || Rating > 5)
            {
                ModelState.AddModelError("Rating", "Rating must be between 1 and 5.");
            }
            if (string.IsNullOrWhiteSpace(Content) || Content.Length > 1000)
            {
                ModelState.AddModelError("Content", "Content is required and must be 1000 characters or less.");
            }

            if (!ModelState.IsValid)
            {
                Review = reviewFromDb;
                return Page();
            }

            reviewFromDb.Rating = Rating;
            reviewFromDb.Content = Content;
            reviewFromDb.Date = DateTime.Now;
            _unitOfWork.Review.Update(reviewFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Review updated successfully";
            return RedirectToPage("Index");
        }
    }
}
