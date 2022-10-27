using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGatewayApi.PaymentModels;
using PaymentGatewayApi.Controllers;
using FakeItEasy;
using PaymentGatewayApi.App_Data;
using System.Web.Http.Results;

namespace PaymentGateTest
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void Postnewuser_okResult()
        {
            //Arrange
            var roles = new List<Roles>();
            roles.Add(new Roles { RolesId = 2, RoleName = "Admin" }); roles.Add(new Roles { RolesId = 3, RoleName = "Seller" });
            var newUser = new Users { UserId = 3, Name = "Luis Fernandez", UserName = "luisfernandez", Roles = roles, EmailAddress = "lufer62@hotmail.com", Password = "Cachorrito8963#", PhoneNumber = 5595643218, URL = null };

            //Act
            var usersController = new UsersController(null);
            var actionResult = usersController.PostUser(newUser);

            //Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod]
        public void createtokentest()
        {
            //Arrange
            
            var fakecontext = A.Fake<IPaymentGateContext>();
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).Returns(new Users { UserId = 1, UserName = "fakeuser", Roles = new List<Roles> { new Roles { RoleName = "Admin" }, new Roles { RoleName = "Customer" } },Password="fakepassword",});
           
            //Act
            var usersController = new UsersController(fakecontext);
            var actionResult= usersController.CreateToken("fakeuser", "fakepassword").Value;
            var token = actionResult!.Token;

            //Assert
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).MustHaveHappened();
        }
    }
}
