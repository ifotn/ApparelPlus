using ApparelPlus.Controllers;
using ApparelPlus.Data;
using ApparelPlus.Models;
using Microsoft.AspNetCore.Mvc;
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
    public void IndexReturnsIndexView()
    {
        // no arrange needed here; happens automatically in TestInitialize

        // act
        var result = (ViewResult)controller.Index();

        // assert
        Assert.AreEqual("Index", result.ViewName);
    }

    [TestMethod]
    public void IndexReturnsCategories()
    {
        // verify Index returns list of category data
        // no arrange needed

        // act
        var result = (ViewResult)controller.Index();
        List<Category> model = (List<Category>)result.Model;

        // assert => is data model in view equal to our Categories data in mock db?
        CollectionAssert.AreEqual(_context.Categories.ToList(), model);
    }

    [TestMethod]
    public void EditGetValidIdReturnsEditView()
    {
        // act
        var result = (ViewResult)controller.Edit(27);

        // assert
        Assert.AreEqual("Edit", result.ViewName);
    }

    [TestMethod]
    public void EditGetInvalidIdReturns404View()
    {
        // act
        var result = (ViewResult)controller.Edit(-1);

        // assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void EditGetValidIdReturnsCategory()
    {
        // act
        var result = (ViewResult)controller.Edit(27);
        var model = (Category)result.Model;

        // assert
        Assert.AreEqual(_context.Categories.Find(27), model);
    }
}
