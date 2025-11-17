using ApparelPlus.Controllers;
using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.EntityFrameworkCore;

namespace ApparelPlusTests;

[TestClass]
public class CategoriesControllerTests
{
    // shared mock db
    private ApplicationDbContext _context;
    List<Category> Categories = new List<Category>();
    CategoriesController controller;

    [TestInitialize] // this runs automatically before each test
    public void TestInitialize()
    {
        // create new in-memory mock db
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        // populate mock category data
        var category = new Category { CategoryId = 27, Name = "Random One" };
        _context.Categories.Add(category);

        category = new Category { CategoryId = 39, Name = "A Good Category" };
        _context.Categories.Add(category);

        category = new Category { CategoryId = 89, Name = "The Best Category" };
        _context.Categories.Add(category);
        _context.SaveChanges();

        // instantiate CategoriesController for unit tests
        controller = new CategoriesController(_context);
    }

    [TestMethod]
    public void TestMethod1()
    {
    }
}
