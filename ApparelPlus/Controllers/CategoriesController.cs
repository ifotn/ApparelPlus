using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApparelPlus.Controllers
{
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
            return View(categories);
        }

        // GET: /Categories/Create => display empty Category form
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create => validate & create new Category from form submission
        [HttpPost]
        public IActionResult Create([Bind("Name")] Category category)
        {
            if (!ModelState.IsValid)
            {
                // form incomplete, show user the form again
                return View();
            }

            // create new Category in db using DbSet object - this writes hidden sql we don't see
            _context.Categories.Add(category);
            _context.SaveChanges();

            // redirect to Index method to show updated Category list
            return RedirectToAction("Index");
        }
    }
}
