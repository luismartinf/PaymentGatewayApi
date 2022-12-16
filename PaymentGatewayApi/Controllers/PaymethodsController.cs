using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.PaymentModels;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentGatewayApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PaymethodsController : ControllerBase
    {
        private readonly IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");


        public PaymethodsController(IPaymentGateContext? contextI = null)
        {
            if (contextI != null)
            {
                _context = contextI;
            }

        }

        // GET: api/<PaymethodsController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public virtual ActionResult<IEnumerable<Paymethods>> GetPaymethods()
        {
            foreach (var identity in User.Identities)
            {
                if (identity.Claims.Any())
                { 
                    var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                    var roleslist = roles.ToList();
                    var authuserid = Convert.ToInt32(identity.Claims.ElementAt(0).Value);
                    if (roleslist[0] == "3" || roleslist[0] == "4")
                    { return _context.Paymethods.Where(r => r.UserId.Equals(authuserid)).ToList(); };
                    return _context.Paymethods.ToList();
                };
            };
            return NoContent();
        }

        // GET api/<PaymethodsController>/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public virtual ActionResult<Paymethods> GetPaymethodbyId(int id)
        {
            var paymethods = _context.Paymethods.Find(id);

            if (paymethods == null)
            {
                return NotFound();
            }

            return paymethods;
        }

        // POST api/<PaymethodsController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public ActionResult<Paymethods> PostPaymethod(Paymethods paymethod)
        {
            Users? updateuser = new UsersController(null, _context).GetUserbyId(paymethod.UserId).Value;
            paymethod!.UserPaymethod = updateuser!;

            try 
            {
                _context.Paymethods.Add(paymethod); 
                _context.MarkAsModified(paymethod);
                _context.SaveChanges();
              return CreatedAtAction("GetPaymethodbyID", new { id = paymethod.PaymethodId }, paymethod);
            }
            catch (InvalidOperationException)
            {
                var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
                return actionResult;
            }
            catch (NotImplementedException)
            {
                var actionResult = new JsonResult(new { Message = "MarkAsModified may be not implemented" }) { StatusCode = 501 };
                return actionResult;
            }
        }

        // PUT api/<PaymethodsController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,Roles ="1,3,4")]

        public IActionResult PutPaymethod(int id, Paymethods paymethod)
        {
            if (id != paymethod.PaymethodId)
            {
                return BadRequest();
            }

            try
            {
                _context.MarkAsModified(paymethod);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                try
                {
                    var exist = _context.PaymethodExists(id);
                    if (!exist)
                    {
                        return NotFound();
                    }
                    else
                    {
                        var actionResult = new JsonResult(new { Message = "Unknow error" }) { StatusCode = 500 };
                        return actionResult;
                    }
                }
                catch (NotImplementedException)
                {
                    var actionResult = new JsonResult(new { Message = "PaymethodsExist may be not implemented" }) { StatusCode = 501 };
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


        // DELETE api/<PaymethodsController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,Roles ="1,3,4")]

        public virtual ActionResult<Paymethods> DeletePaymethod(int id)
        {
            var paydel = _context.Paymethods.Find(id);
            if (paydel == null)
            {
                return NotFound();
            }

            _context.Paymethods.Remove(paydel);
            _context.SaveChanges();

            return paydel;
        }
    }
}
