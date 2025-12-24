using Quartz;

namespace Application;

public class InventoryAlertJob : IJob
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IStoresRepository _storesRepository;
    private readonly IEmailService _emailService;
    private readonly IUsersRepository _usersRepository;

    public InventoryAlertJob(IDashboardRepository dashboardRepository, IEmailService emailService, IStoresRepository storesRepository, IUsersRepository usersRepository)
    {
        _dashboardRepository = dashboardRepository;
        _emailService = emailService;
        _storesRepository = storesRepository;
        _usersRepository = usersRepository;
    }

    //public async Task Execute(IJobExecutionContext context)
    //{
    //    var stores = await _storesRepository.GetAllAsync();
    //    foreach (var store in stores)
    //    {
    //        var lowStockItems = await _dashboardRepository.GetInventorySummaryReportAsync(store.Id);

    //        if (lowStockItems.TotalLowStockWarnings != null)
    //        {
    //            var message = "⚠️ Cảnh báo tồn kho thấp:\n" +
    //                          string.Join("\n", lowStockItems.TotalLowStockWarnings.Select(i => $"{i.Name}: {i.InventoryQuantity}")) +
    //                          string.Join("\n", store.Name +" - " + store.Address);

    //            var emailContent = new EmailContent
    //            {
    //                To = "vantu.dev@gmail.com",
    //                Subject = "Cảnh báo tồn kho thấp",
    //                Body = message
    //            };

    //            await _emailService.SendMailAsync(emailContent);
    //        }
    //    }
    //}
    public async Task Execute(IJobExecutionContext context)
    {

        try
        {
            var stores = await _storesRepository.GetAllAsync();

            foreach (var store in stores)
            {
                // Lấy danh sách user thuộc store này
                var storeUsers = await _usersRepository.GetUsersByStoreIdAsync(store.Id);
                if (storeUsers == null || !storeUsers.Any())
                    continue; // nếu store chưa có user thì bỏ qua

                var lowStockItems = await _dashboardRepository.GetInventorySummaryReportAsync(store.Id);

                // Nếu có cảnh báo tồn kho thấp
                if (lowStockItems.TotalLowStockWarnings != null && lowStockItems.TotalLowStockWarnings.Any())
                {
                    //var message = "⚠️ Cảnh báo tồn kho thấp tại cửa hàng: " + store.Name + "\n" +
                    //              string.Join("\n", lowStockItems.TotalLowStockWarnings.Select(i => $"{i.Name}: {i.InventoryQuantity}")) +
                    //              $"\nĐịa chỉ: {store.Address}";


                    // Tạo bảng HTML
                    var tableRows = string.Join("", lowStockItems.TotalLowStockWarnings.Select(i =>
                        $"<tr><td style='padding:5px;border:1px solid #ddd'>{i.Name}</td>" +
                        $"<td style='padding:5px;border:1px solid #ddd;text-align:right'>{i.InventoryQuantity}</td></tr>"));

                    var message = $@"
                <p>⚠️ Cảnh báo tồn kho thấp tại cửa hàng: <strong>{store.Name}</strong></p>
                <p>Địa chỉ: {store.Address}</p>
                <table style='border-collapse:collapse;width:100%;border:1px solid #ddd'>
                    <thead>
                        <tr>
                            <th style='padding:5px;border:1px solid #ddd;text-align:left'>Sản phẩm</th>
                            <th style='padding:5px;border:1px solid #ddd;text-align:right'>Tồn kho</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tableRows}
                    </tbody>
                </table>";

                    foreach (var user in storeUsers)
                    {
                        if (string.IsNullOrWhiteSpace(user.Email))
                            continue;

                        var emailContent = new EmailContent
                        {
                            To = user.Email,
                            Subject = $"Cảnh báo tồn kho thấp - {store.Name}",
                            Body = message
                        };

                        await _emailService.SendMailAsync(emailContent);
                    }
                }
            }

            Console.WriteLine($"[{DateTime.Now}] InventoryAlertJob finished");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ InventoryAlertJob error: {ex}");
        }
    }
}
