namespace Infrastructure;

[RegisterService(typeof(IPromotionConditionsRepository))]
public class PromotionConditionsRepository(EfApplicationDbContext context) 
    : RepositoryBase<PromotionConditionEntity>(context), IPromotionConditionsRepository
{
    private readonly EfApplicationDbContext _context = context;
}
