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

        public ActionResult<IEnumerable<Transfers>> GetTranfers()
        {
            var identity = (ClaimsIdentity)User.Identity!;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            var roleslist = roles.ToList();
            var authuserid = identity.Claims.Where(c => c.Type == "Id").Select(c => c.Value);
            var authuserlist = authuserid.ToList();
            if (roleslist[0] == "Seller" || roleslist[0] == "Customer")
            { return _context.Transfers.Where(r => r.UserId.Equals(Convert.ToInt32(authuserlist[0]))).ToList(); };
            return _context.Transfers.ToList();
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
        _context.Transfers.Add(newtransf);

        try
        {
            _context.SaveChanges();
            return CreatedAtAction("GetTransactionbyId", new { id = newtransf.TransactionId }, newtransf);
        }
        catch (InvalidOperationException)
        {
            var actionResult = new JsonResult(new { Message = "Foreign Key doesn´t match" }) { StatusCode = 409 };
            return actionResult;
        }
    }

        // PUT api/<TransferController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "SuperAdmin, Admin")]
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
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "SuperAdmin, Admin")]

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
