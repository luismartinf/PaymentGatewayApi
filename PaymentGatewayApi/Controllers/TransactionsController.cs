using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using PaymentGatewayApi.PaymentModels;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FakeItEasy;
using System.Data.Entity;
using System.Security.Principal;

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
            foreach (var identity in User.Identities)
            {
                if (identity.Claims.Any())
                {
                    var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                    var roleslist = roles.ToList();
                    var authuserid = Convert.ToInt32(identity.Claims.ElementAt(0).Value);
                    if (roleslist[0] == "3" || roleslist[0] == "4")
                    { return _context.Transactions.Where(r => r.UserId.Equals(authuserid)).ToList(); };
                    return _context.Transactions.ToList(); 
                };
            };
            return NoContent();
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
            Users? updateuser = new UsersController(null, _context).GetUserbyId(newtrans.UserId).Value;
            newtrans!.UserTrans = updateuser!;
            Paymethods? updatepay = new PaymethodsController(_context).GetPaymethodbyId(newtrans.PaymethodId).Value;
            newtrans!.PaymethodTrans = updatepay!;

            try
            {
                _context.Transactions.Add(newtrans);
                _context.MarkAsModified(newtrans);
                _context.SaveChanges();
                return CreatedAtAction("GetPaymethodbyID", new { id = newtrans.TransactionId }, newtrans);
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
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, Roles ="1")]

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
