using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> objCategoryList = _unitOfWork.Product.GetAll().ToList();
            return View(objCategoryList);

        }
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {

            _unitOfWork.Product.Add(product);
            _unitOfWork.Save();
            TempData["Success"] = "Category Changed Successfully";
            return RedirectToAction("Index"); ;
        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }
            return View();
        }
       

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteProduct(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Remove(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Deleted Successfully";

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            if (ProductFromDb == null)
            {
                return NotFound();
            }
            return View(ProductFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Product Updated Successfully";

                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
