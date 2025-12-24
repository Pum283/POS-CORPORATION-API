namespace Application;

public class OrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        //var httpContext = Context.GetHttpContext();
        //if (httpContext != null)
        //{
        //    var storeId = httpContext.Request.Query["storeId"].FirstOrDefault();
        //    await Groups.AddToGroupAsync(Context.ConnectionId, $"store-{storeId}");
        //}
        //await base.OnConnectedAsync();
        var httpContext = Context.GetHttpContext();
        var isDashboard = httpContext?.Request.Query["dashboard"].FirstOrDefault() == "true";
        var storeId = httpContext?.Request.Query["storeId"].FirstOrDefault();

        if (isDashboard)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard");
        }

        if (!string.IsNullOrEmpty(storeId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"store-{storeId}");
        }

        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        await base.OnDisconnectedAsync(ex);
    }
}
