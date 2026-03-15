using Restaurant.Data.Data;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;

namespace Restaurant.Data.Repository
{
    public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
