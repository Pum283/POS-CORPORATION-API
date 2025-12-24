using Microsoft.Extensions.Configuration;

namespace Infrastructure.Dapper;

public sealed class DapperApplicationDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public DapperApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString(AppConstants.SqlServerForDapperConnection)!;
    }

    public IDbConnection? Connection
    {
        get
        {
            if(_connection is null || _connection.State != ConnectionState.Open)
                _connection = new SqlConnection(_connectionString);
            return _connection;
        }
    }
    public IDbTransaction? Transaction
    {
        get => _transaction;
        set => _transaction = value;
    }
}