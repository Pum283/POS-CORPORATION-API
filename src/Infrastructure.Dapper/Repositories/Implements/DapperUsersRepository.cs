namespace Infrastructure.Dapper;

[RegisterService(typeof(IDapperUsersRepository))]
public class DapperUsersRepository : DapperRepositoryBase<UserEntity>, IDapperUsersRepository
{

    public DapperUsersRepository(DapperApplicationDbContext _context) : base(_context)
    {
    }
}
