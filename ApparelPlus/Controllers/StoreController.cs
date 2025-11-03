using ApparelPlus.Data;
using Microsoft.AspNetCore.Mvc;

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
    }
}
