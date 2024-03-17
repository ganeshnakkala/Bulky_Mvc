using System.ComponentModel.DataAnnotations;


namespace BulkyBook.Models
{
    public class OwnershipController
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
