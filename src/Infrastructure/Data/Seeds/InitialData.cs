namespace Infrastructure;

internal sealed class InitialData
{
    public static IEnumerable<RoleEntity> Roles =>
    new List<RoleEntity>
    {
        RoleEntity.Create(new Guid("2bc526a8-452e-499a-b848-bee5531fe1d2"), "Staff"),
        RoleEntity.Create(new Guid("8b79c8d2-f4d1-4e2d-8f9a-b9f978112d3b"), "Admin"),
        RoleEntity.Create(new Guid("d271e812-08b1-4ea5-b9d9-7c4f536e982c"), "Accountant"),
    };

    public static IEnumerable<UserEntity> Users =>
    new List<UserEntity>
    {
        UserEntity.Create(new Guid("5C235FA5-4AE7-4051-D5EB-08DD5B312E2F"), "POS Corporation", "staff@gmail.com", BCrypt.Net.BCrypt.HashPassword("!Abc123"), false),
        UserEntity.Create(new Guid("AFFF700D-999C-4305-BB76-0E5260BE66C6"), "Admin", "admin@gmail.com", BCrypt.Net.BCrypt.HashPassword("!Abc123"), false),
        UserEntity.Create(new Guid("39E7099D-B699-4F63-A98A-1125F34936B1"), "Accountant", "acc@gmail.com", BCrypt.Net.BCrypt.HashPassword("!Abc123"), false),
    };

    public static IEnumerable<UserRoleEntity> UserRoles =>
    new List<UserRoleEntity>
    {
        UserRoleEntity.Create(new Guid("75C5639A-33CD-4F8E-A5D4-5AA900C67AC0"), new Guid("5C235FA5-4AE7-4051-D5EB-08DD5B312E2F"), new Guid("2bc526a8-452e-499a-b848-bee5531fe1d2")),
        UserRoleEntity.Create(new Guid("B73C70A0-B247-43DE-8565-79672465DBB2"), new Guid("AFFF700D-999C-4305-BB76-0E5260BE66C6"), new Guid("8b79c8d2-f4d1-4e2d-8f9a-b9f978112d3b")),
        UserRoleEntity.Create(new Guid("35F905BE-A21E-43E4-B740-81054EE56F3E"), new Guid("39E7099D-B699-4F63-A98A-1125F34936B1"), new Guid("d271e812-08b1-4ea5-b9d9-7c4f536e982c"))
    };

    public static IEnumerable<StoreEntity> Stores =>
    new List<StoreEntity>
    {
        StoreEntity.Create(new Guid("68686868-6868-6868-6868-686868686868"), "POS Corporation", "966 Ngô Quyền", "0123456789")
    };

    public static IEnumerable<StoreAccountantEntity> StoreAccountants =>
    new List<StoreAccountantEntity>
    {
        StoreAccountantEntity.Create(new Guid("E3A5C2D1-8F4E-4D3A-8D6B-3F4E5A6B7C8D"), new Guid("68686868-6868-6868-6868-686868686868"), new Guid("39E7099D-B699-4F63-A98A-1125F34936B1"))
    };

