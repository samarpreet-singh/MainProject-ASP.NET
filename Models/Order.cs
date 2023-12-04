using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MainProject.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; } = 0; // if you call it Id, ASP will automatically pick it up and make it auto increment, primary key etc but still add the key decorator.

        [Required]
        public string UserId { get; set; } = String.Empty; // Refers to the User itself! Associated to User below through ASP's intelligent syntax recognition. 
        //This creates an association to the User 
        //because we don't want an order to exist on its own. It HAS to be linked to a User! And this order cannot be accessed by other people

        [Required]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; } = 0.00M;

        [Required]
        public bool PaymentReceived { get; set; } = false;

        public IdentityUser User { get; set; } = new IdentityUser(); // This is technically a foreign key relationship to a User. Associated to UserId above through 
        //intelligent syntax recognition You can use a decorator for that too but ASP is intelligent. 
        // Decorator would be something like [ForeignKey('UserId')] but NOT SURE!!!

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Linking to OrderItem where we store all our items for the snapshot
    }
}