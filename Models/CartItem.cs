// we dont need to get into DB with this because it is utilizing session storage only
namespace MainProject.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public Product Product { get; set; } = new Product();
    }
}