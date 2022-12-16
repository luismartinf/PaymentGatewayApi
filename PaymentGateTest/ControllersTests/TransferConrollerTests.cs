using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.Controllers;
using PaymentGatewayApi.PaymentModels;
using System.Data.Entity.Infrastructure;
using System.Security.Claims;

namespace PaymentGatewayApiTests.ControllersTests
{
    public class TransferConrollerTests
    {
        private IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");

        [TestMethod]
        public void GetTransfers_numberofallobjects()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            TransfersController fakecontroller = A.Fake<TransfersController>(x => x.WithArgumentsForConstructor(() => new TransfersController(mockRepository)));
            var dummies = (List<Transfers>)A.CollectionOfDummy<Transfers>(Elemntsindb());
            A.CallTo(() => fakecontroller.GetTranfers()).Returns(dummies);

            var tokencontroller = new UsersController(null, _context);
            var accesstoken = tokencontroller.CreateToken(new UserLogins { UserName = "raulmartinez", Password = "Ternera4628%" }).Value!;
            var claims = accesstoken.GetClaims(accesstoken.GuidId);
            ClaimsIdentity identity = new(); identity.AddClaims(claims);
            DefaultHttpContext httpContext = new(); httpContext.User.AddIdentity(identity);

            // Act
            var controller = new TransfersController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetTranfers();
            var content = actionResult.Value as IList<Transfers>;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Transfers>>));
            Assert.AreEqual(dummies.Count, content?.Count);
        }

        [TestMethod]
        public void GetTransfers_numberofuserobjects()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            TransfersController fakecontroller = A.Fake<TransfersController>(x => x.WithArgumentsForConstructor(() => new TransfersController(mockRepository)));
            var dummies = (List<Transfers>)A.CollectionOfDummy<Transfers>(_context.Transfers.Where(c => c.UserTransfers.UserId == 5).Count());
            A.CallTo(() => fakecontroller.GetTranfers()).Returns(dummies);

            var tokencontroller = new UsersController(null, _context);
            var accesstoken = tokencontroller.CreateToken(new UserLogins { UserName = "enriquericarte", Password = "Ligurdas453$" }).Value!;
            var claims = accesstoken.GetClaims(accesstoken.GuidId);
            ClaimsIdentity identity = new();
            identity.AddClaims(claims);
            var httpContext = new DefaultHttpContext();
            httpContext.User.AddIdentity(identity);

            // Act
            var controller = new TransfersController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetTranfers();
            var content = actionResult.Value as IList<Transfers>;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Transfers>>));
            Assert.AreEqual(dummies.Count, content?.Count);

        }

        [TestMethod]
        public void GetTransfers_nocontent()
        {
            // Arrange
            ClaimsIdentity identity = new();
            var httpContext = new DefaultHttpContext();
            httpContext.User.AddIdentity(identity);

            // Act
            var controller = new TransfersController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetTranfers();

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Transfers>>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(NoContentResult));
        }


        [TestMethod]
        public void GetPaymethobyid_ReturnsTransfersSameId()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transfers.Find(2)).Returns(new Transfers {TransferId=1, TransactionId = 2,});

            // Act
            var mockcontroller = new TransfersController(mockRepository);
            var controller = new TransfersController(null);
            var fakeTransfer = mockcontroller.GetTransferbyId(2).Value; ;
            var contentResult = controller.GetTransferbyId(2).Value;


            // Assert
            A.CallTo(() => mockRepository.Transfers.Find(2)).MustHaveHappened();
            Assert.AreEqual(fakeTransfer?.TransferId, contentResult?.TransferId);
            Assert.AreEqual(fakeTransfer?.TransactionId, contentResult?.TransactionId);
        }

        [TestMethod]
        public void GetTransferbyid_ReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transfers.Find(Elemntsindb() + 5))!.Returns(null);

            // Act
            var controller = new TransfersController(mockRepository);
            var actionResult = controller.GetTransferbyId(Elemntsindb() + 5).Result;


            // Assert
            A.CallTo(() => mockRepository.Transfers.Find(Elemntsindb() + 5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));

        }


        [TestMethod]
        public void PostMethod_okResult()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transfers.Add(A<Transfers>.Ignored)).Returns(new Transfers { TransferId = Elemntsindb(), });
            var faketrans = A.Fake<Transfers>();

            // Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.PostTransfer(faketrans);

            // Assert
            A.CallToSet(() => faketrans.UserTransfers).MustHaveHappened();
            A.CallToSet(() => faketrans.TransTransfers).MustHaveHappened();
            A.CallToSet(() => faketrans.TransferDate).MustHaveHappened();
            A.CallToSet(() => faketrans.TransTransfers.UserTrans).MustHaveHappened();
            A.CallToSet(() => faketrans.TransTransfers.PaymethodTrans).MustHaveHappened();
            A.CallTo(() => mockRepository.Transfers.Add(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Transfers>));

        }


        [TestMethod]
        public void PostMethodReturn_NotImplementedModifiedstated()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).Throws(new NotImplementedException());

            // Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.PostTransfer(A.Fake<Transfers>());


            // Assert
            A.CallTo(() => mockRepository.Transfers.Add(A<Transfers>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.MarkAsModified(A.Fake<Transfers>()));
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Transfers>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(JsonResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();

        }


        [TestMethod]
        public void PostMethodReturnNotModifiedstatedForeignKey()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new InvalidOperationException());

            // Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.PostTransfer(A.Fake<Transfers>());


            // Assert
            A.CallTo(() => mockRepository.Transfers.Add(A<Transfers>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.SaveChanges());
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Transfers>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(JsonResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();

        }


        [TestMethod]
        public void PutReturnsContentResult()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var mockTransfer = A.Fake<Transfers>();
            A.CallTo(() => mockTransfer.TransferId).Returns(5);
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored));
            A.CallTo(() => mockRepository.SaveChanges());


            //Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.Puttransfer(5, mockTransfer);

            //assert
            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransferExists(5)).MustNotHaveHappened();
        }


        [TestMethod]
        public void PutReturnsBadRequest()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            Transfers mockTransfer = A.Fake<Transfers>();


            //Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.Puttransfer(1, mockTransfer);

            //assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
            Assert.AreNotEqual(1, mockTransfer.TransferId);
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }


        [TestMethod]
        public void PutReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Transfers mockTransfer = A.Fake<Transfers>();
            A.CallTo(() => mockRepository.TransferExists(5)).Returns(false);
            A.CallTo(() => mockTransfer.TransferId).Returns(5);


            //Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.Puttransfer(5, mockTransfer);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransferExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsFailMarkAsModified()
        {
            // Arrange
            Transfers mockTransfer = A.Fake<Transfers>();
            A.CallTo(() => mockTransfer.TransferId).Returns(5);
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.MarkAsModified(mockTransfer)).Throws(new NotImplementedException());


            //Act
            TransfersController controller = new(mockRepository);
            IActionResult actionResult = controller.Puttransfer(5, mockTransfer);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
            A.CallTo(() => mockRepository.TransferExists(A<int>.Ignored)).MustNotHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.MarkAsModified(mockTransfer));
        }


        [TestMethod]
        public void PutReturnsInvalidOperationForeignKey()
        {
            // Arrange
            Transfers mockTransfer = A.Fake<Transfers>();
            A.CallTo(() => mockTransfer.TransferId).Returns(5);
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new InvalidOperationException());


            //Act
            TransfersController controller = new(mockRepository);
            IActionResult actionResult = controller.Puttransfer(5, mockTransfer);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransferExists(A<int>.Ignored)).MustNotHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsJsonUnknow()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Transfers mockTransfer = A.Fake<Transfers>();
            A.CallTo(() => mockRepository.TransferExists(5)).Returns(true);
            A.CallTo(() => mockTransfer.TransferId).Returns(5);


            //Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.Puttransfer(5, mockTransfer);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransferExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsNotImplementedTransferexist()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Transfers mockTransfer = A.Fake<Transfers>();
            A.CallTo(() => mockRepository.TransferExists(5)).Throws(new NotImplementedException());
            A.CallTo(() => mockTransfer.TransferId).Returns(5);


            //Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.Puttransfer(5, mockTransfer);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransferExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.TransferExists(5));
        }


        [TestMethod]
        public void DeleteReturnsOk()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();

            // Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.DeleteTransf(2);

            // Assert
            A.CallTo(() => mockRepository.Transfers.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Transfers.Remove(A<Transfers>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsNotNull(actionResult);
        }


        [TestMethod]
        public void DeleteReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transfers.Find(Elemntsindb() + 5))!.Returns(null);

            // Act
            TransfersController controller = new(mockRepository);
            var actionResult = controller.DeleteTransf(Elemntsindb() + 5);

            // Assert

            A.CallTo(() => mockRepository.Transfers.Find(Elemntsindb() + 5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
            A.CallTo(() => mockRepository.Transfers.Remove(A<Transfers>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }

        [TestMethod]
        public void DeleteReturnsBadrequest()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transfers.Find(3)).Returns(A.Fake<Transfers>()); ;
            A.CallTo(() => mockRepository.Transfers.Remove(A<Transfers>.Ignored)).Throws(new InvalidOperationException());

            // Act
            TransfersController controller = new(mockRepository);
            ActionResult<Transfers> actionResult;
            try { actionResult = controller.DeleteTransf(3); }
            catch { actionResult = new BadRequestResult(); }

            // Assert
            A.CallTo(() => mockRepository.Transfers.Find(3)).MustHaveHappened();
            A.CallTo(() => mockRepository.Transfers.Remove(A<Transfers>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.Transfers.Remove(A<Transfers>.Ignored));
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestResult));
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }

        private int Elemntsindb()
        {
            var elementsdb = _context.Transfers.ToList().Count; ;
            return elementsdb;
        }
    }
}
