namespace Application;

public static class AutoMapperProfiles
{
    public static IMapper Initial()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UsersMappingProfile>();
            cfg.AddProfile<ProductsMappingProfile>();
            cfg.AddProfile<ProductGroupsMappingProfile>();
            cfg.AddProfile<ProducCategoriesMappingProfile>();
            cfg.AddProfile<PurchaseOrdersMappingProfile>();
            cfg.AddProfile<SalesOrdersMappingProfile>();
            cfg.AddProfile<PromotionsMappingProfile>();
            cfg.AddProfile<PromotionStoresMappingProfile>();
            cfg.AddProfile<InventoriesMappingProfile>();
            cfg.AddProfile<StoresMappingProfile>();
            cfg.AddProfile<RolesMappingProfile>();
            cfg.AddProfile<IngredientsMappingProfile>();
            cfg.AddProfile<InventoryPeriodLocksMappingProfile>();
            cfg.AddProfile<IngredientInventoriesMappingProfile>();
            cfg.AddProfile<PeriodLockMappingProfile>();
            cfg.AddProfile<InventoryPeriodLockMappingProfile>();
            cfg.AddProfile<CustomersMappingProfile>();
            cfg.AddProfile<LoyaltyPointsMappingProfile>();
            cfg.AddProfile<StoreAccountantsMappingProfile>();
            cfg.AddProfile<OperationMetricsMappingProfile>();
            cfg.AddProfile<LoyaltyPointConfigsMappingProfile>();
        });

        return mapperConfiguration.CreateMapper();
    }
}
