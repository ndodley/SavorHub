using Restaurant.Data.Data;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Data.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _db;

        public ReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Review review)
        {
            var objFromDb = _db.Reviews.FirstOrDefault(r => r.Id == review.Id);
            if (objFromDb != null)
            {
                objFromDb.Content = review.Content;
                objFromDb.Rating = review.Rating;
                objFromDb.Date = review.Date;
            }
        }
    }
}
