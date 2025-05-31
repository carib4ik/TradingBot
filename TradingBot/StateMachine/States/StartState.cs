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
        var greetings = $"Бот может ответить на твой вопрос касательно рынка и экономики в целом, а так же предложить тебе краткосрочную сделку по основным крипто монетам";
        
        var askButton = InlineKeyboardButton.WithCallbackData("Спросить ChatGPT", GlobalData.ASK);
        var marketDataButton = InlineKeyboardButton.WithCallbackData("Получить точку входа", GlobalData.MARKET_DATA);
        
        var keyboard = new InlineKeyboardMarkup([[askButton], [marketDataButton]]);
        
        await _botClient.SendMessage(chatId, greetings.EscapeMarkdownV2(), replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
}