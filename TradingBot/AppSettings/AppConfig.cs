using Newtonsoft.Json;

namespace TradingBot.AppSettings;

public class AppConfig
{
    public string TelegramBotToken { get; set; }
    public string BybitKey { get; set; }
    public string BybitSecret { get; set; }
    public string OpenAiKey { get; set; }
}