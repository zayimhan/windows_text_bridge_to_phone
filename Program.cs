using Microsoft.AspNetCore.SignalR;
using WidgetBridge;

var builder = WebApplication.CreateBuilder(args);

// ENV'den alacağız
// TELEGRAM_WEBHOOK_SECRET=... (Telegram header doğrulama)
builder.Services.AddSignalR();

var app = builder.Build();

app.MapGet("/", () => "WidgetBridge OK");

// SignalR Hub
app.MapHub<InboxHub>("/hubs/inbox");

// Telegram webhook endpoint
app.MapPost("/telegram/webhook", async (
    HttpRequest request,
    TelegramUpdate update,
    IHubContext<InboxHub> hub
) =>
{
    // 1) Güvenlik: Telegram Secret Token doğrulama (önerilir)
    var expectedSecret = Environment.GetEnvironmentVariable("TELEGRAM_WEBHOOK_SECRET");
    if (!string.IsNullOrWhiteSpace(expectedSecret))
    {
        // Telegram bu header ile gönderir:
        // X-Telegram-Bot-Api-Secret-Token
        if (!request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var got) ||
            got.ToString() != expectedSecret)
        {
            return Results.Unauthorized();
        }
    }

    // 2) Mesajı yakala (message veya edited_message)
    var msg = update.message ?? update.edited_message;
    var chatId = msg?.chat?.id;
    var text = msg?.text;


    //Console.WriteLine($"Webhook geldi → ChatId: {chatId} | Text: {text}");

    // Text yoksa (fotoğraf vb.) ignore edebiliriz
    if (chatId == null || string.IsNullOrWhiteSpace(text))
        return Results.Ok();

    // 3) SignalR ile o chatId grubuna push
    var payload = new
    {
        chatId = chatId.Value.ToString(),
        text = text,
        messageId = msg!.message_id,
        unixDate = msg.date
    };

    await hub.Clients.Group(chatId.Value.ToString())
        .SendAsync("InboxMessage", payload);

    return Results.Ok();
});

app.Run();  