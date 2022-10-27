using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.PaymentModels;
using System.Data;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentGatewayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
        public RolesController(IPaymentGateContext? contextI = null)
        {
            if (contextI != null)
            {
                _context = contextI;
            }
        }

        // GET: api/<RolesController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "1, 2")]
        public ActionResult<IEnumerable<Roles>> GetRoles()
        {
            return _context.Roles.ToList();
        }

        // GET api/<RolesController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
          public ActionResult<Roles> GetRolesbyId(int id)
        {
            var roles = _context.Roles.Find(id);

            if (roles == null)
            {
                return NotFound();
            }

            return roles;
        }

        // POST api/<RolesController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "1")]
        public ActionResult<Roles> Post(Roles newrole)
        {
            _context.Roles.Add(newrole);

            try
            {
                _context.SaveChanges();
                return CreatedAtAction("GetRolesbyId", new { id = newrole.RolesId }, newrole);
            }
            catch (InvalidOperationException)
            {
                var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
                return actionResult;
            }
        }

        // PUT api/<RolesController>/5
        [HttpPut("{id}")]
        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "1, 2")]
        public IActionResult PutRoles(int id, Roles role)
        {
            if (id != role.RolesId)
            {
                return BadRequest();
            }

            //_context.Entry(BookModel).State = EntityState.Modified;
            _context.MarkAsModified(role);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                try
                {
                    var exist = _context.RolesExists(id);
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
                    var actionResult = new JsonResult(new { Message = "RolesExist may be not implemented" }) { StatusCode = 501 };
                    return actionResult;
                }
            }
            catch (NotImplementedException)
            {
                var actionResult = new JsonResult(new { Message = "MarkAsModified may be not implemented" }) { StatusCode = 501 };
                return actionResult;
            }
            catch (InvalidOperationException)
            {
                var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
                return actionResult;
            }

            return NoContent();
        }

        // DELETE api/<RolesController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "1")]
        public ActionResult<Roles> DeleteRoles(int id)
        {
            var rolesdel = _context.Roles.Find(id);
            if (rolesdel == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(rolesdel);
            _context.SaveChanges();

            return rolesdel;
        }
    }
}
