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
    public class OrderDetailRepository : Repository<OrderDetails>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetails obj)
        {
            _db.OrderDetails.Update(obj);
        }
    }
}
