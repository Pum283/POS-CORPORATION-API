namespace Infrastructure.Dapper;

public class DapperUnitOfWork : IDapperUnitOfWork
{
    private bool _dispoded;
    private readonly DapperApplicationDbContext _context;
    public IDapperUsersRepository Users { get; private set; } = null!;

    public DapperUnitOfWork(DapperApplicationDbContext context)
    {
        _context = context;
        Init();
    }

    private void Init()
    {
        Users = new DapperUsersRepository(_context); 
    }


    public void BeginAsync()
    {
        _context.Connection?.Open();
        _context.Transaction = _context.Connection?.BeginTransaction();
    }

    public void CommitAsync()
    {
       _context.Transaction?.Commit();
        _context.Transaction?.Dispose();
        _context.Transaction = null;
    }

    public void RollbackAsync()
    {
       _context.Transaction?.Rollback();
        _context.Transaction?.Dispose();
        _context.Transaction = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_dispoded)
        {
            if (disposing)
            {
                _context.Transaction?.Dispose();
                _context.Connection?.Dispose();
            }
            _dispoded = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void CommitAndCloseConnection()
    {
        _context.Transaction?.Commit();
        _context.Transaction?.Dispose();
        _context.Transaction = null;
        _context.Connection?.Close();
        _context.Connection?.Dispose();
    }
}
