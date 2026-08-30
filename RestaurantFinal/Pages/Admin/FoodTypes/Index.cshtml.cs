using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;

namespace SavorHub.Web.Pages.Admin.FoodTypes
{
    public class IndexModel : PageModel
    {
        // UnitOfWork
        private readonly IUnitOfWork _unitOfWork;

        public IEnumerable<FoodType> FoodTypes { get; set; }
        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void OnGet()
        {
            FoodTypes = _unitOfWork.FoodType.GetAll();
        }
    }
}
