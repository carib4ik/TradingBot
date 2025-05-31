using Newtonsoft.Json;
using Telegram.Bot;
using TradingBot.AppSettings;
using TradingBot.Services;
using TradingBot.StateMachine;

namespace TradingBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var config = LoadConfig("AppSettings/Config.json");
        
        var botClient = new TelegramBotClient(config.TelegramBotToken);
        
        var usersDataProvider = new UsersDataProvider();
        var marketDataService = new MarketDataService(config.BybitKey, config.BybitSecret);
        var chatGptService = new ChatGptService(config.OpenAiKey);
        
        var chatStateMachine = new ChatStateMachine(botClient, usersDataProvider, chatGptService, marketDataService);
        var chatStateController = new ChatStateController(chatStateMachine);
        var telegramBot = new TelegramBotController(botClient, chatStateController);
        
        telegramBot.StartBot();
        await Task.Delay(Timeout.Infinite);
    }
    
    private static AppConfig LoadConfig(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл конфигурации не найден: {filePath}");

        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<AppConfig>(json);
    }
}