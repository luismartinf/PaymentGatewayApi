using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.Controllers;
using PaymentGatewayApi.PaymentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateTest
{
    [TestClass]
    internal class IntegratingTesting
    {
        [TestMethod]
        public void PostPaymethods_okResult_Integrating()
        {

            // Arrange
            Paymethods paymethodcur = new() { TypePayment = "Card", PaymentNum = 4257891824019283, BillingAdress = "Gardenias 1637, Alamos, Xochimilco cp:5679, CDMX, Mexico", UserId = 2 };
            // Act
            PaymethodsController controller = new(null);
            var actionResult = controller.PostPaymethod(paymethodcur);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Paymethods>));

        }

        [TestMethod]
        public void Postnewuser_okResult_Integrating()
        {
            //Arrange
            var roles = new List<Roles>();
            roles.Add(new Roles { RolesId = 3, RoleName = "Seller" });
            var newUser = new Users { UserId = 3, Name = "Martín García", UserName = "martingarcia", Roles = roles, EmailAddress = "mgar67@hotmail.com", Password = "Camarena56%", PhoneNumber = 2227645190, URL = "https://www.floresgarcia.com" };

            //Act
            var usersController = new UsersController(null, null);
            var actionResult = usersController.PostUser(newUser);

            //Assert
            Assert.IsNotNull(actionResult);
        }
    }
}
