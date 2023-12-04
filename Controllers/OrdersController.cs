using System.Security.Claims;
using MainProject.Models;
using MainProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CartService _cartService;
        private ApplicationDbContext _context;

        public OrdersController(CartService cartService, ApplicationDbContext context) // constructor
        {
            _cartService = cartService;
            _context = context;
        }

        [Authorize()] // role agnostic since brackets are empty, forces them to log in but no role specifics.
        public IActionResult Checkout() // building the snapshot OBJECT for us
        { // var automatically grabs data type
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Name identifier witll return the id actually // User.FindFirstValue is in base controller class : Controller. 
            var cart = _cartService.GetCart();

            if (cart == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                UserId = userId, // userId cannot be null even though yellow underline present. This is due to the Authorize decorator.
                Total = cart.CartItems.Sum(cartItem => (decimal)(cartItem.Quantity * cartItem.Product.MSRP)),
                OrderItems = new List<OrderItem>()
            }; // product size will be in OrderItem not Order!!

            foreach (var cartItem in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                { // this is the snapshot object built that will then be given to OrdersController to further work with it. Snapshot itself will be created during checkout and payments. This will be in flux if product price and stuff changes! 
                    OrderId = order.Id,
                    ProductName = cartItem.Product.Name,
                    Quantity = cartItem.Quantity,
                    Size = cartItem.Size,
                    Image = cartItem.Product.Image,
                    Price = cartItem.Product.MSRP
                });
            }

            if (order.OrderItems != null && order.OrderItems.Count > 0)
            {
                return View("Details", order);
            }
            else
            {
                // Redirect to the Carts/Index action if there are no order items, this prevents the user from just typing Orders/Checkout in the url and reaching that page.
                return RedirectToAction("Index", "Carts");
            }
        }
    }
}