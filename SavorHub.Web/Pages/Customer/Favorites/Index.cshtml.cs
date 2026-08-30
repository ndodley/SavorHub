using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using System.Security.Claims;

namespace SavorHub.Web.Pages.Customer.Favorites
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Favorite> Favorites { get; set; } = [];

        public IEnumerable<Category> CategoryList { get; set; } = [];

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            CategoryList = _unitOfWork.Category.GetAll(orderby: u => u.OrderBy(c => c.DisplayOrder));

            Favorites = _unitOfWork.Favorite.GetAll(
                f => f.ApplicationUserId == userId,
                includeProperties: "MenuItem,MenuItem.Category,MenuItem.FoodType",
                orderby: q => q.OrderByDescending(f => f.DateCreated)
            );
        }

        public IActionResult OnPostRemove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fav = _unitOfWork.Favorite.GetFirstOrDefault(f => f.Id == id && f.ApplicationUserId == userId);
            if (fav == null)
            {
                return NotFound();
            }

            _unitOfWork.Favorite.Remove(fav);
            _unitOfWork.Save();

            TempData["success"] = "Removed from favorites";
            return RedirectToPage();
        }
    }
}
