namespace PaymentGatewayApi.PaymentModels
{
    public class JwtSettings
    {
        public bool ValidateIssuerSigningKey { get; set; }

        public string IssuerSigningKey { get; set; } = string.Empty;

        public bool ValidateIssuer { get; set; }

        public string ValidIssuer { get; set; } = string.Empty;

        public bool ValidateAudience { get; set; }

        public string ValidAudience { get; set; } = string.Empty;

        public bool RequireExpirationTime { get; set; }

        public bool ValidateLifetime { get; set; }
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
