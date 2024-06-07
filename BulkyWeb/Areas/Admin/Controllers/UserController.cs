using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataUtility;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager) 
        {
            
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {

            return View();

        }


       
        public IActionResult RoleManagement(string userId)
        {
            var roleID = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            var rolename = _db.Roles.FirstOrDefault(u => u.Id == roleID).Name;
            
   

            UserVM userVM = new UserVM
            {
                ApplicationUsers = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                companylist = _db.CompanyList.Select(i=>new SelectListItem { Text=i.Name,Value=i.Name}),
                //Role = _db.Roles.Select(i=> new SelectListItem { Text=i.Name,Value=i.Id.ToString()}),
                rolelist = _db.Roles.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
            };

            userVM.ApplicationUsers.Role = rolename;
            return View(userVM);
      
        }


        [HttpPost]
        public IActionResult RoleManagement(UserVM roleManagmentVM)
        {
            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.ApplicationUsers.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name; 
            if (!(roleManagmentVM.ApplicationUsers.Role == oldRole))
            {
                // a role was updated
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagmentVM.ApplicationUsers.Id);
                if (roleManagmentVM.ApplicationUsers.Role == SD.Role_Company)
                {
                    applicationUser.IdofCompany = roleManagmentVM.ApplicationUsers.IdofCompany;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.IdofCompany = null;
                }
                _db.SaveChanges();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUsers.Role).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
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
