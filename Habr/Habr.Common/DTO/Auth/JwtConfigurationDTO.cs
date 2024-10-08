namespace Habr.Common.DTO.Auth
{
    public class JwtConfigurationDTO
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string AccessExpiresInSeconds { get; set; }
        public string RefreshExpiresInDays { get; set; }
    }
}
