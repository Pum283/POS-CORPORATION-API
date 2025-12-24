namespace Application;

public class DocumentHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
   
        var storeId = httpContext?.Request.Query["storeId"].FirstOrDefault();

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
