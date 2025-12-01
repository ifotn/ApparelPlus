using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace ApparelPlus.Controllers.api.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // shared db conn
        private readonly ApplicationDbContext _context;

        // constructor w/db dependency
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/v1/products => fetch all products
        [HttpGet]
        public IActionResult Index()
        {
            // fetch products from db
            var products = _context.Products.ToList();

            // return product data as json
            return Ok(products);
        }

        // GET: /api/v1/products/22 => fetch selected product
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: /api/v1/products => create new product
        [HttpPost]
        public IActionResult Create(Product product)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return BadRequest(); // 400 status
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            // return 201 resource created
            return CreatedAtAction("Create", product);
        }
    }
}
