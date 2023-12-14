using System.Security.Claims;
using MainProject.Models;
using MainProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace MainProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CartService _cartService;
        private ApplicationDbContext _context;

        private readonly IConfiguration _configuration;

        public OrdersController(CartService cartService, ApplicationDbContext context, IConfiguration configuration) // constructor will return the singleton if it already exists, that is just how the constructor works. A session is a singleton too! STRIPE REQUIRES HTTPS
        {
            _cartService = cartService;
            _context = context;
            _configuration = configuration;
        }

        [Authorize()]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // claim types is helper library that grabs info from identity library. Grabs the user id from the identity library

            if (userId == null) return NotFound();

            var orders = await _context.Orders // .Include calls the passed function and provides an argument to that param and we use that argument to tell it what to return back. 
                .Include(order => order.OrderItems)
                .Include(order => order.User)
                .Where(order => order.UserId == userId)
                .Where(order => order.PaymentReceived == true)
                .OrderByDescending(order => order.Id)
                .ToListAsync(); // Required step where it converts everything into an array.
                //It ends up creating a sql statetment that we need to get orders
                // the where clauses can go before include and vice versa and that goes for all of them because of in built functionality.
            
            return View(orders);
        }

        [Authorize()]
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return NotFound();

            var order = await _context.Orders
                .Include(order => order.OrderItems)
                .Include(order => order.User)
                .Where(order => order.UserId == userId)
                .FirstOrDefaultAsync(order => order.Id == id);

            return View(order);
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

        [Authorize()]
        [HttpPost]
        public IActionResult ProcessPayment()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = _cartService.GetCart();

            if (userId == null || cart == null) return NotFound();

            var order = new Order
            {
                UserId = userId,
                Total = cart.CartItems.Sum(cartItem => (decimal)(cartItem.Quantity * cartItem.Product.MSRP)),
                OrderItems = new List<OrderItem>()
            };

            //Set Stripe API Key
            StripeConfiguration.ApiKey = _configuration.GetSection("Stripe")["SecretKey"];

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)order.Total * 100, // hover over UnitAmount and it has to be in cents that is why we multiply 
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "CraftyCreations Purchase"
                            }
                        },
                        Quantity = 1
                    }
                },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Orders/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Carts/ViewMyCart"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url); // Sending them to stripe for the payment
            return new StatusCodeResult(303); // Permanent redirect code is 303, 303 is used when you want to redirect to another link and companies use it when they want users to be redirected and it also caches so google will clear out the old links over time
        }

        [Authorize()]
        public async Task<IActionResult> SaveOrder()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = _cartService.GetCart();

            if (userId == null || cart == null) return NotFound();

            var order = new Order // stubing out an object to be inserted into DB
            {
                UserId = userId,
                Total = cart.CartItems.Sum(cartItem => (decimal)(cartItem.Quantity * cartItem.Product.MSRP)),
                OrderItems = new List<OrderItem>(),
                PaymentReceived = true
            };

            foreach (var cartItem in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id, // reference location to the id. Once it writes to the db, it will use that. Before that, memory location is null.
                    ProductName = cartItem.Product.Name,
                    Image = cartItem.Product.Image,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.MSRP
                });
            }

            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            _cartService.DestroyCart();

            return RedirectToAction("Details", new { id = order.Id });
        }
    }
}