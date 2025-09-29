using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApparelPlus.Controllers
{
    public class ProductsController : Controller
    {
        // db connection
        private readonly ApplicationDbContext _context;

        // constructor using db connection
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // get product list from db
            var products = _context.Products.ToList();

            // show View and pass product list
            return View(products);
        }

        // GET: /Products/Create => show empty Product form
        public IActionResult Create()
        {
            // fetch Categories for parent Dropdown list
            // ViewBag: simple object shared between controller and view, similar to ViewData
            ViewBag.CategoryId = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");

            return View();
        }

        // POST: /Products/Create => save new Product to db
        [HttpPost]
        public IActionResult Create([Bind("Name,Size,Price,Description,Image,CategoryId")] Product product)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // save to db
            _context.Products.Add(product);
            _context.SaveChanges();

            // redirect
            return RedirectToAction("Index");
        }
    }
}
