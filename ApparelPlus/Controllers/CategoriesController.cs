using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApparelPlus.Controllers
{
    [Authorize(Roles = "Administrator")] // restrict access to all methods to Administrator users only
    public class CategoriesController : Controller
    {
        // shared db connection for all methods
        private readonly ApplicationDbContext _context;

        // constructor that received db connection instance
        // db object is a dependency of this controller
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // create mock in-memory list of Categories (replaced next week with db)
            //var categories = new List<Category>();

            //for (int i = 1; i < 11; i++)
            //{
            //    categories.Add(new Category { CategoryId = i, Name = "Category " + i.ToString() });
            //}

            // use DbSet to fetch current category list
            var categories = _context.Categories.ToList();

            // load the view and pass the category list for display
            return View("Index", categories);
        }

        // GET: /Categories/Create => display empty Category form
        public IActionResult Create()
        {
            return View("Create");
        }

        // POST: /Categories/Create => validate & create new Category from form submission
        [HttpPost]
        [ValidateAntiForgeryToken] // check hidden token to prevent Form POSTS from other domains
        public IActionResult Create([Bind("Name")] Category category)
        {
            if (!ModelState.IsValid)
            {
                // form incomplete, show user the form again
                return View("Create");
            }

            // create new Category in db using DbSet object - this writes hidden sql we don't see
            _context.Categories.Add(category);
            _context.SaveChanges();

            // redirect to Index method to show updated Category list
            return RedirectToAction("Index");
        }

        // GET: /Categories/Edit/5 => show pre-filled Category form
        public IActionResult Edit(int id)
        {
            // check db for selected category
            var category = _context.Categories.Find(id);

            // error if id not in db
            if (category == null)
            {
                return View("404");
            }

            // load page and pass the Category object
            return View("Edit", category);
        }

        // POST: /Categories/Edit/5 => update selected Category
        [HttpPost]
        [ValidateAntiForgeryToken] // check hidden token to prevent Form POSTS from other domains
        public IActionResult Edit(int id, [Bind("CategoryId,Name")] Category category)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View("Edit");
            }

            // edit & save to db
            _context.Categories.Update(category);
            _context.SaveChanges();

            // refresh list
            return RedirectToAction("Index");
        }

        // GET: /Categories/Delete/5 => delete selected Category
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return View("404");
            }

            // remove from db
            _context.Categories.Remove(category);
            _context.SaveChanges();

            // redirect
            return RedirectToAction("Index");
        }
    }
}
