namespace Application;

//[Authorize]
public class ProductHub(ILogger<ProductHub> _logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext != null)
        {
            var storeId = httpContext.Request.Query["storeId"].FirstOrDefault();
            _logger.LogInformation($"Client {Context.ConnectionId} connected to store-{storeId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"store-{storeId}");
        }
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        _logger.LogInformation($"Client {Context.ConnectionId} disconnected.");
        await base.OnDisconnectedAsync(ex);
    }
}
