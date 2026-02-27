using Microsoft.AspNetCore.SignalR;

namespace WidgetBridge;

public class InboxHub : Hub
{
    // Widget bağlanınca chatId gönderip kendi grubuna join olacak
    public Task Join(string chatId)
        => Groups.AddToGroupAsync(Context.ConnectionId, chatId);
}