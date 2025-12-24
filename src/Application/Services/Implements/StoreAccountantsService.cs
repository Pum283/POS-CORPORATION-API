namespace Application;

[RegisterService(typeof(IStoreAccountantsService))]
public class StoreAccountantsService(
    IStoreAccountantsRepository _repository,
    IUnitOfWork _unitOfWork) : IStoreAccountantsService
{
    public async Task<QueryResult<StoreAccountListModel>> GetStoreAccountantsWithPagingAsync(QueryInfo queryInfo)
    {
        return await _repository.GetStoreAccountantsWithPagingAsync(queryInfo);
    }
    public async Task<QueryResult<StoreAccountantModel>> GetStoreOfAccountantsWithPagingAsync(QueryInfo queryInfo, Guid accountantId)
    {
        var result = await _repository.GetStoreOfAccountantsWithPagingAsync(queryInfo, accountantId);
        if(result.Data.Count() == 0)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(StoreAccountantModel)));

        return result;
    }

    public async Task<List<StoreAccountantModel>?> GetStoresOfAccountantById(Guid accountantId)
    {
        return await _repository.GetStoresOfAccountantByIdAsync(accountantId);
    }

    public async Task<OperationResult> CreateStoreAccountantAsync(CreateOrUpdateStoreAccountantModel model)
    {
        var repository = _unitOfWork.Repository<StoreAccountantEntity>();

        foreach (var storeId in model.StoreId)
        {
            var exists = await repository.CheckExistAsync(x =>
                x.AccountantId == storeId && x.StoreId == model.AccountantId);

            if (exists)
            {
                return OperationResult.FailureResult(
                    ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(StoreAccountantEntity)));
            }

            var entity = new StoreAccountantEntity
            {
                Id = Guid.NewGuid(),
                AccountantId = model.AccountantId,
                StoreId = storeId
            };
            await repository.AddAsync(entity);
        }

        var result = await _unitOfWork.SaveChangesAsync();

        if (!result)
        {
            return OperationResult.FailureResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(StoreAccountantEntity)));
        }

        return OperationResult.SuccessResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(StoreAccountantEntity)));
    }


    public async Task<OperationResult> UpdateStoreAccountantAsync(Guid accountantId, CreateOrUpdateStoreAccountantModel model)
    {
        var repository = _unitOfWork.Repository<StoreAccountantEntity>();

        var currentEntities = await repository.FindByAsync(x => x.AccountantId == accountantId);
        var currentStoreIds = currentEntities.Select(x => x.StoreId).ToHashSet();

        var newStoreIds = model.StoreId.ToHashSet();

        var toRemove = currentEntities
            .Where(x => !newStoreIds.Contains(x.StoreId))
            .ToList();

        var toAdd = newStoreIds
            .Where(storeId => !currentStoreIds.Contains(storeId))
            .Select(storeId => new StoreAccountantEntity
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                AccountantId = model.AccountantId
            })
            .ToList();

        if (toRemove.Any())
            repository.DeleteRangeAsync(toRemove);

        if (toAdd.Any())
            await repository.AddRangeAsync(toAdd);

        var result = await _unitOfWork.SaveChangesAsync();

        if (!result)
        {
            return OperationResult.FailureResult(
                ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(StoreAccountantEntity)));
        }

        return OperationResult.SuccessResult(
            ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(StoreAccountantEntity)));
    }


    public async Task<OperationResult> DeleteStoreAccountantAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<StoreAccountantEntity>().DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
        if (result == 0)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(StoreAccountantEntity)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(StoreAccountantEntity)));
    }

    public async Task<bool> HasStoreAccessAsync(Guid accountantId, Guid storeId)
    {
        return await _unitOfWork.Repository<StoreAccountantEntity>()
            .CheckExistAsync(sa => sa.AccountantId == accountantId && sa.StoreId == storeId);
    }
}
