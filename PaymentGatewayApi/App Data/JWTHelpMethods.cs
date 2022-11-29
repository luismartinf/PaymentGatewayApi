using Microsoft.IdentityModel.Tokens;
using PaymentGatewayApi.PaymentModels;
using System.Collections;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PaymentGatewayApi.App_Data
{
    public static class JWTHelpMethods
    {
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, Guid Id)
        {

            List<Claim> claimslist = new() { 
                new Claim("UserId", userAccounts.UserId.ToString()),
                new Claim(ClaimTypes.Name, userAccounts.UserName),
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt")),
            };
            foreach (var role in userAccounts.Roles.Keys)
            { claimslist.Add(new Claim(ClaimTypes.Role, role.ToString())); }

            IEnumerable<Claim> claims = claimslist.ToArray();
            return claims;
        }
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, out Guid Id)
        {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts, Id);
        }
        public static UserTokens GenToken(UserTokens model,IConfiguration config)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JsonWebToken:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userToken = new UserTokens();
            if (model == null) throw new ArgumentException(nameof(model));
            Guid Id;
            var claims = JWTHelpMethods.GetClaims(model, out Id);
            DateTime expireTime = DateTime.Now.AddMinutes(15);
            userToken.ExpiredTime = expireTime;
            userToken.Validaty = expireTime.TimeOfDay;
            var JWToken = new JwtSecurityToken(config["JsonWebToken:Issuer"], config["JsonWebToken:Audience"], claims, expires: DateTime.Now.AddMinutes(15), signingCredentials: credentials);
            userToken.Token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            userToken.UserName = model.UserName;
            userToken.UserId = model.UserId;
            userToken.GuidId = Id;
            userToken.Roles = model.Roles;
            return userToken;
        }

    }
}
