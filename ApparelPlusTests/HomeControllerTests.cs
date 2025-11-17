using ApparelPlus.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ApparelPlusTests
{
    [TestClass]
    public sealed class HomeControllerTests
    {
        [TestMethod]
        public void IndexReturnsView()
        {
            // check HomeController Index method returns a view called "Index"
            // arrange => set up anything we need to test this scenario
            HomeController controller = new HomeController();

            // act => call the method we want to test
            var result = (ViewResult)controller.Index();

            // assert => did we get the expected result?
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void PrivacyReturnsView()
        {
            // check HomeController Privacy method returns a view called "Privacy"
            // arrange => set up anything we need to test this scenario
            HomeController controller = new HomeController();

            // act => call the method we want to test
            var result = (ViewResult)controller.Privacy();

            // assert => did we get the expected result?
            Assert.AreEqual("Privacy", result.ViewName);
        }
    }
}
