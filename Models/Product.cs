using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Models
{
    public enum ProductSize
    {
        XS,
        S,
        M,
        L,
        XL,
    }

    public class Product
    {
        [Key] // data annotation to specify this is a primary key.
        public int Id { get; set; } = 0;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; } = 0;

        [Required, StringLength(300)]
        public string Name { get; set; } = String.Empty; // this empty initialization tells it that an INSTANCE of this can be empty or will be empty. when you push this to a database using migrations, it will NOT have an empty string.

        [StringLength(1000)]
        public string? Description { get; set; } = String.Empty;

        [StringLength(250)]
        public string? Image { get; set; } = String.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        [DataType(DataType.Currency)]
        public decimal MSRP { get; set; } = 0.01M;

        [Required]
        public ProductSize Size { get; set; } = ProductSize.S;

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; } // creates the association to categories, allows a category to be stored in an instance of a product
    }
}