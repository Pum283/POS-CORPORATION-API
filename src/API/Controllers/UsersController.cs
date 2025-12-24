namespace API.Controllers;

[Route("users")]
[Authorize]
public class UsersController(
    IUsersService _usersService) : BaseApiController
{
    [HttpGet]
    [RequirePermissions("User", "Read")]
    public async Task<QueryResult<User>> GetUsersWithPaging([FromQuery] QueryInfo queryInfo)
    {
        return await _usersService.GetAllWithPagingAsync(queryInfo);
    }

    [HttpGet("{id}")]
    public async Task<UserModel> GetUserById(Guid id)
    {
        return await _usersService.GetByIdAsync(id);
    }

    [HttpGet("{roleName}/role")]
    public async Task<List<UserModel>> GetUsersByRole([FromRoute] string roleName)
    {
        return await _usersService.GetUsersByRoleAsync(roleName);
    }

    [HttpPost]
    [RequirePermissions("User:Create")]
    [ValidateModel]
    public async Task<OperationResult> CreateUser([FromBody] CreateUserModel model)
    {
        return await _usersService.CreateAsync(model);
    }

    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateUserProfile([FromRoute] Guid id, [FromForm] UpdateUserModel model)
    {
        return await _usersService.UpdateAsync(id, model);
    }

    [HttpPatch("{userId}/admin")]
    [RequirePermissions("User:Update")]
    [RequirePermissions("User:Create")]
    [ValidateModel]
    public async Task<OperationResult> UpdateUserByAdmin([FromRoute] Guid userId, [FromBody] UpdateUserByAdminModel model)
    {
        return await _usersService.UpdateUserByAdminAsync(userId, model);
    }
    [HttpPatch("block")]
    public Task<OperationResult> BlockOrUnblock([FromBody] BlockUserModel model)
    {
        return _usersService.BlockOrUnblockAsync(model);
    }
}
