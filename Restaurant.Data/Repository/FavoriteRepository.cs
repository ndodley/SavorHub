using SavorHub.Data.Data;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;

namespace SavorHub.Data.Repository
{
    public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
