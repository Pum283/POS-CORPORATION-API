namespace Application;

public interface IUsersService
{
    Task<QueryResult<User>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<UserForTokenModel> GetByIdForRefreshTokenAsync(Guid id);
    Task<UserModel> GetByIdAsync(Guid id);
    Task<List<UserModel>> GetUsersByRoleAsync(string roleName);
    Task<User> LoginAsync(UserLoginModel model);
    //Task<QueryResult<User>> GetByPageAsync(QueryInfo queryInfo);
    Task<OperationResult> CreateAsync(CreateUserModel model);
    Task<OperationResult> UpdateAsync(Guid id, UpdateUserModel model);
    Task<OperationResult> UpdateUserByAdminAsync(Guid userId, UpdateUserByAdminModel model);
    Task UpdateRefreshTokenAsync(Guid id, string refreshToken);
    Task<OperationResult> BlockOrUnblockAsync(BlockUserModel model);
}
