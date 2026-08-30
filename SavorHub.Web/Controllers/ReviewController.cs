using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using System.Security.Claims;

namespace SavorHub.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allReviews = _unitOfWork.Review.GetAll(r => r.UserId == userId);
            return Json(new { data = allReviews });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == id && r.UserId == userId);
            if (review == null)
            {
                return NotFound();
            }
            return Json(review);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Review review)
        {
            if (review == null || id != review.Id)
            {
                return BadRequest();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviewFromDb = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == id && r.UserId == userId);
            if (reviewFromDb == null)
            {
                return NotFound();
            }
            reviewFromDb.Content = review.Content;
            reviewFromDb.Rating = review.Rating;
            reviewFromDb.Date = DateTime.Now;
            _unitOfWork.Review.Update(reviewFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Review updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var review = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == id);
            if (review == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Review.Remove(review);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
