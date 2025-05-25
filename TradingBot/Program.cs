using Telegram.Bot;
using TradingBot.Services;
using TradingBot.StateMachine;

namespace TradingBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var botClient = new TelegramBotClient("7892283240:AAEboODXlIAmpA3FzNvAFpAjXV_AQ4p_3eM");
        
        var usersDataProvider = new UsersDataProvider();
        var marketDataService = new MarketDataService();
        var chatGptService = new ChatGptService("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");
        
        var chatStateMachine = new ChatStateMachine(botClient, usersDataProvider, chatGptService, marketDataService);
        var chatStateController = new ChatStateController(chatStateMachine);
        var telegramBot = new TelegramBotController(botClient, chatStateController);
        
        telegramBot.StartBot();
        await Task.Delay(Timeout.Infinite);

    }
}