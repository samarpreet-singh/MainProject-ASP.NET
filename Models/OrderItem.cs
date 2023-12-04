using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Models
{
    public class OrderItem // see comments in Order.cs to understand this because functionality is very similar.
    // Also we are not storing cart and cart items here because they are in the session and not to be written to the database!! The user's ORDER is stored in DB though so that info has to be here
    {
        [Key]
        public int Id { get; set; } = 0;

        [Required]
        public int OrderId { get; set; } = 0;

        [Required]
        public string ProductName { get; set; } = String.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } = 0.00M;

        [Required]
        public int Quantity { get; set; } = 0;

        [Required]
        public ProductSize Size { get; set; }

        [StringLength(250)]
        public string? Image { get; set; } = String.Empty;

        [ForeignKey("OrderId")] // direct decorator here because to demonstrate it is the same without it.
        public virtual Order? Order { get; set; }
    }
}