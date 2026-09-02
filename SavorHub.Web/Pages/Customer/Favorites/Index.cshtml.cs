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

        public Dictionary<int, ShoppingCart> CartByMenuItemId { get; set; } = [];

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            CategoryList = _unitOfWork.Category.GetAll(orderby: u => u.OrderBy(c => c.DisplayOrder));

            Favorites = _unitOfWork.Favorite.GetAll(
                f => f.ApplicationUserId == userId,
                includeProperties: "MenuItem,MenuItem.Category,MenuItem.FoodType",
                orderby: q => q.OrderByDescending(f => f.DateCreated)
            );

            CartByMenuItemId = _unitOfWork.ShoppingCart
                .GetAll(sc => sc.ApplicationUserId == userId)
                .ToDictionary(sc => sc.MenuItemId, sc => sc);
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

        public IActionResult OnPostAddToCart(int menuItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.ApplicationUserId == userId && sc.MenuItemId == menuItemId);
            if (cart == null)
            {
                _unitOfWork.ShoppingCart.Add(new ShoppingCart
                {
                    ApplicationUserId = userId,
                    MenuItemId = menuItemId,
                    Count = 1
                });
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostIncrementCart(int menuItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.ApplicationUserId == userId && sc.MenuItemId == menuItemId);
            if (cart != null)
            {
                _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDecrementCart(int menuItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.ApplicationUserId == userId && sc.MenuItemId == menuItemId);
            if (cart != null)
            {
                if (cart.Count <= 1)
                {
                    _unitOfWork.ShoppingCart.Remove(cart);
                    _unitOfWork.Save();
                }
                else
                {
                    _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
                }
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveCart(int menuItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.ApplicationUserId == userId && sc.MenuItemId == menuItemId);
            if (cart != null)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();
            }
            return RedirectToPage();
        }
    }
}
