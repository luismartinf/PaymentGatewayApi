global using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGatewayApi.App_Data;
using FakeItEasy;
using PaymentGatewayApi.PaymentModels;
using PaymentGatewayApi.Controllers;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using NuGet.Common;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PaymentGatewayApiTests.ControllerTest
{
    [TestClass]
    public class PaymethodsControllerTests
    {
        private IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");

        [TestMethod]
        public void GetPaymethods_numberofallobjects()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            PaymethodsController fakecontroller = A.Fake<PaymethodsController>(x => x.WithArgumentsForConstructor(() => new PaymethodsController(mockRepository)));
            var dummies = (List<Paymethods>)A.CollectionOfDummy<Paymethods>(Elemntsindb());
            A.CallTo(() => fakecontroller.GetPaymethods()).Returns(dummies);

            var tokencontroller = new UsersController(null, _context);
            var accesstoken = tokencontroller.CreateToken(new UserLogins { UserName = "raulmartinez", Password = "Ternera4628%" }).Value!;
            var claims = accesstoken.GetClaims(accesstoken.GuidId);
            ClaimsIdentity identity = new(); identity.AddClaims(claims);
            DefaultHttpContext httpContext = new(); httpContext.User.AddIdentity(identity);

            // Act
            var controller = new PaymethodsController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetPaymethods();
            var content = actionResult.Value as IList<Paymethods>;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Paymethods>>));
            Assert.AreEqual(dummies.Count, content?.Count);
        }

        [TestMethod]
        public void GetPaymethods_numberofuserobjects()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            PaymethodsController fakecontroller = A.Fake<PaymethodsController>(x => x.WithArgumentsForConstructor(() => new PaymethodsController(mockRepository)));
            var dummies = (List<Paymethods>)A.CollectionOfDummy<Paymethods>(_context.Paymethods.Where(c => c.UserPaymethod.UserId == 5).Count());
            A.CallTo(() => fakecontroller.GetPaymethods()).Returns(dummies);

            var tokencontroller = new UsersController(null, _context);
            var accesstoken = tokencontroller.CreateToken(new UserLogins { UserName = "enriquericarte", Password = "Ligurdas453$" }).Value!;
            var claims = accesstoken.GetClaims(accesstoken.GuidId);
            ClaimsIdentity identity = new();
            identity.AddClaims(claims);
            var httpContext = new DefaultHttpContext();
            httpContext.User.AddIdentity(identity);

            // Act
            var controller = new PaymethodsController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetPaymethods();
            var content = actionResult.Value as IList<Paymethods>;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Paymethods>>));
            Assert.AreEqual(dummies.Count, content?.Count);

        }

        [TestMethod]
        public void GetPaymethods_nocontent()
        {
            // Arrange
            ClaimsIdentity identity = new();
            var httpContext = new DefaultHttpContext();
            httpContext.User.AddIdentity(identity);

            // Act
            var controller = new PaymethodsController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetPaymethods();

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Paymethods>>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(NoContentResult));
        }


        [TestMethod]
        public void GetPaymethobyid_ReturnsPaymethodsSameId()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Paymethods.Find(1)).Returns(new Paymethods { PaymethodId = 1, PaymentNum = 4257891824019283, TypePayment = "Card", });

            // Act
            var mockcontroller = new PaymethodsController(mockRepository);
            var controller = new PaymethodsController(null);
            var fakePaymethod = mockcontroller.GetPaymethodbyId(1).Value; ;
            var contentResult = controller.GetPaymethodbyId(1).Value;


            // Assert
            A.CallTo(() => mockRepository.Paymethods.Find(1)).MustHaveHappened();
            Assert.AreEqual(fakePaymethod?.PaymethodId, contentResult?.PaymethodId);
            Assert.AreEqual(fakePaymethod?.PaymentNum, contentResult?.PaymentNum);
        }

        [TestMethod]
        public void GetPaymethodbyid_ReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Paymethods.Find(Elemntsindb() + 5))!.Returns(null);

            // Act
            var controller = new PaymethodsController(mockRepository);
            var actionResult = controller.GetPaymethodbyId(Elemntsindb() + 5).Result;


            // Assert
            A.CallTo(() => mockRepository.Paymethods.Find(Elemntsindb() + 5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));

        }


        [TestMethod]
        public void PostMethod_okResult()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Paymethods.Add(A<Paymethods>.Ignored)).Returns(new Paymethods { PaymethodId = Elemntsindb(), });


            // Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PostPaymethod(A.Fake<Paymethods>());

            // Assert
            A.CallTo(() => mockRepository.Paymethods.Add(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Paymethods>));

        }


        [TestMethod]
        public void PostMethodReturn_NotImplementedModifiedstated()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).Throws(new NotImplementedException());

            // Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PostPaymethod(A.Fake<Paymethods>());


            // Assert
            A.CallTo(() => mockRepository.Paymethods.Add(A<Paymethods>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.MarkAsModified(A.Fake<Paymethods>()));
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Paymethods>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(JsonResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();

        }


        [TestMethod]
        public void PostMethodReturnNotModifiedstatedForeignKey()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new InvalidOperationException());

            // Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PostPaymethod(A.Fake<Paymethods>());


            // Assert
            A.CallTo(() => mockRepository.Paymethods.Add(A<Paymethods>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.SaveChanges());
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Paymethods>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(JsonResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();

        }


        [TestMethod]
        public void PutReturnsContentResult()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var mockpaymethod = A.Fake<Paymethods>();
            A.CallTo(() => mockpaymethod.PaymethodId).Returns(5);
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored));
            A.CallTo(() => mockRepository.SaveChanges());


            //Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PutPaymethod(5, mockpaymethod);

            //assert
            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.PaymethodExists(5)).MustNotHaveHappened();
        }


        [TestMethod]
        public void PutReturnsBadRequest()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            Paymethods mockPaymethod = A.Fake<Paymethods>();


            //Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PutPaymethod(1, mockPaymethod);

            //assert
            Assert.IsInstanceOfType(actionResult, typeof(Microsoft.AspNetCore.Mvc.BadRequestResult));
            Assert.AreNotEqual(1, mockPaymethod.PaymethodId);
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }


        [TestMethod]
        public void PutReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Paymethods mockpaymethod = A.Fake<Paymethods>();
            A.CallTo(() => mockRepository.PaymethodExists(5)).Returns(false);
            A.CallTo(() => mockpaymethod.PaymethodId).Returns(5);


            //Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PutPaymethod(5, mockpaymethod);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.PaymethodExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsFailMarkAsModified()
        {
            // Arrange
            Paymethods mockPaymethod = A.Fake<Paymethods>();
            A.CallTo(() => mockPaymethod.PaymethodId).Returns(5);
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.MarkAsModified(mockPaymethod)).Throws(new NotImplementedException());


            //Act
            PaymethodsController controller = new(mockRepository);
            IActionResult actionResult = controller.PutPaymethod(5, mockPaymethod);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
            A.CallTo(() => mockRepository.PaymethodExists(A<int>.Ignored)).MustNotHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.MarkAsModified(mockPaymethod));
        }


        [TestMethod]
        public void PutReturnsInvalidOperationForeignKey()
        {
            // Arrange
            Paymethods mockPaymethod = A.Fake<Paymethods>();
            A.CallTo(() => mockPaymethod.PaymethodId).Returns(5);
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new InvalidOperationException());


            //Act
            PaymethodsController controller = new(mockRepository);
            IActionResult actionResult = controller.PutPaymethod(5, mockPaymethod);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.PaymethodExists(A<int>.Ignored)).MustNotHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsJsonUnknow()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Paymethods mockpaymethod = A.Fake<Paymethods>();
            A.CallTo(() => mockRepository.PaymethodExists(5)).Returns(true);
            A.CallTo(() => mockpaymethod.PaymethodId).Returns(5);


            //Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PutPaymethod(5, mockpaymethod);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.PaymethodExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsNotImplementedPaymethodexist()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Paymethods mockpaymethod = A.Fake<Paymethods>();
            A.CallTo(() => mockRepository.PaymethodExists(5)).Throws(new NotImplementedException());
            A.CallTo(() => mockpaymethod.PaymethodId).Returns(5);


            //Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PutPaymethod(5, mockpaymethod);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.PaymethodExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.PaymethodExists(5));
        }


        [TestMethod]
        public void DeleteReturnsOk()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();

            // Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.DeletePaymethod(2);

            // Assert
            A.CallTo(() => mockRepository.Paymethods.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Paymethods.Remove(A<Paymethods>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsNotNull(actionResult);
        }


        [TestMethod]
        public void DeleteReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Paymethods.Find(Elemntsindb() + 5))!.Returns(null);

            // Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.DeletePaymethod(Elemntsindb() + 5);

            // Assert

            A.CallTo(() => mockRepository.Paymethods.Find(Elemntsindb() + 5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult.Result, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));
            A.CallTo(() => mockRepository.Paymethods.Remove(A<Paymethods>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }

        [TestMethod]
        public void DeleteReturnsBadrequest()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Paymethods.Find(3)).Returns(A.Fake<Paymethods>()); ;
            A.CallTo(() => mockRepository.Paymethods.Remove(A<Paymethods>.Ignored)).Throws(new InvalidOperationException());

            // Act
            PaymethodsController controller = new(mockRepository);
            ActionResult<Paymethods> actionResult;
            try { actionResult = controller.DeletePaymethod(3); }
            catch { actionResult = new Microsoft.AspNetCore.Mvc.BadRequestResult(); }

            // Assert
            A.CallTo(() => mockRepository.Paymethods.Find(3)).MustHaveHappened();
            A.CallTo(() => mockRepository.Paymethods.Remove(A<Paymethods>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.Paymethods.Remove(A<Paymethods>.Ignored));
            Assert.IsInstanceOfType(actionResult.Result, typeof(Microsoft.AspNetCore.Mvc.BadRequestResult));
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }

        private int Elemntsindb()
        {
            var elementsdb = _context.Paymethods.ToList().Count; ;
            return elementsdb;
        }
    }
}