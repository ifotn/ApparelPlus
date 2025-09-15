using Microsoft.AspNetCore.Mvc;

namespace ApparelPlus.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
