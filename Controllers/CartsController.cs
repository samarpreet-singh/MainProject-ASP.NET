using MainProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MainProject.Controllers
{
    public class CartsController : Controller
    {
        private readonly string _cartSessionKey; // name of the column where we put in our session key for a particular cart
        private readonly ApplicationDbContext _context; // db connection to work with DB

        public CartsController(ApplicationDbContext context)
        {
            _cartSessionKey = "Cart"; // this is the container for a cart inside the session. The session is unique to you but the container can be same for people. It is called Cart 
            //because the container NAME can be same for people but the VALUE of the key inside this container will be different for everyone and will be dynamically generated by the session itself.
            _context = context;
        }

        public async Task<IActionResult> Index() // async is usually used with input output operations since they take time to complete. async await will make sure that everything is returned IN PARALLEL with executing all the code. it happens IN PARALLEL
        {
            // Get our cart (or get a new cart if it doesn't exist)
            var cart = GetCart();

            if (cart == null)
            {
                return NotFound();
            }

            // If the cart exists, lets fill it with our products
            if (cart.CartItems.Count > 0) // we want to see if there are any items in the cart
            {
                foreach (var cartItem in cart.CartItems) // if this assignment was by copy, we would not be able to do the if (product != null) condition below. So this actually happens by reference not copy since this is a complex object so it is actually going to that memory location inside that cart item.
                {
                    /*
                    SELECT * FROM PRODUCTS
                    JOIN Departments ON Products.DepartmentId = Departments.Id
                    WHERE Products.Id = p.Id 
                    */
                    var product = await _context.Products // find the product through DB
                        .Include(p => p.Category)
                        .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId); // First or Default means return first or default value and default here is null and it is async

                    if (product != null)
                    {
                        cartItem.Product = product;  // assign product if product found is not null. Let's say someone deleted the product was deleted by someone in administration or similar scenarios.
                    } // reference or copy concept here along with pointers.
                    //in C# when you assign a native data type (PRIMITIVE VALUES NOT OBJECTS) to another value or variable it just copies that value over and does NOT refer to it through memory address!! Same in the case of Java!!!
                }
            }

            return View(cart);
        }

        [HttpPost] // tells the ACTION about what kind of http method is to be used to send, here it is through post
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var cart = GetCart(); // returns either existing or brand new cart

            if (cart == null)
            {
                return NotFound();
            }

            var cartItem = cart.CartItems.Find(cartItem => cartItem.ProductId == productId); // This is looking up the cart item and if it exists we dont want to add a new row to the cart but update the existing quantity, below code is for 
            //If it finds an item in our cart with product id equal to argument that is passed, it will 

            if (cartItem != null && cartItem.Product != null)
            {
                cartItem.Quantity += quantity; // update by the passed in quantity.
            }
            else
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == productId); // await was used here and not at line 63 because database is outside and it is input output stuff. If the item was not found in the cart already, then we have to add it to the cart. So this block of code is actually looking for the product itself in the database to finally add it to the cart because it does not already exist there.

                if (product == null)
                {
                    return NotFound();
                }

                cartItem = new CartItem { ProductId = productId, Quantity = quantity, Product = product }; // we have to create a new cartItem object in this case and add the product manually to it because it could not be found above.

                cart.CartItems.Add(cartItem); // then we add that to the cart object we created.
            }

            SaveCart(cart); // save the whole cart object based on whatever happens in one of the above if branches.

            return RedirectToAction("Index"); // index here is the index of the CART page.
        }


        private Cart? GetCart()
        {
            var cartJson = HttpContext.Session.GetString(_cartSessionKey); // HTTPCONTEXT IS THE REQUEST AND WE CAN GET ANY INFORMATION RELATED TO USER FROM IT

            return cartJson == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(cartJson); // this could return null that's why method is declared with Cart? question mark which means nullable
        }

        private void SaveCart(Cart cart) // saving
        {
            var cartJson = JsonConvert.SerializeObject(cart);

            HttpContext.Session.SetString(_cartSessionKey, cartJson);
        }
    }
}