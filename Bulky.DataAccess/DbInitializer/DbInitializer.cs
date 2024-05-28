using BulkyBook.DataAccess.Data;
using BulkyBook.DataUtility;
using BulkyBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDBInitializer
    {
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<IdentityUser> _userManager;
        public readonly ApplicationDbContext _db;

        public DbInitializer(RoleManager<IdentityRole> roleManager,
                             UserManager<IdentityUser> userManager,
                             ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            if (!_roleManager.RoleExistsAsync(SD.Role_Company).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();


                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@dotnetmastery.com",
                    Email = "admin@dotnetmastery.com",
                    Name = "Ganesh Nakkala",
                    PhoneNumber = "9876543211",
                    StreetAddress = "west loop 24",
                    State = "iL",
                    PostalCode = "54698",
                    City = "Chicago",
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");
                _userManager.AddToRoleAsync(user,SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
