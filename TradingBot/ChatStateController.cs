using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TradingBot.Data;
using TradingBot.StateMachine;
using TradingBot.StateMachine.States;

namespace TradingBot;

public class ChatStateController
{
    private readonly ChatStateMachine _stateMachine;

    public ChatStateController(ChatStateMachine chatStateMachine)
    {
        _stateMachine = chatStateMachine;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message == null && update.CallbackQuery == null)
        {
            return;
        }

        string? data;
        Message message;
        
        switch (update.Type)
        {
            case UpdateType.Message:
                data = update.Message.Text;
                message = update.Message;
                break;
            
            case UpdateType.CallbackQuery:
                data = update.CallbackQuery.Data;
                message = update.CallbackQuery.Message;
                break;
            
            default:
                return;
        }
        
        var chatId = message.Chat.Id;
        
        switch (data)
        {
            case GlobalData.START:
                await _stateMachine.TransitTo<StartState>(chatId);
                break;
            
            case GlobalData.ASK:
                await _stateMachine.TransitTo<AskState>(chatId);
                break;
            
            case GlobalData.MARKET_DATA:
                await _stateMachine.TransitTo<MarketDataState>(chatId);
                break;
            
            case GlobalData.POSITIONS:
                await _stateMachine.TransitTo<TrackPositionsState>(chatId);
                break;
            
            case GlobalData.RSI:
                await _stateMachine.TransitTo<RsiState>(chatId);
                break;
            
            case GlobalData.BTC:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.BTC);
                break;
            
            case GlobalData.ETH:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.ETH);
                break;
            
            case GlobalData.SOL:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.SOL);
                break;
            
            case GlobalData.ADA:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.ADA);
                break;
            
            case GlobalData.XRP:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.XRP);
                break;
            
            case GlobalData.LTC:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.LTC);
                break;
            
            case GlobalData.TON:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.TON);
                break;
            
            case GlobalData.SUI:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.SUI);
                break;
            
            case GlobalData.AVAX:
                await _stateMachine.TransitTo<GetTradeState>(chatId, GlobalData.AVAX);
                break;
            
            default:
                var state = _stateMachine.GetState(chatId);
                await state.HandleMessage(message);
                break;
        }
    }
}