using MainProject.Models;
using Newtonsoft.Json;

namespace MainProject.Services
{
    public class CartService // Services purpose is to create an interface for data at a global level. We dont want to instantiate the carts controller inside orders so this makes much more sense!!
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // Any request coming through passes through here. Sessions live on IHttpContextAccessor and so do requests!! So if we want to access those we need this here! CartsController has automatic access to all that stuff because of using Microsoft.AspNetCore.Mvc; but this Service does not!!

        private const string _cartSessionKey = "Cart"; // this is the container inside the session. The session is unique to you but the container can be same for people

        public CartService(IHttpContextAccessor httpContextAccessor) // constructor with the http context accessor
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
        
        public void DestroyCart()
        {
            _httpContextAccessor.HttpContext.Session.Remove(_cartSessionKey); // removes the cart session
        }
    }
}