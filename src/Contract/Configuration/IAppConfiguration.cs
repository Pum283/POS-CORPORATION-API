namespace Contract;

public interface IAppConfiguration
{
    JwtSettings GetJwtSetting();
    string? GetSqlServerConnectionString();
    string? GetSqlServerForDapperConnectionString();
    public string? GetEnvironment();
}
