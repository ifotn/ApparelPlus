using ApparelPlus.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApparelPlus.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            // create mock in-memory list of Categories (replaced next week with db)
            var categories = new List<Category>();

            for (int i = 1; i < 11; i++)
            {
                categories.Add(new Category { CategoryId = i, Name = "Category " + i.ToString() });
            }

            // load the view and pass the category list for display
            return View(categories);
        }
    }
}
