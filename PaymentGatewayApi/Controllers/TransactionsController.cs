using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.PaymentModels;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FakeItEasy;
using System.Data.Entity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentGatewayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");

        public TransactionsController(IPaymentGateContext? contextI=null)
        {
            
            if (contextI != null)
            {
                _context = contextI;
            }

        }

        // GET: api/<TransactionsController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public virtual ActionResult<IEnumerable<Transactions>> GetTransactions()
        {
            var identity = (ClaimsIdentity)User.Identity!;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            var roleslist = roles.ToList();
            var authuserid = identity.Claims.Where(c => c.Type=="Id").Select(c => c.Value);
            var authuserlist = authuserid.ToList();
            if (roleslist[0] == "3" || roleslist[0] == "4")
            { return _context.Transactions.Where(r => r.UserId.Equals(Convert.ToInt32(authuserlist[0]))).ToList(); };
            return _context.Transactions.ToList();
        }

        // GET api/<TransactionsController>/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public virtual ActionResult<Transactions>GetTransactionbyId(int id)
        {
            var transaction = _context.Transactions.Find(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // POST api/<TransactionsController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public ActionResult<Transactions>PostTransactions(Transactions newtrans)
        {
            _context.Transactions.Add(newtrans);

            try
            {
                _context.SaveChanges();
                return CreatedAtAction("GetTransactionbyId", new { id = newtrans.TransactionId }, newtrans);
            }
            catch (InvalidOperationException)
            {
                var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
                return actionResult;
            }
        }

        // PUT api/<TransactionsController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public IActionResult Puttransactions(int id, Transactions trans)
        {
            if (id != trans.TransactionId)
            {
                return BadRequest();
            }

            //_context.Entry(BookModel).State = EntityState.Modified;
            _context.MarkAsModified(trans);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                try
                {
                    var exist = _context.TransactionExists(id);
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

        // DELETE api/<TransactionsController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public ActionResult<Transactions> DeleteTrans(int id)
        {
            var transdel = _context.Transactions.Find(id);
            if (transdel == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transdel);
            _context.SaveChanges();

            return transdel;
        }
    }
}
