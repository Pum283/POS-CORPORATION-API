using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BackgroundJobs
{
    public class AutoInventoryPeriodLockJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AutoInventoryPeriodLockJob(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            try
            {
                var stores = await _unitOfWork.Repository<StoreEntity>().GetQueryable().ToListAsync();


                foreach (var store in stores)
                {
                    var storeId = store.Id;

                    // Lấy InventoryPeriodLocks của ngày trước  //yesterdayLocks được lấy theo thứ tự giảm dần của CreateDate, tức là bản ghi mới nhất trước đó sẽ đứng đầu.
                    var yesterdayLocks = await _unitOfWork.Repository<InventoryPeriodLockEntity>()
                        .GetQueryable()
                        .Where(x => x.StoreId == storeId)
                        .OrderByDescending(x => x.CreateDate)
                        .ToListAsync();

                    // Lấy inventory hiện tại
                    var inventoryProducts = await _unitOfWork.Repository<InventoryEntity>()
                        .GetListByConditionAsync(x => x.StoreId == storeId && x.ProductId != Guid.Empty);

                    var inventoryIngredients = await _unitOfWork.Repository<IngredientInventoryEntity>()
                        .GetListByConditionAsync(x => x.StoreId == storeId && x.IngredientId != Guid.Empty);

                    var lockEntries = new List<InventoryPeriodLockEntity>();

                    // Xử lý product
                    foreach (var p in inventoryProducts)
                    {
                        var previousLock = yesterdayLocks
                            .FirstOrDefault(x => x.ProductId == p.ProductId);

                        lockEntries.Add(new InventoryPeriodLockEntity
                        {
                            Id = Guid.NewGuid(),
                            StoreId = storeId,
                            ProductId = p.ProductId,
                            //OpeningQuantity = previousLock?.ClosingQuantity ?? p.Quantity,
                            OpeningQuantity = previousLock?.ClosingQuantity ?? 0, // ⚡ nếu không có lock trước đó => 0
                            ClosingQuantity = p.Quantity,
                            EditStatus = InventoryEditStatusType.NotEdited
                        });
                    }

                    // Xử lý ingredient
                    foreach (var i in inventoryIngredients)
                    {
                        var previousLock = yesterdayLocks
                            .FirstOrDefault(x => x.IngredientId == i.IngredientId);

                        lockEntries.Add(new InventoryPeriodLockEntity
                        {
                            Id = Guid.NewGuid(),
                            StoreId = storeId,
                            IngredientId = i.IngredientId,
                            //OpeningQuantity = previousLock?.ClosingQuantity ?? i.Quantity,
                            OpeningQuantity = previousLock?.ClosingQuantity ?? 0, // ⚡ nếu không có lock trước đó => 0
                            ClosingQuantity = i.Quantity,
                            EditStatus = InventoryEditStatusType.NotEdited
                        });
                    }

                    await _unitOfWork.Repository<InventoryPeriodLockEntity>().AddRangeAsync(lockEntries);
                }

                await _unitOfWork.SaveChangesAsync();
                Console.WriteLine($"[{DateTime.Now}] AutoInventoryPeriodLockJob finished");


                var emailContent = new EmailContent
                {
                    To = "luongtnd.info@gmail.com",
                    Subject = "Đã lưu cuối ngày cho POS Corporation",
                    Body = "Đã lưu cuối ngày cho POS Corporation"
                };

                await _emailService.SendMailAsync(emailContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ AutoInventoryPeriodLockJob error: {ex}");
            }
        }
    }
}
