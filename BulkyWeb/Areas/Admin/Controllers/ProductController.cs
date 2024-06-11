using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataUtility;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) 
        { 
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProductList);

        }
        public IActionResult Upsert(int? id) 
        {
            
            //ViewBag.CategoryList = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
              .GetAll().Select(u => new SelectListItem
              {
                  Text = u.Name,
                  Value = u.Id.ToString()

              }),
                
                Product = new Product()
            };
            if(id==null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id,includeProperties: "ProductImages");
                return View(productVM);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0 || productVM.Product.Id == null)
                {
                    _unitOfWork.Product.Add(productVM.Product);

                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);

                }
                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);
                        // string finalPath = Path.Combine(wwwRootPath, "images", "product");

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId= (int)productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                       productVM.Product.ProductImages.Add(productImage);
                        //_unitOfWork.ProductImage.Add(productImage);
                    }
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
            
                }

                TempData["Success"] = "Product created/updated Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()

                });
                return View(productVM);
            }

        }

        public IActionResult DeleteImage(int imageid)
        {
            var imageToBeDeleted  = _unitOfWork.ProductImage.Get(u=>u.Id==imageid);
            int idOfimageToBeDeleted = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if(!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.Trim('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();
                TempData["success"] = "Image Deleted successfully";
            }
            return RedirectToAction(nameof(Upsert), new { id = idOfimageToBeDeleted });
        }


        #region API CALLS
        [HttpGet]    
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product
                .GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

       // [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false , message="error in deleting"});
            }
            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            // string finalPath = Path.Combine(wwwRootPath, "images", "product");

            if (Directory.Exists(finalPath))
            {
                string[] filepaths = Directory.GetFiles(finalPath);
                foreach (string filepath in filepaths)
                {
                    System.IO.File.Delete(filepath);
                }
                Directory.Delete(finalPath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "deleted successfully"});
          //  return View()
        }
        #endregion
    }
}
