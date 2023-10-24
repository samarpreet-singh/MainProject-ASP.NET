using System.ComponentModel.DataAnnotations;

namespace MainProject.Models
{
    public class Category
    {
        [Key] // data annotation to specify this is a primary key.
        public int Id { get; set; } = 0;

        [Required, StringLength(200)]
        public string Name { get; set; } = String.Empty;

        [StringLength(1000)]
        public string? Description { get; set; } = String.Empty;

        [StringLength(250)]
        public string? Image { get; set; } = String.Empty;

        // relationship with products and a place to store the reference to the product in the category instance.
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}