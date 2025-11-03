using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApparelPlus.Controllers
{
    public class StoreController : Controller
    {
        // shared db connection
        private readonly ApplicationDbContext _context;

        // constructor to instantiate db connection
        public StoreController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
