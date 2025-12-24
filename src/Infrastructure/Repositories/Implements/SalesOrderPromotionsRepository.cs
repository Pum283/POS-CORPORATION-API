namespace Infrastructure;

[RegisterService(typeof(ISalesOrderPromotionsRepository))]
public class SalesOrderPromotionsRepository(EfApplicationDbContext context) 
    : RepositoryBase<SalesOrderPromotionEntity>(context), ISalesOrderPromotionsRepository
{
    private readonly EfApplicationDbContext _context = context;
}
