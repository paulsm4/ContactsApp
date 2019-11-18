using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContactsApp.Controllers;
using System.Web.Mvc;
using ContactsApp.Models;
using System.Threading.Tasks;

namespace ContactsApp.Tests.Controllers
{
    [TestClass]
    public class ContactsControllerTest
    {
        [TestMethod]
        public async Task TestDetailsView()
        {
            // Create controller
            ContactsController controller = new ContactsController();

            // Act
            ViewResult result = await controller.Details(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
