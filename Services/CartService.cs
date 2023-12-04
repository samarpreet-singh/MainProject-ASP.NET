using MainProject.Models;
using Newtonsoft.Json;

namespace MainProject.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private const string _cartSessionKey = "Cart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Cart? GetCart()
        {
            var cartJson = _httpContextAccessor.HttpContext.Session.GetString(_cartSessionKey); // HTTPCONTEXT IS THE REQUEST AND WE CAN GET ANY INFORMATION RELATED TO USER FROM IT

            return cartJson == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(cartJson); // this could return null that's why method is declared with Cart? question mark which means nullable
        }

        public void SaveCart(Cart cart) // saving
        {
            var cartJson = JsonConvert.SerializeObject(cart);

            _httpContextAccessor.HttpContext.Session.SetString(_cartSessionKey, cartJson);
        }
    }
}