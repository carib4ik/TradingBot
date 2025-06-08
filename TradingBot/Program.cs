using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TradingBot.AppSettings;
using TradingBot.Services;
using TradingBot.StateMachine;

namespace TradingBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var config = LoadConfig();
        
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
    
    private static AppConfig LoadConfig()
    {
        return new AppConfig
        {
            TelegramBotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"),
            BybitKey = Environment.GetEnvironmentVariable("BYBIT_KEY"),
            BybitSecret = Environment.GetEnvironmentVariable("BYBIT_SECRET"),
            OpenAiKey = Environment.GetEnvironmentVariable("OPEN_AI_KEY"),
        };
    }
}