using Telegram.Bot;
using TradingBot.Services;
using TradingBot.StateMachine;

namespace TradingBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var botClient = new TelegramBotClient("");
        
        var usersDataProvider = new UsersDataProvider();
        var marketDataService = new MarketDataService();
        var chatGptService = new ChatGptService("");
        
        var chatStateMachine = new ChatStateMachine(botClient, usersDataProvider, chatGptService, marketDataService);
        var chatStateController = new ChatStateController(chatStateMachine);
        var telegramBot = new TelegramBotController(botClient, chatStateController);
        
        telegramBot.StartBot();
        await Task.Delay(Timeout.Infinite);

    }
}
