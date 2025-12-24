namespace Application;

[RegisterService(typeof(IIngredientInventoriesService))]
public class IngredientInventoriesService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IIngredientInventoriesService
{
    public async Task<OperationResult> UpdateAsync(Guid id, UpdateInventoryIngredientModel model)
    {
        var ingredient = await _unitOfWork.Repository<IngredientInventoryEntity>().GetByIdAsync(id);
        _mapper.Map(model, ingredient);
        var result = await _unitOfWork.Repository<IngredientInventoryEntity>().UpdateAsync(ingredient);
        if (await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(IngredientInventoryEntity)));
        }
        return OperationResult.FailureResult(
            ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(IngredientInventoryEntity)));
    }
}
