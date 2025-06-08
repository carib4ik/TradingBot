using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TradingBot.Data;
using TradingBot.Extensions;

namespace TradingBot.StateMachine.States;

public class StartState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    
    public StartState(ChatStateMachine stateMachine, ITelegramBotClient botClient) : base(stateMachine)
    {
        _botClient = botClient;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
    
    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("StartState");
        
        var result = await _botClient.GetMyCommands();
        Console.WriteLine("Установленные команды:");
        foreach (var cmd in result)
            Console.WriteLine($"/{cmd.Command} — {cmd.Description}");

        await SendGreeting(chatId);
    }

    private async Task SendGreeting(long chatId)
    {
        var greetings = "\ud83e\udd16 Crypto Insight Bot\nТвой помощник по аналитике рынка и экономики.\n\ud83d\udcca Задай вопрос — получи ответ от ИИ.\n\ud83d\udca1 Получай краткосрочные сделки по топовым криптомонетам.\n\ud83d\udcc8 Автооповещения о зонах перекупленности/перепроданности (RSI).";
        
        var askButton = InlineKeyboardButton.WithCallbackData("Задать вопрос ИИ", GlobalData.ASK);
        var marketDataButton = InlineKeyboardButton.WithCallbackData("Получить точку входа", GlobalData.MARKET_DATA);
        var positionsButton = InlineKeyboardButton.WithCallbackData("Посмотреть открытые позиции", GlobalData.POSITIONS);
        var rsiButton = InlineKeyboardButton.WithCallbackData("Текущий BTCUSDT RSI", GlobalData.RSI);
        
        var keyboard = new InlineKeyboardMarkup([[askButton], [marketDataButton], [positionsButton], [rsiButton]]);
        
        await _botClient.SendMessage(chatId, greetings.EscapeMarkdownV2(), replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
}