global using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGatewayApi.App_Data;
using FakeItEasy;
using PaymentGatewayApi.PaymentModels;
using PaymentGatewayApi.Controllers;
using System.Web.Http.Results;

namespace PaymentGateTest
{
    [TestClass]
    public class UnitTestingsPaymethods
    {
        [TestMethod]
        public void GetLendeeModel_ReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = new PaymentGatewayContext("fakeconnection");
            //A.CallTo(() => mockRepository.Lendees.Find(elemntsindb() + 5)).Returns(null);

            // Act
            var controller = new PaymethodsController(mockRepository);
            var actionResult = controller.GetPaymethodbyId(3).Result;


            // Assert
            A.CallTo(() => mockRepository.Paymethods.Find(3)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));

        }
        
        [TestMethod]
        public void Post_ReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = new PaymentGatewayContext("fakeconnection");
            var mockPaymethod = A.Fake<Paymethods>();
            //A.CallTo(() => mockRepository.Lendees.Find(elemntsindb() + 5)).Returns(null);

            // Act
            var controller = new PaymethodsController(mockRepository);
            var actionResult = controller.PostPaymethod(mockPaymethod).Result;


            // Assert
            //A.CallTo(() => mockRepository.Paymethods.Find(3)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));

        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            IPaymentGateContext mockRepository = new PaymentGatewayContext("fakeconnection");
            Paymethods mockPayment = A.Fake<Paymethods>();


            //Act
            PaymethodsController controller = new(mockRepository);
            var actionResult = controller.PutPaymethod(0, mockPayment);

            //assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
            Assert.AreNotEqual(1, mockPayment.PaymethodId);
            A.CallTo(() => mockRepository.MarkAsModified(A<Paymethods>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();

        }
    }
}