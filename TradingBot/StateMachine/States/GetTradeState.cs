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
        
        await _botClient.SendChatAction(chatId, ChatAction.Typing);
        
        // var chart4H = await _marketDataService.GenerateChart("BTCUSDT", "4h");
        // var chart15M = await _marketDataService.GenerateChart("BTCUSDT", "15m");
        // var chart1M = await _marketDataService.GenerateChart("BTCUSDT", "1m");
        //
        // var response = await _chatGptService.GetTradeFromChatGpt([chart15M, chart1M]);

        var chart = await _marketDataService.GetData(currency, "3m");
        var response = await _chatGptService.GetTradeFromChatGpt(chart);
        
        await _botClient.SendMessage(chatId, response);
        // await using var stream = File.OpenRead(filePath);
        // await _botClient.SendPhoto(chatId, photo: InputFile.FromStream(stream, Path.GetFileName(filePath)));

        
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
}