namespace WidgetBridge;

public class TelegramUpdate
{
	public TelegramMessage? message { get; set; }
	public TelegramMessage? edited_message { get; set; }
}

public class TelegramMessage
{
	public long message_id { get; set; }
	public TelegramChat? chat { get; set; }
	public string? text { get; set; }
	public int date { get; set; }
}

public class TelegramChat
{
	public long id { get; set; }
	public string? type { get; set; }
}