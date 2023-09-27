using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Models
{
    public class Product
    {
        [Key] // data annotation to specify this is a primary key.
        public int Id { get; set; } = 0;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; } = 0;

        [Required, StringLength(300)]
        public string Name { get; set; } = String.Empty;

        [StringLength(1000)]
        public string? Description { get; set; } = String.Empty;

        [StringLength(250)]
        public string? Image { get; set; } = String.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        [DataType(DataType.Currency)]
        public decimal MSRP { get; set; } = 0.01M;

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}