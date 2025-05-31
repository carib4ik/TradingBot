using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TradingBot.Data;

namespace TradingBot;

public class TelegramBotController
{
    private readonly ITelegramBotClient _botClient;
    private readonly ChatStateController _chatStateController;

    public TelegramBotController(ITelegramBotClient telegramBotClient, ChatStateController chatStateController)
    {
        _botClient = telegramBotClient;
        _chatStateController = chatStateController;
    }

    public void StartBot()
    {
        using var cts = new CancellationTokenSource();
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery]
        };

        CreateCommandsKeyboard().WaitAsync(cts.Token);
        
        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);

        Console.WriteLine("Bot started");
    }

    private async Task CreateCommandsKeyboard()
    {
        await _botClient.DeleteMyCommands();

        var commands = new[]
        {
            new BotCommand { Command = GlobalData.START, Description = "Запустить бота" },
            // new BotCommand { Command = GlobalData.MARKET_DATA, Description = "Получить точку входа" }
        };
        
        await _botClient.SetMyCommands(
            commands,
            scope: new BotCommandScopeDefault(),
            languageCode: "ru"
        );
        
        // await _botClient.SetMyCommands(commands);
    }
    
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var requestException = exception switch
        {
            ApiRequestException apiRequestException => apiRequestException,
            _ => exception
        };

        Console.WriteLine("Произошла критическая ошибка. Требуется *ПЕРЕЗАПУСК* бота\n" + requestException.Message);
        return Task.CompletedTask;
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Update received: {update.Type}");
        
        if (update.Message == null && update.CallbackQuery == null)
        {
            return;
        }
        
        var message = update.Message;
        var callbackQuery = update.CallbackQuery;
        
        if (message != null && message.Type != MessageType.Text)
        {
            return;
        }
        
        var userId = message != null ? message.From.Id : callbackQuery.From.Id;
        var messageId = message != null ? message.MessageId : callbackQuery.Message.MessageId;
        var messageText = message != null ? message.Text : callbackQuery?.Data;
        // var chatId = message?.Chat.Id;
        
        // string response;

        if (messageText == GlobalData.START)
        {
            await DeleteMessageAsync(userId, messageId, cancellationToken);
        }
        
        await _chatStateController.HandleUpdateAsync(update);
    }

    
    private async Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.DeleteMessage(chatId, messageId, cancellationToken: cancellationToken);
        }
        catch (ApiRequestException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }
}