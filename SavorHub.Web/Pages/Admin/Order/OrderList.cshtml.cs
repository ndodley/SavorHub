using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SavorHub.Utilities;

namespace SavorHub.Web.Pages.Admin.Order
{
    [Authorize(Roles = $"{SD.ManagerRole},{SD.FrontDeskRole}")]
    public class OrderListModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
