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
    private const string CHART_INTERVAL = "1h";
    private const double MAX_RSI_ALERT = 70;
    private const double MIN_RSI_ALERT = 30;

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
                token.Rsi = await _marketDataService.GetCurrentRsi(token.Symbol, CHART_INTERVAL);
            }

            foreach (var token in _tokens)
            {
                switch (token.Rsi)
                {
                    case >= MAX_RSI_ALERT:
                        alertMessage += $"\ud83d\udd34 SHORT\n{token.Symbol} ({CHART_INTERVAL}) RSI: {token.Rsi:F1}\n\n";
                        break;
                    case <= MIN_RSI_ALERT:
                        alertMessage += $"\ud83d\udfe2 LONG\n{token.Symbol} ({CHART_INTERVAL}) RSI: {token.Rsi:F1}\n\n";
                        break;
                }
            }

            if (alertMessage != "")
            {
                foreach (var chatId in _usersDataProvider.LoadChatIds())
                {
                    await _botClient.SendMessage(chatId, alertMessage, cancellationToken: stoppingToken);
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
        ];
    }
}
