using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace ApparelPlus.Controllers
{
    public class StoreController : Controller
    {
        // shared db connection
        private readonly ApplicationDbContext _context;

        // configuration to read stripe key from appsettings
        private readonly IConfiguration _configuration;

        // constructor to instantiate db connection & read vars from appsettings
        public StoreController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: /Store => show Categories so user can choose 1
        public IActionResult Index()
        {
            // fetch Category list
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();

            // show view and pass Category list data for display
            return View(categories);
        }

        // GET: /Store/Products/5 => show Products in selected Category
        public IActionResult Products(int id)
        {
            // fetch Category to display the Name as the page heading & title
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            ViewData["Category"] = category.Name;

            // fetch products in this category
            var products = _context.Products
                .Where(p => p.CategoryId == id)
                .OrderBy(p => p.Name)
                .ToList();

            return View(products);
        }

        // POST: /Store/AddToCart => save item to user's cart in db
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult AddToCart(int productId, int quantity)
        {
            // find product price
            var product = _context.Products.Find(productId);
            var price = product.Price;

            // identify customer; hard-code at first
            // var customerId = "Test Customer";
            var customerId = GetCustomerId();

            // create & save new CartItem
            var cartItem = new CartItem
            {
                ProductId = productId,
                Price = price,
                Quantity = quantity,
                CustomerId = customerId
            };

            _context.CartItems.Add(cartItem);
            _context.SaveChanges();

            // show Cart page
            return RedirectToAction("Cart");
        }

        private string GetCustomerId()
        {
            // if no session var for CustomerId, create one using Globally Unique Identifier (GUID)
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                HttpContext.Session.SetString("CustomerId", Guid.NewGuid().ToString());
            }

            return HttpContext.Session.GetString("CustomerId");
        }

        // GET: /Store/Cart => show current user's full cart
        public IActionResult Cart()
        {
            // identify cart from session var
            var customerId = GetCustomerId();

            // get CartItems from db for this customer
            var cartItems = _context.CartItems
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Product) // parent ref to include Product Name for display
                .ToList();

            // store Item Count in session var
            var itemCount = (from c in cartItems select c.Quantity).Sum();
            HttpContext.Session.SetInt32("ItemCount", itemCount);

            // show view and pass CartItem data
            return View(cartItems);
        }

        // GET: /Store/DeleteFromCart/22 => remove item from user's cart & refresh
        public IActionResult DeleteFromCart(int id)
        {
            var cartItem = _context.CartItems.Find(id);
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

        // GET: /Store/Checkout => show form to capture customer info
        // customer must log in now
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        // POST: /Store/Checkout => store customer info in session var then go to Stripe payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone")] Order order)
        {
            // auto-fill date, total & email
            order.OrderDate = DateTime.Now;
            order.CustomerId = User.Identity.Name;

            var cartItems = _context.CartItems.Where(c => c.CustomerId == HttpContext.Session.GetString("CustomerId"));
            order.OrderTotal = (from c in cartItems
                                select (c.Quantity * c.Price)).Sum();

            // validate inputs
            //if (!ModelState.IsValid)
            //{
            //    return View(order);
            //}

            // store order obj w/10 props in a session var
            HttpContext.Session.SetObject("Order", order);

            // load payment page w/Stripe
            return RedirectToAction("Payment");
        }

        // GET: /Store/Payment => invoke Stripe Payment & redirect based on response
        [Authorize]
        public IActionResult Payment()
        {
            // get stripe key from configuration
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            // get order from session var so we know the total of payment
            var order = HttpContext.Session.GetObject<Order>("Order");
            var orderTotal = order.OrderTotal;

            // set up payment options and call Stripe
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Store/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Store/Cart",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long?)(orderTotal * 100),
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions { Name = "Apparel+ Purchase" }
                        }
                    }
                }
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        // GET: /Store/SaveOrder => create order, clear cart, redirect to Confirmation
        [Authorize]
        public IActionResult SaveOrder()
        {
            // create order
            var order = HttpContext.Session.GetObject<Order>("Order");
            _context.Orders.Add(order);
            _context.SaveChanges();

            // copy cart items => new order details
            var cartItems = _context.CartItems.Where(c => c.CustomerId == GetCustomerId());
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    OrderId = order.OrderId
                };
                _context.OrderDetails.Add(orderDetail);
                _context.CartItems.Remove(item);  // remove item from cart
            }
            _context.SaveChanges();

            // empty cart
            HttpContext.Session.SetInt32("ItemCount", 0);

            // show order confirmation: /Orders/Details/72
            return RedirectToAction("Details", "Orders", new { @id = order.OrderId });
        }
    }
}
