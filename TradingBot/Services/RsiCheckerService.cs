using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TradingBot.Data;

namespace TradingBot.Services;

public class RsiCheckerService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly MarketDataService _marketDataService;
    private readonly TimeSpan _checkingInterval = TimeSpan.FromMinutes(30);
    private readonly UsersDataProvider _usersDataProvider;

    private readonly List<TokenData> _tokens;
    private const string ChartInterval = "1h";
    private const double MaxRsiAlert = 70;
    private const double MinRsiAlert = 30;

    public RsiCheckerService(ITelegramBotClient botClient, MarketDataService marketDataService, UsersDataProvider usersDataProvider)
    {
        _botClient = botClient;
        _marketDataService = marketDataService;
        _usersDataProvider = usersDataProvider;

        _tokens = LoadTokensList();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Auto checking RSI");
            
            var alertMessage = "";
            
            foreach (var token in _tokens)
            {
                token.Rsi = await _marketDataService.GetCurrentRsi(token.Symbol, ChartInterval);
            }

            foreach (var token in _tokens)
            {
                switch (token.Rsi)
                {
                    case >= MaxRsiAlert:
                        alertMessage += $"\ud83d\udd34 SHORT\n{token.Symbol} ({ChartInterval}) RSI: {token.Rsi:F1}\n\n";
                        break;
                    case <= MinRsiAlert:
                        alertMessage += $"\ud83d\udfe2 LONG\n{token.Symbol} ({ChartInterval}) RSI: {token.Rsi:F1}\n\n";
                        break;
                }
            }
            
            foreach (var chatId in _usersDataProvider.LoadChatIds())
            {
                await _botClient.SendMessage(chatId, alertMessage, cancellationToken: stoppingToken);
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
            new TokenData { Symbol = GlobalData.AAVE },
            new TokenData { Symbol = GlobalData.LINK },
        ];
    }
}