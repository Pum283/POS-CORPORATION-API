namespace Application;

[RegisterService(typeof(IOperationMetricsServices))]
public class OperationMetricsServices(
    IOperationMetricsRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IOperationMetricsServices
{
    public async Task<QueryResult<OperationMetricModel>> GetOperationMetricsAsync(QueryInfo queryInfo, OperationMetricQuery operationMetricQuery)
        => await _repository.GetOperationMetricsAsync(queryInfo, operationMetricQuery);
    public async Task<OperationMetricModel> GetOperationMetricByIdAsync(Guid id)
        => await _repository.GetOperationMetricByIdAsync(id);
    public async Task<OperationResult> CreateAsync(CreateOrUpdateOperationMetricModel model)
    {
        if (!await _unitOfWork.Repository<StoreEntity>().CheckExistByIdAsync(model.StoreId))
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(StoreEntity), model.StoreId));

        var entity = _mapper.Map<OperationMetricEntity>(model);
        await _unitOfWork.Repository<OperationMetricEntity>().AddAsync(entity);
        if(!await _unitOfWork.SaveChangesAsync())
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(OperationMetricEntity)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(OperationMetricEntity)));

    }
    public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateOperationMetricModel model)
    {
        var exsitingEntity = await _unitOfWork.Repository<OperationMetricEntity>().GetByIdAsync(id);
        _mapper.Map(model, exsitingEntity);
        _unitOfWork.Repository<OperationMetricEntity>().UpdateWithoutSave(exsitingEntity);
        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(OperationMetricEntity)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(OperationMetricEntity)));
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<OperationMetricEntity>().DeleteByIdAsync(id);
        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(OperationMetricEntity)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(OperationMetricEntity)));
    }
}
