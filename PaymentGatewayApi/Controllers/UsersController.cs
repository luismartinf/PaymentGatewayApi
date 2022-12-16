using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.PaymentModels;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentGatewayApi.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config = WebApplication.Create().Configuration;
        private readonly IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
       
        public UsersController (IConfiguration? config, IPaymentGateContext? contextI = null)
        {
            if (contextI != null)
            {_context = contextI;}

            if (config != null)
            { _config = config; }
        }

        // GET: api/<UsersController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, Roles = "1,2")]
        
        public virtual ActionResult<IEnumerable<Users>> GetUsers()
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
            newUser.AddDate = DateTime.Now;
            newUser.UpdateDate = DateTime.Now;
            _context.Users.Add(newUser);

            try
            {
                foreach (var role in newUser.Roles.ToList())
                {
                    Roles? updaterole = new RolesController(_context).GetRolesbyId(role.RolesId).Value;
                    _context.Addusersinroles(updaterole!, newUser);
                    _context.Roles.Attach(updaterole!);
                    _context.MarkAsModified(updaterole!);
                }
                _context.SaveChanges();
                var token = CreateToken(new UserLogins { UserName = newUser.UserName, Password = newUser.Password });
                return CreatedAtAction("GetUserbyId", new { id = newUser.UserId }, newUser);
            }
            catch (NotImplementedException)
            {
                var actionResult = new JsonResult(new { Message = "Addusersinroles or MarkAsModified may be not implemented" }) { StatusCode = 501 };
                return actionResult;
            }
        }

        // POST api/<TokensController>
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<UserTokens> CreateToken(UserLogins userLogin)
        {
            
            Users? userdata;
            try { userdata = _context.Findusertoken(userLogin.UserName); }
            catch (NotImplementedException)
            {
                var actionResult = new JsonResult(new { Message = "Findusertoken may be not implemented" }) { StatusCode = 501 };
                return actionResult;
            }
            if (userdata == null) { return NotFound(); };
            if (userdata.Password == userLogin.Password)
            {
                var listroles = userdata.Roles.ToList();
                Dictionary<int, string> dicroles = new();
                foreach (var role in listroles) { dicroles.Add(role.RolesId, role.RoleName); };

                var token = new UserTokens
                {
                    UserName = userdata.UserName,
                    UserId = userdata.UserId,
                    Roles = dicroles
                };

                token = JWTHelpMethods.GenToken(token,_config);
                return new ActionResult<UserTokens>(token);
            }
            else
            {
                return BadRequest("wrong password");
            }

        }

       
    

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, Roles = "1,3,4")]
        public IActionResult Putuser(int id, Users user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            try 
            {
                try
                {
                    var olduser = _context.Users.Find(id);
                    bool delroles = _context.Anyexceprole(olduser, user);
                    bool newroles = _context.Anyexceprole(user, olduser);
                    if (delroles || newroles)
                    {
                        foreach (var role in _context.Rolestolist())
                        {
                            _context.Delusersinroles(role,user);
                            _context.Roles.Attach(role);
                            _context.MarkAsModified(role);
                        }

                        foreach (var role in user.Roles.ToList())
                        {
                            Roles? updaterole = new RolesController(_context).GetRolesbyId(role.RolesId).Value;
                            _context.Addusersinroles(updaterole!,user);
                            _context.Roles.Attach(updaterole!);
                            _context.MarkAsModified(updaterole!);
                        }

                    }
                }
                catch (InvalidOperationException) { return NotFound(); }
                catch (NotImplementedException)
                {
                    var actionResult = new JsonResult(new { Message = "Rolestolist, Delusersinroles, Addusersinroles or Anyexceprole may not be implemented" }) { StatusCode = 501 };
                    return actionResult;
                }
            
                user.UpdateDate = DateTime.Now;
                _context.MarkAsModified(user);
                _context.SaveChanges();
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
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, Roles = "1,3,4")]
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
