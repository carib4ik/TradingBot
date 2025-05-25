using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TradingBot.Data;
using TradingBot.Extensions;

namespace TradingBot.StateMachine.States;

public class MarketDataState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;

    public MarketDataState(ChatStateMachine stateMachine, ITelegramBotClient botClient) : base(stateMachine)
    {
        _botClient = botClient;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
    
    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("MarketDataState");
        
        const string response = "Выберите монету. Ответ может занять некоторое время";
        
        var btcButton = InlineKeyboardButton.WithCallbackData("BTC", GlobalData.BTC);
        var ethButton = InlineKeyboardButton.WithCallbackData("ETH", GlobalData.ETH);
        var solButton = InlineKeyboardButton.WithCallbackData("SOL", GlobalData.SOL);
        var backButton = InlineKeyboardButton.WithCallbackData("Назад", GlobalData.START);
        
        var keyboard = new InlineKeyboardMarkup([[btcButton, ethButton, solButton], [backButton]]);

        await _botClient.SendMessage(chatId, response.EscapeMarkdownV2()!, replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
    }
}