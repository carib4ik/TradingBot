using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TradingBot.Services;

namespace TradingBot.StateMachine.States;

public class GetTradeState : ChatStateBase
{
    private readonly MarketDataService _marketDataService;
    private readonly ChatGptService _chatGptService;
    private readonly ChatStateMachine _stateMachine;
    private readonly ITelegramBotClient _botClient;

    public GetTradeState(ChatStateMachine stateMachine, ITelegramBotClient botClient, MarketDataService marketDataService, ChatGptService chatGptService) : base(stateMachine)
    {
        _marketDataService = marketDataService;
        _chatGptService = chatGptService;
        _stateMachine = stateMachine;
        _botClient = botClient;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId, string currency)
    {
        Console.WriteLine("GetTradeState");
        
        var cancellationTokenSource = new CancellationTokenSource();
        var typingTask = KeepTyping(chatId, cancellationTokenSource.Token);

        try
        {
            var chart = await _marketDataService.GetDataFromByBit(currency, "1h");
            var response = await _chatGptService.GetTradeFromChatGpt(chart);
            await _botClient.SendMessage(chatId, currency + "\n\n" + response, cancellationToken: cancellationTokenSource.Token);

            // отправить график:
            // var chart5m = await _marketDataService.GenerateChart(currency, "5m");
            // await using var stream = File.OpenRead(chart5m);
            // await _botClient.SendPhoto(chatId, InputFile.FromStream(stream, Path.GetFileName(chart5m)));
        }
        finally
        {
            await cancellationTokenSource.CancelAsync();
            await _stateMachine.TransitTo<IdleState>(chatId);
        }
    }
    
    private async Task KeepTyping(long chatId, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await _botClient.SendChatAction(chatId, ChatAction.Typing, cancellationToken: token);
                await Task.Delay(3000, token); // каждые 3 секунды
            }
        }
        catch (TaskCanceledException)
        {
            // Ожидаемое завершение — ничего делать не нужно
        }
    }
}