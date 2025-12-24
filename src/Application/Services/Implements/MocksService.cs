namespace Application;

[RegisterService(typeof(IMocksService))]
public class MocksService : IMocksService
{
    private readonly EfApplicationDbContext _context;

    public MocksService(EfApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<OperationResult> CreateMockDocumentAsync(List<MockUpdateInventoryModel> rows, Guid storeId)
    {
        // Bước 1: Load danh sách mã Product và Ingredient
        var productCodes = (await _context.Products
            .Select(p => p.SKU.ToLower().Trim())
            .ToListAsync())
            .ToHashSet();

        var ingredientCodes = (await _context.Ingredients
            .Select(i => i.Code.ToLower().Trim())
            .ToListAsync())
            .ToHashSet();

        var unknownRows = new List<MockUpdateInventoryModel>();

        // Bước 2: Duyệt từng dòng
        foreach (var row in rows)
        {
            var code = row.Code.Trim().ToLower();
            if (productCodes.Contains(code))
            {
                var product = await _context.Products.FirstAsync(p => p.SKU.ToLower() == code);
                await ImportProductInventory(product.Id, storeId, row.Quantity);
            }
            else if (ingredientCodes.Contains(code))
            {
                var ingredient = await _context.Ingredients.FirstAsync(i => i.Code.ToLower() == code);
                await ImportIngredientInventory(ingredient.Id, storeId, row.Quantity);
            }
            else
            {
                unknownRows.Add(row);
            }
        }

        await _context.SaveChangesAsync();

        return OperationResult.SuccessResult("Import thành công", new
        {
            Total = rows.Count,
            Imported = rows.Count - unknownRows.Count,
            Unknown = unknownRows
        });
    }
    private async Task ImportProductInventory(Guid productId, Guid storeId, decimal quantity)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.StoreId == storeId);

        if (inventory == null)
        {
            _context.Inventories.Add(new InventoryEntity
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                ProductId = productId,
                Quantity = quantity,
                CreateDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            });
        }
        else
        {
            inventory.Quantity += quantity;
        }
    }

    private async Task ImportIngredientInventory(Guid ingredientId, Guid storeId, decimal quantity)
    {
        var inventory = await _context.IngredientInventories
            .FirstOrDefaultAsync(x => x.IngredientId == ingredientId && x.StoreId == storeId);

        if (inventory == null)
        {
            _context.IngredientInventories.Add(new IngredientInventoryEntity
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                IngredientId = ingredientId,
                Quantity = quantity,
                CreateDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            });
        }
        else
        {
            inventory.Quantity += quantity;
        }
    }
}
