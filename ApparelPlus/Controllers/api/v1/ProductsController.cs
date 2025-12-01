using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace ApparelPlus.Controllers.api.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        // GET: /api/v1/products
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Products will go here next");
        }

    }
}
