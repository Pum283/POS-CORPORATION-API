using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Contract;

public class AppConfiguration(IConfiguration _configuration, IHostEnvironment _hostEnvironment) : IAppConfiguration
{

    public JwtSettings GetJwtSetting()
    {
        //return _configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        var audience = _configuration["JwtSettings:Audience"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var secretKey = _configuration["JwtSettings:SecretKey"];
        var accessTokenExpiration = _configuration["JwtSettings:AccessTokenExpiration"];
        var refreshTokenExpiration = _configuration["JwtSettings:RefreshTokenExpiration"];

        return new JwtSettings
        {
            Audience = audience ?? "",
            Issuer = issuer ?? "",
            SecretKey = secretKey ?? "",
            AccessTokenExpiration = int.Parse(accessTokenExpiration ?? "")  ,
            RefreshTokenExpiration = int.Parse(refreshTokenExpiration ?? "")
        };
    }

    public string? GetSqlServerConnectionString()
     => _configuration.GetConnectionString(AppConstants.SqlServerConnection) ?? throw new Exception("An unexpected error occurred.");

    public string? GetSqlServerForDapperConnectionString()
     => _configuration.GetConnectionString(AppConstants.SqlServerForDapperConnection) ?? throw new Exception("An unexpected error occurred.");

    public string? GetEnvironment()
    {
        return _hostEnvironment.EnvironmentName;
    }
}
