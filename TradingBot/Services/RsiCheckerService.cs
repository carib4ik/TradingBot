using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TradingBot.Data;

namespace TradingBot.Services;

public class RsiCheckerService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly MarketDataService _marketDataService;
    private readonly TimeSpan _checkingInterval = TimeSpan.FromMinutes(20);
    private readonly UsersDataProvider _usersDataProvider;
    private const string ChartInterval = "1h";
    private const double MaxRsiAlert = 70;
    private const double MinRsiAlert = 30;

    public RsiCheckerService(ITelegramBotClient botClient, MarketDataService marketDataService, UsersDataProvider usersDataProvider)
    {
        _botClient = botClient;
        _marketDataService = marketDataService;
        _usersDataProvider = usersDataProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tokens = LoadTokensList();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Auto checking RSI");
            
            var chatIds = _usersDataProvider.LoadChatIds();

            foreach (var token in tokens)
            {
                token.Rsi = await _marketDataService.GetCurrentRsi(token.Symbol, ChartInterval);
            }

            foreach (var token in tokens)
            {
                if (token.Rsi >= MaxRsiAlert)
                {
                    foreach (var chatId in chatIds)
                    {
                        await _botClient.SendMessage(
                            chatId:chatId,
                            text: $"\ud83d\udd34 SHORT signal\n{token.Symbol} ({ChartInterval})\nRSI: {token.Rsi:F1}", 
                            cancellationToken: stoppingToken);
                    }
                }
                if (token.Rsi <= MinRsiAlert)
                {
                    foreach (var chatId in chatIds)
                    {
                        await _botClient.SendMessage(
                            chatId:chatId,
                            text: $"\ud83d\udfe2 LONG signal\n{token.Symbol} ({ChartInterval})\nRSI: {token.Rsi:F1}", 
                            cancellationToken: stoppingToken);
                    }
                }
            }
            
            await Task.Delay(_checkingInterval, stoppingToken);
        }
    }

    private List<TokenData> LoadTokensList()
    {
        return
        [
            new TokenData { Symbol = GlobalData.BTC },
            new TokenData { Symbol = GlobalData.ETH },
            new TokenData { Symbol = GlobalData.SOL },
            new TokenData { Symbol = GlobalData.ADA },
            new TokenData { Symbol = GlobalData.AVAX },
            new TokenData { Symbol = GlobalData.LTC },
            new TokenData { Symbol = GlobalData.TON },
            new TokenData { Symbol = GlobalData.SUI },
            new TokenData { Symbol = GlobalData.XRP },
            new TokenData { Symbol = GlobalData.NEAR },
            new TokenData { Symbol = GlobalData.INJ },
            new TokenData { Symbol = GlobalData.AAVE },
            new TokenData { Symbol = GlobalData.LINK },
        ];
    }
}