    public static IEnumerable<ProductCategoryEntity> ProductCategories =>
    new List<ProductCategoryEntity>
    {
        ProductCategoryEntity.Create(new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c"), "Food"),
        ProductCategoryEntity.Create(new Guid("814a947c-4d27-487c-c6d5-08ddfe6264ee"), "Groceries")
    };
    public static IEnumerable<ProductGroupEntity> ProductGroups =>
    new List<ProductGroupEntity>
    {
        ProductGroupEntity.Create(new Guid("397d73e7-90ba-4e8a-b1ef-08ddf436b017"), "Bánh bao", new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c")),
        ProductGroupEntity.Create(new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c"), "Bánh mì", new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c")),
        ProductGroupEntity.Create(new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1b"), "Nước", new Guid("814a947c-4d27-487c-c6d5-08ddfe6264ee"))
    };
    public static IEnumerable<ProductEntity> Products =>
    new List<ProductEntity>
    {
    ProductEntity.Create(
        new Guid("6dda1e17-30ce-4cb9-84f3-21cb0f4acb0e"),
        new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c"),
        "BMT-001", "Bánh mì trứng", "Bánh mì trứng", "https://posblob686868.blob.core.windows.net/posbh/0d3525c4-0ff8-44b1-bfb2-95a72151fe24_product_image_1757492569254_6voevj.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("ad15292a-feef-4613-951d-ee671b3479f0"),
        new Guid("397d73e7-90ba-4e8a-b1ef-08ddf436b017"),
        "BBTT-001", "Bánh bao thịt trứng", "Bánh bao thịt trứng", "https://posblob686868.blob.core.windows.net/posbh/3dfee929-e6bc-444d-9123-e18122f583b8_product_image_1755911833660_7otivf.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("bd6ac720-40e1-4aa5-94a5-68458792bbac"),
        new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c"),
        "BMHQ-002", "Bánh mì heo quay", "Bánh mì heo quay", "https://posblob686868.blob.core.windows.net/posbh/486b091f-f27c-463e-867b-8d498a7cfddb_product_image_1758945754474_3232s8.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("da24203a-533b-4fd3-a122-a78673b5a98f"),
        new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c"),
        "BMC-003", "Bánh mì chả", "Bánh mì chả", "https://posblob686868.blob.core.windows.net/posbh/488f6b4f-39a1-4396-89cb-b8b4ac9d5d59_product_image_1755911782066_msyrm8.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("894c76db-abbf-4ffe-bfc3-1969c6498a79"),
        new Guid("397d73e7-90ba-4e8a-b1ef-08ddf436b017"),
        "BBT-002", "Bánh bao thịt", "Bánh bao thịt", "https://posblob686868.blob.core.windows.net/posbh/61174cfc-0529-4aac-930d-0a33825ac71b_product_image_1755911892552_3aq2va.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("07abcfff-21a9-442e-9e0f-c7cd56545a65"),
        new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1b"),
        "NN7-001", "Nước ngọt 7Up", "Nước ngọt 7Up", "https://posblob686868.blob.core.windows.net/posbh/659974f5-0435-44e3-b37e-dc840e1c61b7_product_image_1755912125176_w2789a.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("d64faa80-13cc-49b4-9a29-a60fe4e75de7"),
        new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c"),
        "BMGD-004", "Bánh mì giòn da", "Bánh mì giòn da", "https://posblob686868.blob.core.windows.net/posbh/716da545-912f-41fe-8713-932e692eb27c_product_image_1756094748809_qdyr68.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("e5afdec8-31ad-4a6e-92e9-24d9503c5e39"),
        new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1b"),
        "NNCC-002", "Nước ngọt Coca Cola", "Nước ngọt Coca Cola", "https://posblob686868.blob.core.windows.net/posbh/8a60e1c7-dd04-41a0-91fa-394280459f8c_product_image_1755912203621_fds9ui.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("55751a68-f4b4-470f-90a9-336f0f5095e7"),
        new Guid("6d2f3664-c276-422c-abc6-08ddfea8ed1c"),
        "BMBK-005", "Bánh mì bò kho", "Bánh mì bò kho", "https://posblob686868.blob.core.windows.net/posbh/d13d3871-eca7-431a-91c4-b4fe687f0b81_product_image_1757492617445_qfhxs2.jpg", 199.99m, 199.99m, UnitType.Loaf, true),

    ProductEntity.Create(
        new Guid("164a8a70-c7e5-4fb1-9e1b-bf1c3bd638ed"),
        new Guid("397d73e7-90ba-4e8a-b1ef-08ddf436b017"),
        "H-003", "Hambuger", "Hambuger", "https://posblob686868.blob.core.windows.net/posbh/d66bab3e-4480-4e93-b4f2-106104b17a52_product_image_1757492679957_1fyiel.jpg", 199.99m, 199.99m, UnitType.Loaf, true),
    };

    public static IEnumerable<PermissionEntity> Permissions =>
     new List<PermissionEntity>
     {
        PermissionEntity.Create(Guid.NewGuid(), "User", "Read"),
        PermissionEntity.Create(Guid.NewGuid(), "User", "Create"),
        PermissionEntity.Create(Guid.NewGuid(), "User", "Update"),
        PermissionEntity.Create(Guid.NewGuid(), "User", "Delete"),

        PermissionEntity.Create(Guid.NewGuid(), "Product", "Read"),
        PermissionEntity.Create(Guid.NewGuid(), "Product", "Create"),
        PermissionEntity.Create(Guid.NewGuid(), "Product", "Update"),
        PermissionEntity.Create(Guid.NewGuid(), "Product", "Delete")
     };
}
