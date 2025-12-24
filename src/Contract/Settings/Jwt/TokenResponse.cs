namespace Contract;

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    //public string RefreshToken { get; set; } = string.Empty;
}
