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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitofwork;
        public UserController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager,IUnitOfWork unitOfWork) 
        {
            _unitofwork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {

            return View();

        }

        public IActionResult RoleManagement(string userId)
        {
            UserVM userVM = new UserVM
            {
                ApplicationUsers = _unitofwork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                companylist = _unitofwork.Company.GetAll().Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                rolelist = _roleManager.Roles.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
            };
            userVM.ApplicationUsers.Role = _userManager.GetRolesAsync(_unitofwork.ApplicationUser.Get(u=>u.Id == userId)).GetAwaiter().
                                            GetResult().FirstOrDefault();
            return View(userVM);
        }


        [HttpPost]
        public IActionResult RoleManagement(UserVM roleManagmentVM)
        {
            var user = _unitofwork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUsers.Id);
            if (user == null)
            {
                // Handle the case where the user does not exist
                return NotFound();
            }
            

            // Retrieve the old role
            string oldrole = _userManager.GetRolesAsync(_unitofwork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUsers.Id)).GetAwaiter().GetResult().FirstOrDefault();
            string rolename=  _roleManager.FindByIdAsync(roleManagmentVM.ApplicationUsers.Role).GetAwaiter().GetResult().Name;

            ApplicationUser applicationUser = _unitofwork.ApplicationUser.
                                              Get(u => u.Id == roleManagmentVM.ApplicationUsers.Id);
            // Update the role and company if needed
            if (!(roleManagmentVM.ApplicationUsers.Role == oldrole))
            {
                if (roleManagmentVM.ApplicationUsers.Role == SD.Role_Company)
                {
                    applicationUser.IdofCompany = roleManagmentVM.ApplicationUsers.IdofCompany;
                }
                if (oldrole == SD.Role_Company)
                {
                    applicationUser.IdofCompany = null;
                }


                _unitofwork.ApplicationUser.Update(applicationUser);
                _unitofwork.Save();

                 _userManager.RemoveFromRoleAsync(applicationUser, oldrole).GetAwaiter().GetResult();
                 _userManager.AddToRoleAsync(applicationUser, rolename).GetAwaiter().GetResult();
        
            }
            else
            {
                if (oldrole == SD.Role_Company && applicationUser.IdofCompany!= roleManagmentVM.ApplicationUsers.IdofCompany)
                {
                    applicationUser.IdofCompany = roleManagmentVM.ApplicationUsers.IdofCompany;
                    _unitofwork.ApplicationUser.Update(applicationUser);
                    _unitofwork.Save();
                }
            }

            return RedirectToAction("Index");
        }










        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitofwork.ApplicationUser.GetAll(includeProperties:"Company").ToList();
            
            foreach(var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
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
            var objFromDb = _unitofwork.ApplicationUser.Get(u => u.Id == id);
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

            _unitofwork.ApplicationUser.Update(objFromDb);
            _unitofwork.Save();
            return Json(new { success = true, message = "Operation successfull"});
        }




        #endregion
    }
}
