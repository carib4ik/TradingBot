using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TradingBot.Data;
using TradingBot.Extensions;
using TradingBot.Services;

namespace TradingBot.StateMachine.States;

public class AskState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly ChatGptService _chatGptService;

    public AskState(ChatStateMachine stateMachine, ITelegramBotClient botClient, ChatGptService chatGptService) : base(stateMachine)
    {
        _botClient = botClient;
        _chatGptService = chatGptService;
    }

    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        var question = message.Text;
        
        await _botClient.SendChatAction(chatId, ChatAction.Typing);
        
        var answer = await _chatGptService.GetAnswerFromChatGpt(question);
        
        await _botClient.SendMessage(chatId, answer);
        await StateMachine.TransitTo<IdleState>(chatId);
    }

    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("AskState");
        
        const string response = "Пожалуйста введите свой вопрос. Ответ может занять некоторое время";
        
        var backButton = InlineKeyboardButton.WithCallbackData("Назад", GlobalData.START);
        
        var keyboard = new InlineKeyboardMarkup(new[] { backButton });

        await _botClient.SendMessage(chatId, response.EscapeMarkdownV2()!, replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
    }
}