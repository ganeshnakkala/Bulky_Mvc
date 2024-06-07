using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class UserVM : ApplicationUser
    {
        public ApplicationUser ApplicationUsers { get; set; }
        public IEnumerable<SelectListItem> rolelist { get; set; }
        public IEnumerable<SelectListItem> companylist { get; set; }
    }
}
