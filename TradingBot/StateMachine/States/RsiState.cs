using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TradingBot.Services;

namespace TradingBot.StateMachine.States;

public class RsiState : ChatStateBase
{
    private readonly MarketDataService _marketDataService;
    private readonly ChatStateMachine _stateMachine;
    private readonly ITelegramBotClient _botClient;

    public RsiState(ChatStateMachine stateMachine, ITelegramBotClient botClient,
        MarketDataService marketDataService) : base(stateMachine)
    {
        _marketDataService = marketDataService;
        _stateMachine = stateMachine;
        _botClient = botClient;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
    
    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("RsiState");

        var cancellationTokenSource = new CancellationTokenSource();
        await _botClient.SendChatAction(chatId, ChatAction.Typing, cancellationToken: cancellationTokenSource.Token);
        
        try
        {
            var currentRsi = await _marketDataService.GetCurrentRsi("BTCUSDT", "1h");

            await _botClient.SendMessage(chatId, $"BTCUSDT RSI: {currentRsi:F1}", cancellationToken: cancellationTokenSource.Token);
        }
        finally
        {
            await cancellationTokenSource.CancelAsync();
            await _stateMachine.TransitTo<IdleState>(chatId);
        }
    }
}