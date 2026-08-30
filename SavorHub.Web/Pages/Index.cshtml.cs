using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;

namespace SavorHub.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public IndexModel(ILogger<IndexModel> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IList<MenuItem> FeaturedItems { get; set; } = [];
    public Dictionary<int, (double AverageRating, int ReviewCount)> MenuItemRatings { get; set; } = [];

    public void OnGet()
    {
        var allItems = _unitOfWork.MenuItem.GetAll(includeProperties: "Category,FoodType").ToList();
        var allReviews = _unitOfWork.Review.GetAll().ToList();

        MenuItemRatings = allItems.ToDictionary(
            mi => mi.Id,
            mi =>
            {
                var reviews = allReviews.Where(r => r.MenuItemId == mi.Id).ToList();
                var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0.0;
                return (avgRating, reviews.Count);
            }
        );

        FeaturedItems = allItems
            .OrderByDescending(mi => MenuItemRatings[mi.Id].AverageRating)
            .ThenByDescending(mi => MenuItemRatings[mi.Id].ReviewCount)
            .ToList();
    }
}
