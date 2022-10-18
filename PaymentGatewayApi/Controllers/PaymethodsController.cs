using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.PaymentModels;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            var identity = (ClaimsIdentity)User.Identity!;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            var roleslist = roles.ToList();
            var authuserid = identity.Claims.Where(c => c.Type == "Id").Select(c => c.Value);
            var authuserlist = authuserid.ToList();
            if (roleslist[0] == "Seller" || roleslist[0] == "Customer")
            { return _context.Paymethods.Where(r => r.UserId.Equals(Convert.ToInt32(authuserlist[0]))).ToList(); };
            return _context.Paymethods.ToList();
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
            _context.Paymethods.Add(paymethod);

            try 
            { 
             _context.SaveChanges();
              return CreatedAtAction("GetPaymethodbyID", new { id = paymethod.PaymethodId }, paymethod);
            }
            catch (InvalidOperationException)
            {
                var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
                return actionResult;
            }
        }

        // PUT api/<PaymethodsController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public IActionResult PutPaymethod(int id, Paymethods paymethod)
        {
            if (id != paymethod.PaymethodId)
            {
                return BadRequest();
            }

            //_context.Entry(BookModel).State = EntityState.Modified;
            _context.MarkAsModified(paymethod);

            try
            {
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
                        var actionResult = new JsonResult(new { Message = "Unknowerror" }) { StatusCode = 500 };
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
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

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
