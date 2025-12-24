using System.Threading.Tasks;

namespace Infrastructure;

public class UnitOfWork(EfApplicationDbContext _context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<string, object> _repository = new();
    private IDbContextTransaction? _transaction;

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public IRepositoryBase<TEntity> Repository<TEntity>() where TEntity : EntityBase<Guid>
    {
        var type = typeof(TEntity).Name;

        return (IRepositoryBase<TEntity>)_repository.GetOrAdd(type, t =>
        {
            var repositoryType = typeof(RepositoryBase<>).MakeGenericType(typeof(TEntity));
            return Activator.CreateInstance(repositoryType, _context)
            ?? throw new InvalidOperationException(
                $"Could not create repository instance for {t}");
        });
    }
    public async Task ExecuteWithTransactionAsync(Func<Task> operation)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await BeginTransactionAsync();
            try
            {
                await operation();
                await CommitTransactionAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        });
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
            return;

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _transaction!.RollbackAsync();
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public IExecutionStrategy CreateExecutionStrategyAsync()
    {
        return _context.Database.CreateExecutionStrategy();
    }
}