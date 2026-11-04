using Microsoft.AspNetCore.Mvc;
using SavorHub.Data.Repository.IRepository;
using System.Security.Claims;

namespace SavorHub.Web.ViewComponents
{
    // Renders the item-count badge next to the navbar cart icon.
    // Shows the total quantity of items in the signed-in user's cart.
    public class CartCountViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartCountViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var count = 0;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                count = _unitOfWork.ShoppingCart
                    .GetAll(sc => sc.ApplicationUserId == userId)
                    .Sum(sc => sc.Count);
            }

            return View(count);
        }
    }
}
