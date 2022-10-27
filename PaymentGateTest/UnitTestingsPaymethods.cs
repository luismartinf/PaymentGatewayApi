global using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGatewayApi.App_Data;
using FakeItEasy;
using PaymentGatewayApi.PaymentModels;
using PaymentGatewayApi.Controllers;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace PaymentGateTest
{
//    [TestClass]
//    public class UnitTestingsPaymethods
//    {
//        private IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");

//        [TestMethod]
//        public void GetPaymethods_PaymethodsToList_returns_numberofobjects()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            BookModelController fakecontroller = A.Fake<BookModelController>(x => x.WithArgumentsForConstructor(() => new BookModelController(mockRepository)));
//            var dummies = (List<BookModel>)A.CollectionOfDummy<BookModel>(Elemntsindb() - 1);
//            A.CallTo(() => fakecontroller.GetBooks()).Returns(dummies);

//            // Act
//            var controller = new BookModelController(null);
//            var actionResult = controller.GetBooks();
//            var content = (IList<BookModel>)actionResult.Value;

//            // Assert
//            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<BookModel>>));
//            Assert.AreEqual(dummies.Count, content.Count);
//        }

//        [TestMethod]
//        public void GetBookModel_ReturnsBookWithSameId()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.Books.Find(2)).Returns(new BookModel { BookId = 2, Name = "El entre nosotras", Author = "Karls Meyer", Onlend = true, LendeeId = 1 });

//            // Act
//            var mockcontroller = new BookModelController(mockRepository);
//            var controller = new BookModelController(null);
//            var fakeBook = mockcontroller.GetBookModel(2).Value; ;
//            var contentResult = controller.GetBookModel(2).Value;


//            // Assert
//            A.CallTo(() => mockRepository.Books.Find(2)).MustHaveHappened();
//            Assert.AreEqual(fakeBook.BookId, contentResult.BookId);
//            Assert.AreEqual(fakeBook.Name, contentResult.Name);
//        }

//        [TestMethod]
//        public void GetBookModel_ReturnsNotFound()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.Books.Find(Elemntsindb() + 5)).Returns(null);

//            // Act
//            var controller = new BookModelController(mockRepository);
//            var actionResult = controller.GetBookModel(Elemntsindb() + 5).Result;


//            // Assert
//            A.CallTo(() => mockRepository.Books.Find(Elemntsindb() + 5)).MustHaveHappened();
//            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));

//        }


//        [TestMethod]
//        public void DeleteReturnsOk()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();

//            // Act
//            BookModelController controller = new(mockRepository);
//            var actionResult = controller.DeleteBookModel(2);

//            // Assert
//            A.CallTo(() => mockRepository.Books.Find(2)).MustHaveHappened();
//            A.CallTo(() => mockRepository.Books.Remove(A<BookModel>.Ignored)).MustHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
//            Assert.IsNotNull(actionResult);
//        }


//        [TestMethod]
//        public void DeleteReturnsNotFound()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.Books.Find(Elemntsindb() + 5)).Returns(null);

//            // Act
//            BookModelController controller = new(mockRepository);
//            var actionResult = controller.DeleteBookModel(Elemntsindb() + 5);

//            // Assert

//            A.CallTo(() => mockRepository.Books.Find(Elemntsindb() + 5)).MustHaveHappened();
//            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
//            A.CallTo(() => mockRepository.Books.Remove(A<BookModel>.Ignored)).MustNotHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
//        }

//        [TestMethod]
//        public void DeleteReturnsBadrequest()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.Books.Remove(A<BookModel>.Ignored)).Throws(new InvalidOperationException());

//            // Act
//            BookModelController controller = new(mockRepository);
//            ActionResult<BookModel> actionResult;
//            try
//            {
//                actionResult = controller.DeleteBookModel(3);
//            }
//            catch
//            {
//                actionResult = new BadRequestResult();
//            }

//            // Assert
//            A.CallTo(() => mockRepository.Books.Find(3)).MustHaveHappened();
//            A.CallTo(() => mockRepository.Books.Remove(A<BookModel>.Ignored)).MustHaveHappened();
//            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestResult));
//            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.Books.Remove(A<BookModel>.Ignored));
//            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
//        }

//        [TestMethod]
//        public void PostMethodSetsLocationHeader()
//        {

//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.Books.Add(A<BookModel>.Ignored)).Returns(new BookModel { BookId = Elemntsindb(), Name = "El entre nosotras", Author = "Mark Trudeu", Onlend = true, LendeeId = 1 });


//            // Act
//            BookModelController controller = new(mockRepository);
//            var actionResult = controller.PostBookModel(A.Fake<BookModel>());

//            // Assert
//            A.CallTo(() => mockRepository.Books.Add(A<BookModel>.Ignored)).MustHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<BookModel>));

//        }


//        [TestMethod]
//        public void PostMethodReturnBadRequest()
//        {

//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.Books.Add(A<BookModel>.Ignored)).Throws(new InvalidOperationException());

//            // Act
//            BookModelController controller = new(mockRepository);
//            ActionResult<BookModel> actionResult;
//            try
//            {
//                actionResult = controller.PostBookModel(A.Fake<BookModel>());
//            }
//            catch (InvalidOperationException)
//            {
//                actionResult = new BadRequestResult();
//            }

//            // Assert
//            A.CallTo(() => mockRepository.Books.Add(A<BookModel>.Ignored)).MustHaveHappened();
//            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.Books.Add(A<BookModel>.Ignored));
//            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestResult));
//            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();

//        }

//        [TestMethod]
//        public void PutReturnsContentResult()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            BookModel mockBook = A.Fake<BookModel>();
//            mockBook.BookId = 5;
//            A.CallTo(() => mockRepository.MarkAsModified(A<BookModel>.Ignored));
//            A.CallTo(() => mockRepository.SaveChanges());


//            //Act
//            BookModelController controller = new(mockRepository);
//            var actionResult = controller.PutBookModel(5, mockBook);

//            //assert
//            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
//            A.CallTo(() => mockRepository.MarkAsModified(A<BookModel>.Ignored)).MustHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
//            A.CallTo(() => mockBook.BookModelExists(5)).MustNotHaveHappened();
//        }


//        [TestMethod]
//        public void PutReturnsBadRequest()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            BookModel mockBook = A.Fake<BookModel>();


//            //Act
//            BookModelController controller = new(mockRepository);
//            var actionResult = controller.PutBookModel(1, mockBook);

//            //assert
//            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
//            //Another possible library is Shouldly
//            Assert.AreNotEqual(1, mockBook.BookId);
//            A.CallTo(() => mockRepository.MarkAsModified(A<BookModel>.Ignored)).MustNotHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
//        }

//        [TestMethod]
//        public void PutReturnsNotFound()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
//            BookModel mockBook = A.Fake<BookModel>();
//            A.CallTo(() => mockBook.BookModelExists(5)).Returns(false);
//            mockBook.BookId = 5;

//            //Act
//            BookModelController controller = new(mockRepository);
//            var actionResult = controller.PutBookModel(5, mockBook);

//            //assert
//            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
//            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
//            A.CallTo(() => mockRepository.MarkAsModified(A<BookModel>.Ignored)).MustHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
//            A.CallTo(() => mockBook.BookModelExists(5)).MustHaveHappened();
//        }

//        [TestMethod]
//        public void PutReturnsFailMarkAsModified()
//        {
//            // Arrange
//            BookModel mockBook = A.Fake<BookModel>();
//            mockBook.BookId = 5;
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.MarkAsModified(mockBook)).Throws(new NotImplementedException());


