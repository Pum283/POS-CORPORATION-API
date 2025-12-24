namespace Application;

[RegisterService(typeof(IUsersService))]
internal class UsersService(
    IUsersRepository _repository,
    IMapper _mapper,
    IUnitOfWork _unitOfWork,
    IBlobStorageService _blob,
    IAppConfiguration _configuration,
    ILogger<UsersService> _logger)
    : IUsersService
{
    private readonly IMapper _mapper = _mapper;
    public async Task<QueryResult<User>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        return await _repository.GetAllWithPagingAsync(queryInfo);
    }
    public async Task<UserModel> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetUserByIdAsync(id);
        return result;
    }
    public async Task<List<UserModel>> GetUsersByRoleAsync(string roleName)
    {
        var roleExists = await _unitOfWork.Repository<RoleEntity>()
                                          .CheckExistAsync(r => r.Name == roleName);
        if (!roleExists)
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Role), roleName));
        }

        var users = await _repository.GetUsersByRoleAsync(roleName);

        if (users == null || !users.Any())
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), roleName));
        }

        return _mapper.Map<List<UserModel>>(users);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Repository<UserEntity>()
                                    .GetByConditionAsync(x => x.RefreshToken == refreshToken
                                                           && x.RefreshTokenExpiryTime > DateTime.Now.AddDays(7));

        return _mapper.Map<User?>(user);
    }
    public async Task<UserForTokenModel> GetByIdForRefreshTokenAsync(Guid id)
    {
        var user = await _repository.GetByIdForRefreshTokenAsync(id);
        if (user.IsDeleted)
            throw new BadRequestException("User was blocked");

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new TokenInvalidException("Refresh token expired");
        return user;
    }
    public async Task<User> LoginAsync(UserLoginModel model)
    {
        var user = await _unitOfWork.Repository<UserEntity>().GetByConditionAsync(x => x.Email == model.Email || x.Name == model.Email);
        if (user == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), model.Email));

        if (user.IsDeleted)
            throw new BadRequestException("User was blocked");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
        if (!isPasswordValid)
        {
            throw new BadRequestException(ErrorMessageBase.Format(ErrorMessageBase.InvalidFormat, nameof(user.Password)));
        }

        var result = _mapper.Map<User>(user);
        return result;
    }

    #region  Create User Cũ có check không cho tạo nhiều tài khoản trong một cửa hàng
    //public async Task<OperationResult> CreateAsync(CreateUserModel model)
    //{
    //    if (model.StoreId != null && await _unitOfWork.Repository<UserEntity>().CheckExistAsync(x => x.StoreId == model.StoreId))
    //    {
    //        return OperationResult.FailureResult(string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.StoreId));
    //    }

    //    if (await _unitOfWork.Repository<UserEntity>().CheckExistAsync(x => x.Email == model.Email))
    //    {
    //        return OperationResult.FailureResult(string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.Email));
    //    }

    //    if (await _unitOfWork.Repository<UserEntity>().CheckExistAsync(x => x.Name == model.Name))
    //    {
    //        return OperationResult.FailureResult(string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.Name));
    //    }

    //    model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

    //    var userEntity = _mapper.Map<UserEntity>(model);

    //    if (model.StoreId != null)
    //    {
    //        userEntity.StoreId = model.StoreId;
    //    }
    //    userEntity.IsDeleted = false;
    //    var result = await _unitOfWork.Repository<UserEntity>().AddAsync(userEntity);
    //    if (result == null)
    //        return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(User)));

    //    var role = await _unitOfWork.Repository<RoleEntity>().GetByConditionAsync(x => x.Name == model.Role);
    //    if (role == null)
    //        throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Role), model.Role!));

    //    var userRole = new UserRoleEntity
    //    {
    //        Id = Guid.NewGuid(),
    //        UserId = result.Id,
    //        RoleId = role.Id
    //    };

    //    await _unitOfWork.Repository<UserRoleEntity>().AddAsync(userRole);
    //    await _unitOfWork.SaveChangesAsync();
    //    return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(User)));
    //}
    #endregion

    public async Task<OperationResult> CreateAsync(CreateUserModel model)
    {
        //// Kiểm tra nếu model có StoreId và đã tồn tại user trong store đó thì trả về lỗi
        //if (model.StoreId != null && await _unitOfWork.Repository<UserEntity>()
        //    .CheckExistAsync(x => x.StoreId == model.StoreId))
        //{
        //    // Nếu đã tồn tại, trả về kết quả thất bại với thông báo user đã tồn tại
        //    return OperationResult.FailureResult(
        //        string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.StoreId));
        //}

        // Kiểm tra email đã tồn tại hay chưa
        if (await _unitOfWork.Repository<UserEntity>()
            .CheckExistAsync(x => x.Email == model.Email))
        {
            return OperationResult.FailureResult(
                string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.Email));
        }

        // Kiểm tra tên người dùng đã tồn tại hay chưa
        if (await _unitOfWork.Repository<UserEntity>()
            .CheckExistAsync(x => x.Name == model.Name))
        {
            return OperationResult.FailureResult(
                string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.Name));
        }

        // Hash mật khẩu trước khi lưu vào DB để đảm bảo bảo mật
        model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // Map dữ liệu từ model sang entity
        var userEntity = _mapper.Map<UserEntity>(model);

        // Gán StoreId nếu có
        if (model.StoreId != null)
        {
            userEntity.StoreId = model.StoreId;
        }

        // Đảm bảo user vừa tạo chưa bị đánh dấu xóa
        userEntity.IsDeleted = false;

        // Thêm user mới vào repository
        var result = await _unitOfWork.Repository<UserEntity>().AddAsync(userEntity);

        // Nếu thêm thất bại, trả về lỗi
        if (result == null)
            return OperationResult.FailureResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(User)));

        // Lấy role từ database theo tên role được cung cấp
        var role = await _unitOfWork.Repository<RoleEntity>()
            .GetByConditionAsync(x => x.Name == model.Role);

        // Nếu role không tồn tại, ném ra ngoại lệ NotFound
        if (role == null)
            throw new NotFoundException(
                ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Role), model.Role!));

        // Tạo user-role mapping để liên kết user vừa tạo với role
        var userRole = new UserRoleEntity
        {
            Id = Guid.NewGuid(),  // Tạo Id mới cho mapping
            UserId = result.Id,   // UserId là id vừa tạo
            RoleId = role.Id      // RoleId là id role lấy từ DB
        };

        // Thêm mapping vào repository
        await _unitOfWork.Repository<UserRoleEntity>().AddAsync(userRole);

        // Lưu tất cả thay đổi vào database
        await _unitOfWork.SaveChangesAsync();

        // Trả về kết quả thành công
        return OperationResult.SuccessResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(User)));
    }


    public async Task<OperationResult> UpdateAsync(Guid id, UpdateUserModel model)
    {
        string? newBlobName = null;
        string? oldBlobName = null;

        try
        {
            _logger.LogInformation(LogMessageBase.Processing, nameof(User), id);

            var user = await _unitOfWork.Repository<UserEntity>().GetPurchaseOrderByIdAsync(id);
            if (user == null)
            {
                return OperationResult.FailureResult("User not found.");
            }

            if (model.Picture != null)
            {
                if (!string.IsNullOrEmpty(user.Picture))
                {
                    oldBlobName = user.Picture;
                }

                var blobResult = await _blob.UploadFileAsync(AppConstants.BlobContainer, model.Picture);
                newBlobName = blobResult;
            }

            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                if (newBlobName != null)
                {
                    user.Picture = newBlobName;
                }

                _mapper.Map(model, user);

                var result = await _unitOfWork.Repository<UserEntity>().UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                if (!result)
                {
                    throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)));
                }

                _logger.LogInformation(LogMessageBase.Updated, nameof(User), id);
            });

            if (!string.IsNullOrEmpty(oldBlobName))
            {
                await _blob.DeleteFileAsync(AppConstants.BlobContainer, oldBlobName);
            }

            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(User)));
        }
        catch (Exception ex)
        {
            if (newBlobName != null)
            {
                await _blob.DeleteFileAsync(AppConstants.BlobContainer, newBlobName);
            }

            _logger.LogError(ex, LogMessageBase.OperationFailed, nameof(User), id);
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)) + ": " + ex.Message);
        }
    }
    public async Task<OperationResult> UpdateUserByAdminAsync(Guid userId, UpdateUserByAdminModel model)
    {
        var user = await _unitOfWork.Repository<UserEntity>().GetByConditionAsync(x => x.Id == userId);
        if (user == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), userId));

        if (user.StoreId != model.StoreId)
        {
            user.StoreId = model.StoreId;
            _unitOfWork.Repository<UserEntity>().UpdateWithoutSave(user);
        }

        var role = await _unitOfWork.Repository<RoleEntity>().GetByConditionAsync(x => x.Id == model.RoleId);
        if (role == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Role), model.RoleId!));

        var userRole = await _unitOfWork.Repository<UserRoleEntity>().GetByConditionAsync(x => x.UserId == userId);

        if (userRole == null)
        {
            userRole = new UserRoleEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = role.Id
            };
            await _unitOfWork.Repository<UserRoleEntity>().AddAsync(userRole);
        }
        else if (userRole.RoleId != role.Id)
        {
            userRole.RoleId = role.Id;
            _unitOfWork.Repository<UserRoleEntity>().UpdateWithoutSave(userRole);
        }

        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(User)));
    }
    public async Task UpdateRefreshTokenAsync(Guid id, string refreshToken)
    {
        var jwtSettings = _configuration.GetJwtSetting();

        var user = await _unitOfWork.Repository<UserEntity>().GetPurchaseOrderByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));
        }
        if (user.RefreshToken is null)
        {
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            var resultLogout = await _unitOfWork.Repository<UserEntity>().UpdateAsync(user);
            if (!resultLogout)
            {
                throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)));
            }
        }
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpiration).AddHours(7);
        var result = await _unitOfWork.Repository<UserEntity>().UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        if (!result)
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)));
    }
    public async Task<OperationResult> BlockOrUnblockAsync(BlockUserModel model)
    {
        var user = await _unitOfWork.Repository<UserEntity>().GetByConditionAsync(x => x.Id == model.Id);
        if (user == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), model.Id));
        // true -> false -> true
        if (model.IsDeleted == false)
        {
            user.IsDeleted = false;
        }
        else
        {
            user.IsDeleted = true;
        }

        _unitOfWork.Repository<UserEntity>().UpdateWithoutSave(user);
        await _unitOfWork.SaveChangesAsync();
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(User)));
    }

}
