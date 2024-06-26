using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataUtility;
using BulkyBook.Models;
using BulkyBookWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework.Internal;
using System.Diagnostics;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category,ProductImages");
            return View(productList);
        }
        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u=>u.Id==id,includeProperties: "Category,ProductImages"),
                Count=1,
                ProductId = id

            };
            Product product = _unitOfWork.Product.Get(u=>u.Id==id,includeProperties: "Category");
            return View(cart);
        }




        [HttpPost]
        [Authorize] 
        public IActionResult Details(ShoppingCart shoppingCart)
        {
           var claims = (ClaimsIdentity)User.Identity;
           var userId =claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart existingCart = _unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId==userId
            && u.ProductId==shoppingCart.ProductId);
            if (existingCart != null)
            {
                existingCart.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(existingCart);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    (int)_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart Updated Successfully";
            
            return RedirectToAction(nameof(Index));
        }





        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
