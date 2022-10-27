using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.PaymentModels;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Xml.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentGatewayApi.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
        public UsersController (IPaymentGateContext? contextI = null)
        {
            if (contextI != null)
            {
                _context = contextI;
            }
            this._jwtSettings = new JwtSettings();
        }

        // GET: api/<UsersController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "1")]
        public ActionResult<IEnumerable<Users>> GetUsers()
        {
            return _context.Users.ToList();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<Users> GetUserbyId(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST api/<UsersController>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<Users> PostUser(Users newUser)
        {
            _context.Users.Add(newUser);

            foreach (var role in newUser.Roles.ToList())
            {
                Roles? updaterole = new RolesController(_context).GetRolesbyId(role.RolesId).Value;
                updaterole!.Users.Add(newUser);
                _context.Roles.Attach(updaterole);
                _context.MarkAsModified(updaterole);
            }

            try
            {
                _context.SaveChanges();
                var token = CreateToken(newUser.UserName, newUser.Password);
                return CreatedAtAction("GetUserbyId", new { id = newUser.UserId }, newUser);
            }
            catch (InvalidOperationException)
            {
                var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
                return actionResult;
            }
        }

        // POST api/<TokensController>
        [HttpPost]
        [AllowAnonymous]

        public ActionResult<UserTokens> CreateToken(string username, string password)
        {
            Users? userdata;
            try{ userdata = _context.Findusertoken(username); }
            catch (NotImplementedException)
            {
                var actionResult = new JsonResult(new { Message = "findusertoken may be not implemented" }) { StatusCode = 501 };
                return actionResult;
            }
           if (userdata == null) { return NotFound(); };
           if (userdata.Password == password)
           {
                var listroles = userdata.Roles.ToList();
                string stringroles = string.Empty;
                foreach (var role in listroles) { stringroles = $"{stringroles}, {role.RolesId}"; };
                stringroles = stringroles.Remove(0, 2);

                var token = new UserTokens
                {
                    UserName = userdata.UserName,
                    UserId = userdata.UserId,
                    Roles = stringroles
                };

                token = JwtHelpers.GenTokenkey(token, _jwtSettings);
                return new ActionResult<UserTokens>(token);
           }
           else
           {
                return BadRequest("wrong password");
           }
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles ="1, 3, 4")]
        public IActionResult Putuser(int id, Users user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            //_context.Entry(BookModel).State = EntityState.Modified;
            _context.MarkAsModified(user);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                try
                {
                    var exist = _context.UsersExists(id);
                    if (!exist)
                    {
                        return NotFound();
                    }
                    else
                    {
                        var actionResult = new JsonResult(new { Message = "Unknowerror" }) { StatusCode = 500 };
                        return actionResult;
                    }
                }
                catch (NotImplementedException)
                {
                    var actionResult = new JsonResult(new { Message = "UserExist may not be implemented" }) { StatusCode = 501 };
                    return actionResult;
                }
            }
            catch (NotImplementedException)
            {
                var actionResult = new JsonResult(new { Message = "MarkAsModified may not be implemented" }) { StatusCode = 501 };
                return actionResult;
            }
            catch (InvalidOperationException)
            {
                var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
                return actionResult;
            }

            return NoContent();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "1, 3, 4")]
        public ActionResult<Users> Delete(int id)
        {
            var userdel = _context.Users.Find(id);
            if (userdel == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userdel);
            _context.SaveChanges();

            return userdel;
        }
    }
}
