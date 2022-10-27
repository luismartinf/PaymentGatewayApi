namespace PaymentGatewayApi.PaymentModels
{
    public class JwtSettings
    {
        public bool ValidateIssuerSigningKey { get; set; } = true;

        public string IssuerSigningKey { get; set; } = "64A63153-11C1-4919-9133-EFAF99A9B456";

        public bool ValidateIssuer { get; set; } = true;

        public string ValidIssuer { get; set; } = "https://localhost:44379";

        public bool ValidateAudience { get; set; } = true;

        public string ValidAudience { get; set; } = "https://localhost:44379";

        public bool RequireExpirationTime { get; set; } = true;

        public bool ValidateLifetime { get; set; } = true;
    }

    public class UserTokens
    {
        public string Token { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public TimeSpan Validaty { get; set; }

        public string RefreshToken { get; set; } = string.Empty;

        public int UserId { get; set; }

        public Guid GuidId { get; set; }

        public DateTime ExpiredTime { get; set; }

        public string Roles { get; set; } = string.Empty;

    }
}
