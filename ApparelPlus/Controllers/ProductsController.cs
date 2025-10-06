using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ApparelPlus.Controllers
{
    [Authorize(Roles = "Administrator")]  // restrict all methods to Admininstrator user role only
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
            // get product list from db.  join to parent object so we can display Category Names too
            var products = _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToList();

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
        [ValidateAntiForgeryToken] // check hidden token to prevent Form POSTS from other domains
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

        // GET: /Products/Edit/34 => display Product in form
        public IActionResult Edit(int id)
        {
            // search for product
            var product = _context.Products.Find(id);

            // if not found => 404
            if (product == null)
            {
                return NotFound();
            }

            // fetch Categories for parent dropdown list
            // ViewBag: simple object shared between controller and view, similar to ViewData
            ViewBag.CategoryId = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");

            // show product in view
            return View(product);
        }

        // POST: /Products/Edit/34 => save new Product to db
        [HttpPost]
        [ValidateAntiForgeryToken] // check hidden token to prevent Form POSTS from other domains
        public IActionResult Edit(int id, [Bind("ProductId,Name,Size,Price,Description,Image,CategoryId")] Product product)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // save to db
            _context.Products.Update(product);
            _context.SaveChanges();

            // redirect
            return RedirectToAction("Index");
        }

        // GET: /Products/Delete/27 => remove Selected Product
        public IActionResult Delete(int id)
        {
            // search for product
            var product = _context.Products.Find(id);

            // if not found => 404
            if (product == null)
            {
                return NotFound();
            }

            // delete & redirect
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
