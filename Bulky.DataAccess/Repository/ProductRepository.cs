  using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>,IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Add(Product entity)
        {
            _db.Add(entity);
        }
        public new void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u=>u.Id== obj.Id);
            if (objFromDb != null)
            {


                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.Category = obj.Category;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Author = obj.Author;
                objFromDb.ProductImages = obj.ProductImages;
                //if(obj.ImageUrl!= null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}

            }
        }
    }
}