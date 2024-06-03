using BulkyBook.DataAccess.Data;
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
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using ZendeskApi_v2.Models.Constants;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class UserController : Controller
    {
        
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db) 
        {
            _db = db;
        }
        public IActionResult Index()
        {

            return View();

        }

        #region API CALLS
        [HttpGet]    
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u=>u.Company).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach(var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = objUserList });
        }

       [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message="Error while Locking/Unlocking" });
            }
            if (objFromDb != null && objFromDb.LockoutEnd>DateTime.Now)
            {
                //user id locked already - we need unlock operaion
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                //user id unlocked already - we need lock operaion
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1);
            }

            _db.SaveChanges();
            return Json(new { success = true, message = "Operation successfull"});
        }
        #endregion
    }
}
