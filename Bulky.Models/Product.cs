using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models
{
    public class Product
    {
        
        public int? Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? ISBN { get; set; }
        [Required]
        public string? Author { get; set; }

        [Required]
        [DisplayName("List Price")]
        [Range(1, 1000)]
        public double? ListPrice { get; set; }

        [Required]
        [DisplayName("Price for 1-50")]
        [Range(1,1000)]
        public double? Price { get; set; }

        [Required]
        [DisplayName("Price for 50+")]
        [Range(1, 1000)]
        public double? Price50 { get; set; }

        [Required]
        [DisplayName("Price for 100+")]
        [Range(1, 1000)]
        public double? Price100 { get; set; }

        //Now if I add just this column, table will not know that this is a foreign key to the category table.
        //In order to explicitly define that,we need a navigation property to the category table,
        //So right here, we will have a navigation property to category table and I will call that category.
        //And on top of that we have to explicitly define that this category property is used for foreign key navigation for the category ID.
       public int CategoryId {  get; set; }
       [ForeignKey("CategoryId")]
        [ValidateNever]
       public Category? Category {  get; set; }
        [ValidateNever]
       public string? ImageUrl { get; set; }
    }
}
