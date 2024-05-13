using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IOrderDetialRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
      
    }
}