//            //Act
//            BookModelController controller = new(mockRepository);
//            IActionResult actionResult;
//            try
//            {
//                actionResult = controller.PutBookModel(5, mockBook);
//            }
//            catch (NotImplementedException)
//            {
//                actionResult = new NoContentResult();
//            }

//            //assert
//            Assert.ThrowsException<NotImplementedException>(() => mockRepository.MarkAsModified(mockBook));
//            A.CallTo(() => mockRepository.MarkAsModified(A<BookModel>.Ignored)).MustHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
//            A.CallTo(() => mockBook.BookModelExists(A<int>.Ignored)).MustNotHaveHappened();
//        }

//        [TestMethod]
//        public void PutReturnsNoContent()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbEntityValidationException());
//            BookModel mockBook = A.Fake<BookModel>();
//            mockBook.BookId = 5;

//            //Act
//            BookModelController controller = new(mockRepository);
//            IActionResult actionResult;
//            try
//            {
//                actionResult = controller.PutBookModel(5, mockBook);
//            }
//            catch (DbEntityValidationException)
//            {
//                actionResult = new NoContentResult();
//            }

//            //assert
//            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
//            Assert.ThrowsException<DbEntityValidationException>(() => mockRepository.SaveChanges());
//            A.CallTo(() => mockRepository.MarkAsModified(A<BookModel>.Ignored)).MustHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
//            A.CallTo(() => mockBook.BookModelExists(A<int>.Ignored)).MustNotHaveHappened();
//        }

//        [TestMethod]
//        public void PutMethodNotcatchingConcurrencyException()
//        {
//            // Arrange
//            ILibraryAppContext mockRepository = A.Fake<ILibraryAppContext>();
//            A.CallTo(() => mockRepository.SaveChanges()).Throws(new DbUpdateConcurrencyException());
//            BookModel mockBook = A.Fake<BookModel>();
//            mockBook.BookId = 5;
//            A.CallTo(() => mockBook.BookModelExists(5)).Returns(true);

//            // Act
//            BookModelController controller = new(mockRepository);
//            IActionResult actionResult;
//            try
//            {
//                actionResult = controller.PutBookModel(5, mockBook);
//            }
//            catch
//            {
//                actionResult = new JsonResult(new { Message = "Unknowerror" }) { StatusCode = 500 };
//            }

//            // Assert
//            A.CallTo(() => mockRepository.MarkAsModified(A<BookModel>.Ignored)).MustHaveHappened();
//            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
//            A.CallTo(() => mockBook.BookModelExists(A<int>.Ignored)).MustHaveHappened();
//            Assert.ThrowsException<DbUpdateConcurrencyException>(() => mockRepository.SaveChanges());
//            actionResult.ShouldNotBeOfType(typeof(NotFoundResult));
//        }

//        private int Elemntsindb()
//        {
//            var elementsdb = _context.Books.ToList().Count + 1; ;
//            return elementsdb;
//        }
//    }
}