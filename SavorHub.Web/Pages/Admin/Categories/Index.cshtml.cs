using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Utilities;

namespace SavorHub.Web.Pages.Admin.Categories
{
    [Authorize(Roles = SD.ManagerRole)]
    public class IndexModel : PageModel
    {
        // UnitOfWork
        private readonly IUnitOfWork _unitOfWork;

        public IEnumerable<Category> Categories { get; set; }

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void OnGet()
        {
            Categories = _unitOfWork.Category.GetAll();
        }
    }
}
