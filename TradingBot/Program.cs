using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        
        // var botClient = new TelegramBotClient(config.TelegramBotToken);
        //
        // var usersDataProvider = new UsersDataProvider();
        // var marketDataService = new MarketDataService(config.BybitKey, config.BybitSecret);
        // var rsiCheckerService = new RsiCheckerService(botClient, marketDataService);
        // var chatGptService = new ChatGptService(config.OpenAiKey);
        //
        // var chatStateMachine = new ChatStateMachine(botClient, usersDataProvider, chatGptService, marketDataService);
        // var chatStateController = new ChatStateController(chatStateMachine);
        // var telegramBot = new TelegramBotController(botClient, chatStateController);
        //
        // telegramBot.StartBot();
        // await Task.Delay(Timeout.Infinite);
        
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Регистрируем конфиг
                services.AddSingleton(config);

                // Регистрация зависимостей через фабрики или напрямую
                services.AddSingleton<ITelegramBotClient>(_ =>
                    new TelegramBotClient(config.TelegramBotToken));

                services.AddSingleton<UsersDataProvider>();
                services.AddSingleton<MarketDataService>(_ =>
                    new MarketDataService(config.BybitKey, config.BybitSecret));
                services.AddSingleton<ChatGptService>(_ =>
                    new ChatGptService(config.OpenAiKey));

                services.AddSingleton<ChatStateMachine>();
                services.AddSingleton<ChatStateController>();
                services.AddSingleton<TelegramBotController>();

                // Фоновая служба
                services.AddHostedService<RsiCheckerService>();
            })
            .Build();

        // Запуск бота
        var bot = host.Services.GetRequiredService<TelegramBotController>();
        bot.StartBot();

        await host.RunAsync();
    }
    
    private static AppConfig? LoadConfig(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл конфигурации не найден: {filePath}");

        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<AppConfig>(json);
    }
}