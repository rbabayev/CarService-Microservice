namespace CarServiceBG.Interface.Tokens
{
    public class TokenSettings
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
