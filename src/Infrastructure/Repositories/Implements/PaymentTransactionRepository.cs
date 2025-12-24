namespace Infrastructure;

[RegisterService(typeof(IPaymentTransactionRepository))]
public class PaymentTransactionRepository(EfApplicationDbContext context) 
    : RepositoryBase<PaymentTransactionEntity>(context), IPaymentTransactionRepository
{
    private readonly EfApplicationDbContext _context = context;

}
