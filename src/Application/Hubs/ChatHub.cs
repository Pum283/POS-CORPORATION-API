namespace Application;

public class ChatHub : Hub
{
    public async Task SendMessage(UserConnection conn)
    {
        await Clients.All.SendAsync("ReceiveMessage", "admin", $"{conn.UserName} has joined");
    }

    public async Task JoinSpecificChatRoom(UserConnection conn)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);
        await Clients.Group(conn.ChatRoom).SendAsync("JoinSpecificChatRoom", "admin", $"{conn.UserName} has joined {conn.ChatRoom}");
    }
}
public class UserConnection
{
    public string UserName { get; set; } = string.Empty;
    public string ChatRoom { get; set; } = string.Empty;
}
