namespace Infrastructure.Dapper;

public interface IDapperUnitOfWork : IDisposable
{
    IDapperUsersRepository Users { get; }
    void BeginAsync();
    void CommitAsync();
    void RollbackAsync();
    void CommitAndCloseConnection();
}
