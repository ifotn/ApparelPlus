using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace ApparelPlus.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        // db conn
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Orders => show order history list newest first
        public IActionResult Index()
        {
            var orders = new List<Order>();

            if (User.IsInRole("Administrator"))
            {
                orders = _context.Orders.OrderByDescending(o => o.OrderDate).ToList();
            }
            else
            {
                // only show the customer their own ordres
                orders = _context.Orders
                    .Where(o => o.CustomerId == User.Identity.Name)
                    .OrderByDescending(o => o.OrderDate).ToList();
            }

            return View(orders);
        }

        // GET: /Orders/Details/72 => show Order Confirmation w/all Details
        public IActionResult Details(int id)
        {
            // fetch order from db
            var order = _context.Orders.Find(id);

            if (order == null)
            {
                return NotFound();
            }

            // restrict to Administrator and Customer who made this order
            if (User.IsInRole("Administrator") || (order.CustomerId == User.Identity.Name))
            {
                return View(order);
            }

            return Unauthorized();
        }
    }
}
