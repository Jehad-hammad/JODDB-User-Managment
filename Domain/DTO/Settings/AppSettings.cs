namespace Domain.DTO.Settings
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public JWT JWT { get; set; }
    }
   
    public class ConnectionStrings
    {
        public string ApplicationDBContext { get; set; }
    }
    public class JWT
    {
        public string TokenValidityInMinutes { get; set; }
        public string RefreshTokenValidityInDays { get; set; }
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
    }
}
