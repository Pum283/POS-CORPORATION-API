namespace Application;

[RegisterService(typeof(ICustomersService))]
public class CustomersService(
    ICustomersRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : ICustomersService
{

    public async Task<QueryResult<Customer>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        return await _repository.GetAllWithPagingAsync(queryInfo);
    }

    public async Task<Customer> GetByIdAsync(Guid id)
    {
        var customer = await _repository.GetCustomerByIdAsync(id);
        return _mapper.Map<Customer>(customer);
    }

    public async Task<List<Customer>> GetByPhoneNumberAsync(string phoneNumber)
    {
        var customer = await _repository.GetByPhoneNumberAsync(phoneNumber);
        return _mapper.Map<List<Customer>>(customer);
    }
    public async Task<OperationResult> CreateAsync(CreateOrUpdateCustomerModel model)
    {
        var checkExsit = await _unitOfWork.Repository<CustomerEntity>().CheckExistAsync(x => x.Phone == model.Phone);
        if (checkExsit)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.Existed, nameof(Customer), model.Phone!));

        var customer = _mapper.Map<CustomerEntity>(model);
        var newCustomer =  await _unitOfWork.Repository<CustomerEntity>().AddAsync(customer);
        await _unitOfWork.Repository<LoyaltyPointEntity>().AddAsync(new LoyaltyPointEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = newCustomer.Id,
            Points = 0
        });
        if (await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Customer)));
        }
        return OperationResult.FailureResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Customer)));
    }
 
    public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateCustomerModel model)
    {
        var checkExsit = await _unitOfWork.Repository<CustomerEntity>().CheckExistAsync(x => x.Phone == model.Phone);
        if (checkExsit)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.Existed, nameof(Customer), model.Phone!));

        var customer = _mapper.Map<CustomerEntity>(model);
        await _unitOfWork.Repository<CustomerEntity>().UpdateAsync(customer);
        if (await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Customer)));
        }
        return OperationResult.FailureResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Customer)));
    }

    public async Task<OperationResult> CreateOrUpdateLoyaltyPointAsync(CreateLoyaltyPointModel model)
    {
        var repository = _unitOfWork.Repository<LoyaltyPointEntity>();

        //if (await repository.CheckExistAsync(x => x.CustomerId == model.CustomerId))
        //{
        //    throw new BadRequestException(string.Format(ErrorMessageBase.AlreadyExists, nameof(Customer), model.CustomerId));
        //}
        var existing = await repository.GetByConditionAsync(x => x.CustomerId == model.CustomerId);

        if (existing != null)
        {
            var newPoints = existing.Points + model.Points;
            existing.Points = newPoints < 0 ? 0 : newPoints;

            await repository.UpdateAsync(existing);
        }
        else
        {
            var newEntity = _mapper.Map<LoyaltyPointEntity>(model);
            newEntity.Points = newEntity.Points < 0 ? 0 : newEntity.Points;

            await repository.AddAsync(newEntity);
        }

        if (await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(LoyaltyPoint)));
        }

        return OperationResult.FailureResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(LoyaltyPoint)));
    }

}