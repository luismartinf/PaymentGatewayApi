using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApi.App_Data;
using System.Data.Entity;
using PaymentGatewayApi.PaymentModels;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentGatewayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransfersController : ControllerBase 
    {
        private readonly IPaymentGateContext _context = new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
        public TransfersController(IPaymentGateContext? contextI = null)
        {
            if (contextI != null)
            {
                _context = contextI;
            }
        }


        // GET: api/<TransferController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public virtual ActionResult<IEnumerable<Transfers>> GetTranfers()
        {
            foreach (var identity in User.Identities)
            {
                if (identity.Claims.Any())
                {
                    var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                    var roleslist = roles.ToList();
                    var authuserid = Convert.ToInt32(identity.Claims.ElementAt(0).Value);
                    if (roleslist[0] == "3" || roleslist[0] == "4")
                    { return _context.Transfers.Where(r => r.UserId.Equals(authuserid)).ToList(); };
                    return _context.Transfers.ToList();
                };
            };
            return NoContent();
        }

        // GET api/<TransferController>/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

        public ActionResult<Transfers> GetTransferbyId(int id)
        {
            var transfer = _context.Transfers.Find(id);

            if (transfer== null)
            {
                return NotFound();
            }

            return transfer;
        }
    

        // POST api/<TransferController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<Transfers> PostTransfer(Transfers newtransf)
        {
            Users? updateuser = new UsersController(null, _context).GetUserbyId(newtransf.UserId).Value;
            newtransf!.UserTransfers = updateuser!;
            Transactions? updatetrans = new TransactionsController(_context).GetTransactionbyId(newtransf.TransactionId).Value;
            newtransf!.TransTransfers = updatetrans!;

            try
            {
                _context.Transfers.Add(newtransf);
                _context.MarkAsModified(newtransf);
                _context.SaveChanges();
                return CreatedAtAction("GetPaymethodbyID", new { id = newtransf.TransferId }, newtransf);
            }
            catch (InvalidOperationException)
        {
            var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
            return actionResult;
        }
    }

        // PUT api/<TransferController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, Roles = "1,2")]
        public IActionResult Puttransfer(int id, Transfers transf)
        {
            if (id != transf.TransferId)
            {
                return BadRequest();
            }

            //_context.Entry(BookModel).State = EntityState.Modified;
            _context.MarkAsModified(transf);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                try
                {
                    var exist = _context.TransferExists(id);
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
                    var actionResult = new JsonResult(new { Message = "TransferExist may be not implemented" }) { StatusCode = 501 };
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

        // DELETE api/<TransferController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
     

        public ActionResult<Transfers> DeleteTransf(int id)
        {
            var transfdel = _context.Transfers.Find(id);
            if (transfdel == null)
            {
                return NotFound();
            }

            _context.Transfers.Remove(transfdel);
            _context.SaveChanges();

            return transfdel;
        }
    }
}
