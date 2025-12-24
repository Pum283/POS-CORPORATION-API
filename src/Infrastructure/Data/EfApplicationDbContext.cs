namespace Infrastructure;

public class EfApplicationDbContext(DbContextOptions<EfApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RoleHierarchyEntity> RoleHierarchies { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<ProductGroupEntity> ProductGroups { get; set; }
    public DbSet<ProductCategoryEntity> ProductCategoies { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<IngredientEntity> Ingredients { get; set; }
    public DbSet<ProductIngredientEntity> ProductIngredients { get; set; }
    public DbSet<StoreEntity> Stores { get; set; }
    public DbSet<InventoryEntity> Inventories { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<SalesOrderEntity> SalesOrders { get; set; }
    public DbSet<SalesOrderItemEntity> SalesOrderItems { get; set; }
    public DbSet<PaymentTransactionEntity> PaymentTransactions { get; set; }
    public DbSet<PromotionEntity> Promotions { get; set; }
    public DbSet<PromotionConditionEntity> PromotionConditions { get; set; }
    public DbSet<SalesOrderPromotionEntity> SalesOrderPromotions { get; set; }
    public DbSet<SupplierEntity> Suppliers { get; set; }
    public DbSet<PurchaseOrderEntity> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItemEntity> PurchaseOrderItems { get; set; }
    public DbSet<LoyaltyPointEntity> LoyaltyPoints { get; set; }
    public DbSet<LoyaltyTransactionEntity> LoyaltyTransactions { get; set; }
    public DbSet<AuditLogEntity> AuditLogs { get; set; }
    public DbSet<SystemEntity> Systems { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<DocumentEntity> Documents { get; set; }
    public DbSet<DocumentSupplierEntity> DocumentSuppliers { get; set; }
    public DbSet<BankEntity> Banks { get; set; }
    public DbSet<IngredientInventoryEntity> IngredientInventories { get; set; }
    public DbSet<PeriodLockEntity> PeriodLocks { get; set; }
    public DbSet<PeriodLockNoteEntity> PeriodLockNotes { get; set; }
    public DbSet<InventoryPeriodLockEntity> InventoryPeriodLocks { get; set; }
    public DbSet<StoreAccountantEntity> StoreAccountants { get; set; }
    public DbSet<TimeSaleEntity> TimeSales { get; set; }
    public DbSet<PromotionProductEntity> PromotionProducts { get; set; }
    public DbSet<PromotionStoreEntity> PromotionStores { get; set; }
    public DbSet<PromotionProductGroupEntity> PromotionProductGroups { get; set; }
    public DbSet<OperationMetricEntity> OperationMetrics { get; set; }
    public DbSet<StoreTableSumEntity> StoreTableSums { get; set; }
    public DbSet<TableEntity> Tables { get; set; }
    public DbSet<SettingSystemEntity> SettingSystems { get; set; }
    public DbSet<FoodAppPlatformEntity> FoodAppPlatforms { get; set; } 
    public DbSet<FoodAppPriceEntity> FoodAppPrices { get; set; }
    public DbSet<LoyaltyPointConfigEntity> LoyaltyPointConfigEntities { get; set; }
    public DbSet<ProductPriceHistoryEntity> ProductPriceHistories { get; set; }



    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    //    base.OnModelCreating(modelBuilder);
    //}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<RoleHierarchyEntity>()
            .HasOne(rh => rh.ParentRole)
            .WithMany()
            .HasForeignKey(rh => rh.ParentRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoleHierarchyEntity>()
            .HasOne(rh => rh.ChildRole)
            .WithMany()
            .HasForeignKey(rh => rh.ChildRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserEntity>()
            .HasOne(e => e.Store)
            .WithMany()
            .HasForeignKey(e => e.StoreId);

        modelBuilder.Entity<IngredientEntity>()
        .HasIndex(i => i.Code)
        .IsUnique();

        modelBuilder.Entity<ProductEntity>()
        .HasIndex(i => i.SKU)
        .IsUnique();

        modelBuilder.Entity<IngredientInventoryEntity>()
       .HasIndex(i => new { i.StoreId, i.IngredientId })
       .IsUnique();

        modelBuilder.Entity<InventoryEntity>()
        .HasIndex(i => new { i.StoreId, i.ProductId })
        .IsUnique();

        modelBuilder.Entity<CustomerEntity>()
        .HasIndex(c => c.Phone)
        .IsUnique();

        modelBuilder.Entity<PromotionEntity>(b =>
        {
            b.ToTable("Promotions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Code).IsRequired().HasMaxLength(100);
            b.HasIndex(x => x.Code).IsUnique();

            b.Property(x => x.Name).IsRequired().HasMaxLength(255);
            b.Property(x => x.Type).HasConversion<int>();
            b.Property(x => x.Value).HasColumnType("decimal(18,2)");

            b.HasMany(x => x.PromotionProducts)
             .WithOne(x => x.Promotion)
             .HasForeignKey(x => x.PromotionId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.PromotionProductGroups)
             .WithOne(x => x.Promotion)
             .HasForeignKey(x => x.PromotionId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.PromotionStores)
             .WithOne(x => x.Promotion)
             .HasForeignKey(x => x.PromotionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PromotionProductEntity>(b =>
        {
            b.ToTable("PromotionProducts");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.PromotionId, x.ProductId }).IsUnique();

            b.HasOne(x => x.Product)
             .WithMany() 
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PromotionProductGroupEntity>(b =>
        {
            b.ToTable("PromotionProductGroups");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.PromotionId, x.ProductGroupId }).IsUnique();

            b.HasOne(x => x.ProductGroup)
             .WithMany() 
             .HasForeignKey(x => x.ProductGroupId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PromotionStoreEntity>(b =>
        {
            b.ToTable("PromotionStores");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.PromotionId, x.StoreId }).IsUnique();

            b.HasOne(x => x.Store)
             .WithMany() 
             .HasForeignKey(x => x.StoreId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SalesOrderPromotionEntity>(entity =>
        {
            entity.HasOne(p => p.SalesOrder)            
                  .WithMany(o => o.SalesOrderPromotion)  
                  .HasForeignKey(p => p.SalesOrderId)     
                  .OnDelete(DeleteBehavior.Cascade);     
        });

        // Cấu hình quan hệ SalesOrderPromotion <-> Promotion
        modelBuilder.Entity<SalesOrderPromotionEntity>(entity =>
        {
            entity.HasOne(p => p.Promotion)
                  .WithMany()                             
                  .HasForeignKey(p => p.PromotionId)
                  .OnDelete(DeleteBehavior.Restrict);    
        });

        modelBuilder.Entity<InventoryPeriodLockEntity>()
        .HasIndex(i => new { i.StoreId, i.ProductId, i.IngredientId, i.CreateDate })
        .IsUnique();

        base.OnModelCreating(modelBuilder);

        // FoodAppPlatform
        modelBuilder.Entity<FoodAppPlatformEntity>(entity =>
        {
            entity.ToTable("FoodAppPlatforms");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(x => x.Description)
                  .HasMaxLength(500);
            entity.Property(x => x.IsActive)
                  .IsRequired()
                  .HasDefaultValue(true);
            entity.HasIndex(x => x.Name)
                  .IsUnique()
                  .HasDatabaseName("IX_FoodAppPlatforms_Name");
        });

        // FoodAppPrice
        modelBuilder.Entity<FoodAppPriceEntity>(entity =>
        {
            entity.ToTable("FoodAppPrices");
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.FoodAppPlatform)
                  .WithMany(p => p.FoodAppPrices)
                  .HasForeignKey(x => x.FoodAppPlatformId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Product)
                  .WithMany()
                  .HasForeignKey(x => x.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.Property(x => x.OriginalPrice)
                  .HasColumnType("decimal(18,2)");
            entity.Property(x => x.FoodAppPrice)
                  .HasColumnType("decimal(18,2)");
            entity.Property(x => x.IsActive)
                  .IsRequired()
                  .HasDefaultValue(true);
            entity.HasIndex(x => new { x.FoodAppPlatformId, x.ProductId })
                  .IsUnique()
                  .HasDatabaseName("IX_FoodAppPrices_Platform_Product")
                  .HasFilter("\"IsActive\" = 1"); // Optional: chỉ unique khi còn active
        });

        // ProductPriceHistory
        modelBuilder.Entity<ProductPriceHistoryEntity>(entity =>
        {
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.ChangedByUserId);
        });
    }
}
