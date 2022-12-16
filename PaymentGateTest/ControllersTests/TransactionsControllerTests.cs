using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.Controllers;
using PaymentGatewayApi.PaymentModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayApiTests.ControllersTests
{
    [TestClass]
    public class TransactionsControllerTests
    {
        private IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");

        [TestMethod]
        public void GetTransactions_numberofallobjects()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            TransactionsController fakecontroller = A.Fake<TransactionsController>(x => x.WithArgumentsForConstructor(() => new TransactionsController(mockRepository)));
            var dummies = (List<Transactions>)A.CollectionOfDummy<Transactions>(Elemntsindb());
            A.CallTo(() => fakecontroller.GetTransactions()).Returns(dummies);

            var tokencontroller = new UsersController(null, _context);
            var accesstoken = tokencontroller.CreateToken(new UserLogins { UserName = "raulmartinez", Password = "Ternera4628%" }).Value!;
            var claims = accesstoken.GetClaims(accesstoken.GuidId);
            ClaimsIdentity identity = new(); identity.AddClaims(claims);
            DefaultHttpContext httpContext = new(); httpContext.User.AddIdentity(identity);

            // Act
            var controller = new TransactionsController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetTransactions();
            var content = actionResult.Value as IList<Transactions>;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Transactions>>));
            Assert.AreEqual(dummies.Count, content?.Count);
        }

        [TestMethod]
        public void GetTransactions_numberofuserobjects()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            TransactionsController fakecontroller = A.Fake<TransactionsController>(x => x.WithArgumentsForConstructor(() => new TransactionsController(mockRepository)));
            var dummies = (List<Transactions>)A.CollectionOfDummy<Transactions>(_context.Transactions.Where(c => c.UserTrans.UserId == 5).Count());
            A.CallTo(() => fakecontroller.GetTransactions()).Returns(dummies);

            var tokencontroller = new UsersController(null, _context);
            var accesstoken = tokencontroller.CreateToken(new UserLogins { UserName = "enriquericarte", Password = "Ligurdas453$" }).Value!;
            var claims = accesstoken.GetClaims(accesstoken.GuidId);
            ClaimsIdentity identity = new();
            identity.AddClaims(claims);
            var httpContext = new DefaultHttpContext();
            httpContext.User.AddIdentity(identity);

            // Act
            var controller = new TransactionsController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetTransactions();
            var content = actionResult.Value as IList<Transactions>;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Transactions>>));
            Assert.AreEqual(dummies.Count, content?.Count);

        }

        [TestMethod]
        public void GetTransactions_nocontent()
        {
            // Arrange
            ClaimsIdentity identity = new();
            var httpContext = new DefaultHttpContext();
            httpContext.User.AddIdentity(identity);

            // Act
            var controller = new TransactionsController(null) { ControllerContext = new ControllerContext() { HttpContext = httpContext, } };
            var actionResult = controller.GetTransactions();

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Transactions>>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(NoContentResult));
        }


        [TestMethod]
        public void GetPaymethobyid_ReturnsTransactionsSameId()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transactions.Find(2)).Returns(new Transactions { TransactionId = 2, Item = "ABFD56479032" });

            // Act
            var mockcontroller = new TransactionsController(mockRepository);
            var controller = new TransactionsController(null);
            var fakeTransaction = mockcontroller.GetTransactionbyId(2).Value; ;
            var contentResult = controller.GetTransactionbyId(2).Value;


            // Assert
            A.CallTo(() => mockRepository.Transactions.Find(2)).MustHaveHappened();
            Assert.AreEqual(fakeTransaction?.TransactionId, contentResult?.TransactionId);
            Assert.AreEqual(fakeTransaction?.Item, contentResult?.Item);
        }

        [TestMethod]
        public void GetTransactionbyid_ReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transactions.Find(Elemntsindb() + 5))!.Returns(null);

            // Act
            var controller = new TransactionsController(mockRepository);
            var actionResult = controller.GetTransactionbyId(Elemntsindb() + 5).Result;


            // Assert
            A.CallTo(() => mockRepository.Transactions.Find(Elemntsindb() + 5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));

        }


        [TestMethod]
        public void PostMethod_okResult()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transactions.Add(A<Transactions>.Ignored)).Returns(new Transactions { TransactionId = Elemntsindb(), });
            var faketrans = A.Fake<Transactions>();

            // Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.PostTransactions(faketrans);

            // Assert
            A.CallToSet(() => faketrans.UserTrans).MustHaveHappened();
            A.CallToSet(() => faketrans.BeginTransaction).MustHaveHappened();
            A.CallToSet(() => faketrans.PaymethodTrans!.UserPaymethod).MustHaveHappened();
            A.CallTo(() => mockRepository.Transactions.Add(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Transactions>));

        }


        [TestMethod]
        public void PostMethodReturn_NotImplementedModifiedstated()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).Throws(new NotImplementedException());

            // Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.PostTransactions(A.Fake<Transactions>());


            // Assert
            A.CallTo(() => mockRepository.Transactions.Add(A<Transactions>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.MarkAsModified(A.Fake<Transactions>()));
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Transactions>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(JsonResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();

        }


        [TestMethod]
        public void PostMethodReturnNotModifiedstatedForeignKey()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new InvalidOperationException());

            // Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.PostTransactions(A.Fake<Transactions>());


            // Assert
            A.CallTo(() => mockRepository.Transactions.Add(A<Transactions>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.SaveChanges());
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Transactions>));
            Assert.IsInstanceOfType(actionResult.Result, typeof(JsonResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();

        }


        [TestMethod]
        public void PutReturnsContentResult()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var mockTransaction = A.Fake<Transactions>();
            A.CallTo(() => mockTransaction.TransactionId).Returns(5);
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored));
            A.CallTo(() => mockRepository.SaveChanges());


            //Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.Puttransactions(5, mockTransaction);

            //assert
            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransactionExists(5)).MustNotHaveHappened();
        }


        [TestMethod]
        public void PutReturnsBadRequest()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            Transactions mockTransaction = A.Fake<Transactions>();


            //Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.Puttransactions(1, mockTransaction);

            //assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
            Assert.AreNotEqual(1, mockTransaction.TransactionId);
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }


        [TestMethod]
        public void PutReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Transactions mockTransaction = A.Fake<Transactions>();
            A.CallTo(() => mockRepository.TransactionExists(5)).Returns(false);
            A.CallTo(() => mockTransaction.TransactionId).Returns(5);


            //Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.Puttransactions(5, mockTransaction);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransactionExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsFailMarkAsModified()
        {
            // Arrange
            Transactions mockTransaction = A.Fake<Transactions>();
            A.CallTo(() => mockTransaction.TransactionId).Returns(5);
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.MarkAsModified(mockTransaction)).Throws(new NotImplementedException());


            //Act
            TransactionsController controller = new(mockRepository);
            IActionResult actionResult = controller.Puttransactions(5, mockTransaction);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
            A.CallTo(() => mockRepository.TransactionExists(A<int>.Ignored)).MustNotHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.MarkAsModified(mockTransaction));
        }


        [TestMethod]
        public void PutReturnsInvalidOperationForeignKey()
        {
            // Arrange
            Transactions mockTransaction = A.Fake<Transactions>();
            A.CallTo(() => mockTransaction.TransactionId).Returns(5);
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new InvalidOperationException());


            //Act
            TransactionsController controller = new(mockRepository);
            IActionResult actionResult = controller.Puttransactions(5, mockTransaction);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransactionExists(A<int>.Ignored)).MustNotHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsJsonUnknow()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Transactions mockTransaction = A.Fake<Transactions>();
            A.CallTo(() => mockRepository.TransactionExists(5)).Returns(true);
            A.CallTo(() => mockTransaction.TransactionId).Returns(5);


            //Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.Puttransactions(5, mockTransaction);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransactionExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
        }


        [TestMethod]
        public void PutReturnsNotImplementedTransactionexist()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
            Transactions mockTransaction = A.Fake<Transactions>();
            A.CallTo(() => mockRepository.TransactionExists(5)).Throws(new NotImplementedException());
            A.CallTo(() => mockTransaction.TransactionId).Returns(5);


            //Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.Puttransactions(5, mockTransaction);

            //assert
            A.CallTo(() => mockRepository.MarkAsModified(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.TransactionExists(5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult));
            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.TransactionExists(5));
        }


        [TestMethod]
        public void DeleteReturnsOk()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();

            // Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.DeleteTrans(2);

            // Assert
            A.CallTo(() => mockRepository.Transactions.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Transactions.Remove(A<Transactions>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsNotNull(actionResult);
        }


        [TestMethod]
        public void DeleteReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transactions.Find(Elemntsindb() + 5))!.Returns(null);

            // Act
            TransactionsController controller = new(mockRepository);
            var actionResult = controller.DeleteTrans(Elemntsindb() + 5);

            // Assert

            A.CallTo(() => mockRepository.Transactions.Find(Elemntsindb() + 5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
            A.CallTo(() => mockRepository.Transactions.Remove(A<Transactions>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }

        [TestMethod]
        public void DeleteReturnsBadrequest()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Transactions.Find(3)).Returns(A.Fake<Transactions>()); ;
            A.CallTo(() => mockRepository.Transactions.Remove(A<Transactions>.Ignored)).Throws(new InvalidOperationException());

            // Act
            TransactionsController controller = new(mockRepository);
            ActionResult<Transactions> actionResult;
            try { actionResult = controller.DeleteTrans(3); }
            catch { actionResult = new BadRequestResult(); }

            // Assert
            A.CallTo(() => mockRepository.Transactions.Find(3)).MustHaveHappened();
            A.CallTo(() => mockRepository.Transactions.Remove(A<Transactions>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.Transactions.Remove(A<Transactions>.Ignored));
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestResult));
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }

        private int Elemntsindb()
        {
            var elementsdb = _context.Transactions.ToList().Count; ;
            return elementsdb;
        }
    }
}

