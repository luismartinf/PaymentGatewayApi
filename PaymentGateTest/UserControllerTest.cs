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
using Microsoft.AspNetCore.Mvc;
using System.Data;
using NuGet.Common;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace PaymentGateTest
{
    [TestClass]
    public class UserControllerTest
    {
        private IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
        private readonly IConfiguration _config = WebApplication.Create().Configuration;

        [TestMethod]
        public void Getusers_okResult()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            UsersController fakecontroller = A.Fake<UsersController>(x => x.WithArgumentsForConstructor(() => new UsersController(null,mockRepository)));
            var dummies = (List<Users>)A.CollectionOfDummy<Users>(Elemntsindb());
            A.CallTo(() => fakecontroller.GetUsers()).Returns(dummies);

            // Act
            var controller = new UsersController(null, _context);
            var actionResult = controller.GetUsers();
            var content = actionResult.Value as IList<Users>;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Users>>));
            Assert.AreEqual(dummies.Count, content?.Count);
        }

        
        [TestMethod]
        public void Getuserbyid()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Users.Find(2)).Returns(new Users {  UserId= 2,UserName= "raulmartinez", EmailAddress= "rmartin45@hotmail.com",Name= "RaulMartinez",Password= "Ternera4628%",PhoneNumber= 5538720193,URL= null,AddDate = DateTime.UnixEpoch,UpdateDate= DateTime.UnixEpoch, Roles=null});

            // Act
            var mockcontroller = new UsersController(null,mockRepository);
            var controller = new UsersController(null,null);
            var fakeLendee = mockcontroller.GetUserbyId(2).Value; ;
            var contentResult = controller.GetUserbyId(2).Value;


            // Assert
            A.CallTo(() => mockRepository.Users.Find(2)).MustHaveHappened();
            Assert.AreEqual(fakeLendee!.UserId, contentResult!.UserId);
            Assert.AreEqual(fakeLendee.Name, contentResult.Name);

        }

        [TestMethod]
        public void Getuserbyid_notfound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Users.Find(Elemntsindb() + 5))!.Returns(null);

            // Act
            var controller = new UsersController(null,mockRepository);
            var actionResult = controller.GetUserbyId(Elemntsindb() + 5).Result;


            // Assert
            A.CallTo(() => mockRepository.Users.Find(Elemntsindb() + 5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));


        }

        [TestMethod]
        public void Postnewuser_okResult()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            UsersController controller = new(null, mockRepository);
            List<Roles> roleadd = new() { new Roles{ RolesId = 1, RoleName = "SuperAdmin", Users = null } };
            var user= new Users{ UserId = Elemntsindb() + 1, UserName = "raulmartinez", EmailAddress = "rmartin45@hotmail.com", Name = "RaulMartinez", Password = "Ternera4628%", PhoneNumber = 5538720193, URL = null, AddDate = DateTime.UnixEpoch, UpdateDate = DateTime.UnixEpoch, Roles = roleadd };
            A.CallTo(() => mockRepository.Users.Add(user)).Returns(user);
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).Returns(roleadd[0]);
            A.CallTo(() => mockRepository.Findusertoken(A<string>.Ignored)).Returns(user);
            
            // Act
            var actionResult = controller.PostUser(user);
            var token = controller.CreateToken(new UserLogins { UserName = user.UserName, Password = user.Password }).Value!.Token;

            // Assert
            A.CallTo(() => mockRepository.Users.Add(user)).MustHaveHappened();
            A.CallTo(() => mockRepository.Addusersinroles(A<Roles>.Ignored, user)).MustHaveHappened();
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Roles>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.Findusertoken(A<string>.Ignored)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Users>));
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void PostMethodReturn_Notimplemented()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            A.CallTo(() => fakeuser.Roles).Returns(new List<Roles>{new Roles { RolesId = 1, RoleName = "SuperAdmin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } } );
            A.CallTo(() => mockRepository.Addusersinroles(A<Roles>.Ignored,fakeuser)).Throws(new NotImplementedException());

            // Act
            UsersController controller = new(null,mockRepository);
            var actionResult = controller.PostUser(fakeuser);
          
            // Assert
            A.CallTo(() => mockRepository.Users.Add(A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.Addusersinroles(A<Roles>.Ignored, fakeuser)).MustHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.Addusersinroles(A.Fake<Roles>(), fakeuser));
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).MustNotHaveHappened();
        }

        [TestMethod]
        public void PostMethodReturn_NotModifiedstatedBadrequest()
        {

            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            UsersController controller = new(null, mockRepository);
            List<Roles> roleadd = new() { new Roles { RolesId = 1, RoleName = "SuperAdmin", Users = null } };
            var user = new Users { UserId = Elemntsindb() + 1, UserName = "raulmartinez", EmailAddress = "rmartin45@hotmail.com", Name = "RaulMartinez", Password = "Ternera4628%", PhoneNumber = 5538720193, URL = null, AddDate = DateTime.UnixEpoch, UpdateDate = DateTime.UnixEpoch, Roles = roleadd };
            
            A.CallTo(() => mockRepository.Users.Add(user)).Returns(user);
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).Returns(roleadd[0]);
            A.CallTo(() => mockRepository.SaveChanges()).Throws(new InvalidOperationException());

            // Act
            ActionResult<Users> actionResult;
            try { actionResult = controller.PostUser(user); }
            catch { actionResult = new Microsoft.AspNetCore.Mvc.BadRequestResult(); }

            // Assert
            A.CallTo(() => mockRepository.Users.Add(user)).MustHaveHappened();
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Roles>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            A.CallTo(() => mockRepository.Findusertoken(A<string>.Ignored)).MustNotHaveHappened();
            Assert.IsInstanceOfType(actionResult.Result, typeof(Microsoft.AspNetCore.Mvc.BadRequestResult));
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.SaveChanges());
        }

        [TestMethod]
        public void Createtokentest_validtoken()
        {
            //Arrange
            var fakecontext = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).Returns(fakeuser);
            A.CallTo(() => fakeuser.UserName).Returns("fakeuser");
            A.CallTo(() => fakeuser.Password).Returns("fakepassword");
            A.CallTo(() => fakeuser.Roles).Returns(new List<Roles> { new Roles { RolesId = 2, RoleName = "Admin" }, new Roles { RolesId = 4, RoleName = "Customer" } });

            //Act
            var usersController = new UsersController(null,fakecontext);
            var actionResult = usersController.CreateToken(new UserLogins { UserName = "fakeuser", Password = "fakepassword" }).Value;
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(actionResult!.Token,validateParameters(_config) , out SecurityToken validatedToken);
            var usernametok = principal.FindFirstValue(ClaimTypes.Name);

            //Assert
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeuser.Roles).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult.GuidId, typeof(Guid));
            Assert.AreEqual(actionResult.UserName,usernametok);
        }

        [TestMethod]
        public void Createtokentest_Notfound()
        {
            //Arrange
            var fakecontext = A.Fake<IPaymentGateContext>();
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).Returns(null);

            //Act
            var usersController = new UsersController(null, fakecontext);
            var actionResult = usersController.CreateToken(new UserLogins { UserName = "fakeuser", Password = "fakepassword" });
           
            //Assert
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).MustHaveHappened();
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Result, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));
        }

        [TestMethod]
        public void Createtokentest_FindUserException()
        {
            //Arrange
            var fakecontext = A.Fake<IPaymentGateContext>();
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).Throws(new NotImplementedException());

            //Act
            var usersController = new UsersController(null, fakecontext);
            var actionResult = usersController.CreateToken(new UserLogins { UserName = "fakeuser", Password = "fakepassword" });
           
            //Assert
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).MustHaveHappened();
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Result, typeof(JsonResult));
        }


        [TestMethod]
        public void Createtokentest_WrongPassword()
        {
            //Arrange
            var fakecontext = A.Fake<IPaymentGateContext>();
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).Returns(new Users { UserId = 1, UserName = "fakeuser", Roles = new List<Roles> { new Roles { RolesId = 2, RoleName = "Admin" }, new Roles { RolesId = 4, RoleName = "Customer" } }, Password = "fakepassword", });

            //Act
            var usersController = new UsersController(null, fakecontext);
            var actionResult = usersController.CreateToken(new UserLogins { UserName = "fakeuser", Password = "password" });

            //Assert
            A.CallTo(() => fakecontext.Findusertoken(A<string>.Ignored)).MustHaveHappened();
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Result, typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult));
        }

        [TestMethod]
        public void PutReturnOkwithoutroles() 
        {
            //Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            A.CallTo(() => fakeuser.UserId).Returns(2);
            A.CallTo(() => mockRepository.Users.Find(2)).Returns(new Users {UserId=2,Name="Roberto Perez",EmailAddress="Robper@gmail.com",UserName="robertoperez", Password="Centauro$456",PhoneNumber=5563490231,URL=null, Roles = new List<Roles> { new Roles { RolesId = 2, RoleName = "Admin" ,Users=null}, new Roles { RolesId = 4, RoleName = "Customer", Users=null } }, AddDate=DateTime.Now});
            A.CallTo(() => fakeuser.Roles).Returns(new List<Roles> { new Roles { RolesId = 2, RoleName = "Admin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });

            //Act
            UsersController controller = new(null,mockRepository);
            var actionResult = controller.Putuser(2, fakeuser);

            //Assert
            A.CallTo(() => fakeuser.UserId).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored,A<Users>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).MustNotHaveHappened();
            A.CallToSet(() => fakeuser.UpdateDate).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
        }

        [TestMethod]
        public void PutReturnOkwithroles()
        {
            //Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            var fakerole = A.Fake<Roles>();
            A.CallTo(() => fakeuser.UserId).Returns(2);
            A.CallTo(() => fakerole.Users).Returns(A.Fake<List<Users>>());
            A.CallTo(() => mockRepository.Rolestolist()).Returns(new List<Roles> { new Roles { RolesId = 2, RoleName = "Admin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });
            A.CallTo(() => fakeuser.Roles).Returns(new List<Roles> { new Roles { RolesId = 1, RoleName = "SuperAdmin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });
            A.CallTo(() => mockRepository.Delusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).Returns(true);
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored, A<Users>.Ignored)).Returns(true);
            
            //Act
            UsersController controller = new(null, mockRepository);
            var actionResult = controller.Putuser(2, fakeuser);

            //Assert
            A.CallTo(() => fakeuser.UserId).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored, A<Users>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallTo(() => mockRepository.Rolestolist()).MustHaveHappened();
            A.CallTo(() => mockRepository.Delusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeuser.Roles).MustHaveHappened();
            A.CallTo(() => mockRepository.Addusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallTo(() => mockRepository.MarkAsModified(A<Roles>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallToSet(() => fakeuser.UpdateDate).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
        }

        [TestMethod]
        public void PutReturnNotfound()
        {
            //Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            var fakerole = A.Fake<Roles>();
            A.CallTo(() => fakeuser.UserId).Returns(2);
            A.CallTo(() => mockRepository.Users.Find(2)).Throws(new InvalidOperationException());
            
            //Act
            UsersController controller = new(null, mockRepository);
            var actionResult = controller.Putuser(2, fakeuser);

            //Assert
            A.CallTo(() => fakeuser.UserId).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Users>.Ignored)).MustNotHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.MarkAsModified(A<Users>.Ignored));
            Assert.IsInstanceOfType(actionResult, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));
        }

        [TestMethod]
        public void PutReturnBadrequest()
        {
            //Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            var fakerole = A.Fake<Roles>();
            A.CallTo(() => fakeuser.UserId).Returns(2);
            
            //Act
            UsersController controller = new(null, mockRepository);
            var actionResult = controller.Putuser(4, fakeuser);

            //Assert
            A.CallTo(() => fakeuser.UserId).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Find(2)).MustNotHaveHappened();
            Assert.IsInstanceOfType(actionResult, typeof(Microsoft.AspNetCore.Mvc.BadRequestResult));
        }

        [TestMethod]
        public void PutReturnNotimplemented()
        {
            //Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            var fakerole = A.Fake<Roles>();
            A.CallTo(() => fakeuser.UserId).Returns(2);
            A.CallTo(() => fakerole.Users).Returns(A.Fake<List<Users>>());
            A.CallTo(() => mockRepository.Rolestolist()).Returns(new List<Roles> { new Roles { RolesId = 2, RoleName = "Admin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });
            A.CallTo(() => fakeuser.Roles).Returns(new List<Roles> { new Roles { RolesId = 1, RoleName = "SuperAdmin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored, A<Users>.Ignored)).Throws(new NotImplementedException());

            //Act
            UsersController controller = new(null, mockRepository);
            var actionResult = controller.Putuser(2, fakeuser);

            //Assert
            A.CallTo(() => fakeuser.UserId).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored, A<Users>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mockRepository.Rolestolist()).MustNotHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Users>.Ignored)).MustNotHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.Anyexceprole(A.Fake<Users>(), A.Fake<Users>()));
            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
        }

        [TestMethod]
        public void PutNotImplementedUserMarkasModified()
        {
            //Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            var fakerole = A.Fake<Roles>();
            A.CallTo(() => fakeuser.UserId).Returns(2);
            A.CallTo(() => fakerole.Users).Returns(A.Fake<List<Users>>());
            A.CallTo(() => mockRepository.Rolestolist()).Returns(new List<Roles> { new Roles { RolesId = 2, RoleName = "Admin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });
            A.CallTo(() => fakeuser.Roles).Returns(new List<Roles> { new Roles { RolesId = 1, RoleName = "SuperAdmin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });
            A.CallTo(() => mockRepository.Delusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).Returns(true);
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored, A<Users>.Ignored)).Returns(true);
            A.CallTo(() => mockRepository.MarkAsModified(A<Users>.Ignored)).Throws(new NotImplementedException());


            //Act
            UsersController controller = new(null, mockRepository);
            var actionResult = controller.Putuser(2, fakeuser);

            //Assert
            A.CallTo(() => fakeuser.UserId).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored, A<Users>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallTo(() => mockRepository.Rolestolist()).MustHaveHappened();
            A.CallTo(() => mockRepository.Delusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeuser.Roles).MustHaveHappened();
            A.CallTo(() => mockRepository.Addusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallTo(() => mockRepository.MarkAsModified(A<Roles>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallToSet(() => fakeuser.UpdateDate).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
            Assert.ThrowsException<NotImplementedException>(() => mockRepository.MarkAsModified(A.Fake<Users>()));
            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
        }

        [TestMethod]
        public void PutForeignkeyDismtch()
        {
            //Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            var fakeuser = A.Fake<Users>();
            var fakerole = A.Fake<Roles>();
            A.CallTo(() => fakeuser.UserId).Returns(2);
            A.CallTo(() => fakerole.Users).Returns(A.Fake<List<Users>>());
            A.CallTo(() => mockRepository.Rolestolist()).Returns(new List<Roles> { new Roles { RolesId = 2, RoleName = "Admin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });
            A.CallTo(() => fakeuser.Roles).Returns(new List<Roles> { new Roles { RolesId = 1, RoleName = "SuperAdmin", Users = null }, new Roles { RolesId = 4, RoleName = "Customer", Users = null } });
            A.CallTo(() => mockRepository.Delusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).Returns(true);
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored, A<Users>.Ignored)).Returns(true);
            A.CallTo(() => mockRepository.MarkAsModified(A<Users>.Ignored)).Throws(new InvalidOperationException());


            //Act
            UsersController controller = new(null, mockRepository);
            var actionResult = controller.Putuser(2, fakeuser);

            //Assert
            A.CallTo(() => fakeuser.UserId).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Anyexceprole(A<Users>.Ignored, A<Users>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallTo(() => mockRepository.Rolestolist()).MustHaveHappened();
            A.CallTo(() => mockRepository.Delusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeuser.Roles).MustHaveHappened();
            A.CallTo(() => mockRepository.Addusersinroles(A<Roles>.Ignored, A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.Roles.Attach(A<Roles>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallTo(() => mockRepository.MarkAsModified(A<Roles>.Ignored)).MustHaveHappenedTwiceOrMore();
            A.CallToSet(() => fakeuser.UpdateDate).MustHaveHappened();
            A.CallTo(() => mockRepository.MarkAsModified(A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.MarkAsModified(A.Fake<Users>()));
            Assert.IsInstanceOfType(actionResult, typeof(IActionResult));
        }

        [TestMethod]
        public void DeleteReturnsOk()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Users.Find(2)).Returns(A.Fake<Users>()) ;

            // Act
            UsersController controller = new(null,mockRepository);
            var actionResult = controller.Delete(2);

            // Assert
            A.CallTo(() => mockRepository.Users.Find(2)).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Remove(A<Users>.Ignored)).MustHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustHaveHappened();
            Assert.IsNotNull(actionResult);

        }

        [TestMethod]
        public void DeleteReturnsNotFound()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Users.Find(Elemntsindb() + 5))!.Returns(null);

            // Act
            UsersController controller = new(null,mockRepository);
            var actionResult = controller.Delete(Elemntsindb() + 5);

            // Assert

            A.CallTo(() => mockRepository.Users.Find(Elemntsindb() + 5)).MustHaveHappened();
            Assert.IsInstanceOfType(actionResult.Result, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));
            A.CallTo(() => mockRepository.Users.Remove(A<Users>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }

        [TestMethod]
        public void DeleteReturnsBadrequest()
        {
            // Arrange
            IPaymentGateContext mockRepository = A.Fake<IPaymentGateContext>();
            A.CallTo(() => mockRepository.Users.Find(3)).Returns(A.Fake<Users>()); ;
            A.CallTo(() => mockRepository.Users.Remove(A<Users>.Ignored)).Throws(new InvalidOperationException());

            // Act
            UsersController controller = new(null,mockRepository);
            ActionResult<Users> actionResult;
            try {actionResult = controller.Delete(3);}
            catch {actionResult = new Microsoft.AspNetCore.Mvc.BadRequestResult();}

            // Assert
            A.CallTo(() => mockRepository.Users.Find(3)).MustHaveHappened();
            A.CallTo(() => mockRepository.Users.Remove(A<Users>.Ignored)).MustHaveHappened();
            Assert.ThrowsException<InvalidOperationException>(() => mockRepository.Users.Remove(A<Users>.Ignored));
            Assert.IsInstanceOfType(actionResult.Result, typeof(Microsoft.AspNetCore.Mvc.BadRequestResult));
            A.CallTo(() => mockRepository.SaveChanges()).MustNotHaveHappened();
        }



        private int Elemntsindb()
        {
            var elementsdb = _context.Users.ToList().Count; ;
            return elementsdb;
        }
        
        private TokenValidationParameters validateParameters(IConfiguration config)
        {
            var validateParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["JsonWebToken:Issuer"],
                ValidAudience = config["JsonWebToken:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JsonWebToken:Key"]))
            };

            return validateParameters;
        }
    }
}
