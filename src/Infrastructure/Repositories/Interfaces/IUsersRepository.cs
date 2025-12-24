namespace Infrastructure;

public interface IUsersRepository : IRepositoryBase<UserEntity>
{
    Task<QueryResult<User>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<UserForTokenModel?> GetUserPermissionsAsync(Guid id);
    Task<User> GetByRefreshTokenAsync(string refreshToken);
    Task<UserForTokenModel> GetByIdForRefreshTokenAsync(Guid id);
    Task<UserModel> GetUserByIdAsync(Guid id);
    Task<List<UserModel>> GetUsersByRoleAsync(string roleName);
    Task<List<UserEntity>> GetUsersByStoreIdAsync(Guid storeId);
}
