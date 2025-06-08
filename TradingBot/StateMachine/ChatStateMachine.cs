using System.Collections.Concurrent;
using Telegram.Bot;
using TradingBot.Services;
using TradingBot.StateMachine.States;

namespace TradingBot.StateMachine;

public class ChatStateMachine
{
    private readonly ConcurrentDictionary<long, ChatStateBase> _chatStates = new();
    private readonly Dictionary<Type, Func<ChatStateBase>> _states = new();
    
    public ChatStateMachine(ITelegramBotClient botClient, UsersDataProvider usersDataProvider, ChatGptService chatGptService, MarketDataService marketDataState)
    {
        _states[typeof(IdleState)] = () => new IdleState(this);
        _states[typeof(StartState)] = () => new StartState(this, botClient);
        _states[typeof(AskState)] = () => new AskState(this, botClient, chatGptService);
        _states[typeof(MarketDataState)] = () => new MarketDataState(this, botClient);
        _states[typeof(GetTradeState)] = () => new GetTradeState(this, botClient, marketDataState, chatGptService);
        _states[typeof(TrackPositionsState)] = () => new TrackPositionsState(this, botClient, marketDataState);
        _states[typeof(RsiState)] = () => new RsiState(this, botClient, marketDataState);
    }
    
    public ChatStateBase GetState(long chatId)
    {
        return !_chatStates.TryGetValue(chatId, out var state) ? _states[typeof(IdleState)].Invoke() : state;
    }
    
    public async Task TransitTo<T>(long chatId) where T : ChatStateBase
    {
        if (_chatStates.TryGetValue(chatId, out var currentState))
        {
            await currentState.OnExit(chatId);
        }

        var newState = _states[typeof(T)]();
        _chatStates[chatId] = newState;
        await newState.OnEnter(chatId);
    }
    
    public async Task TransitTo<T>(long chatId, string currency) where T : ChatStateBase
    {
        if (_chatStates.TryGetValue(chatId, out var currentState))
        {
            await currentState.OnExit(chatId);
        }

        var newState = _states[typeof(T)]();
        _chatStates[chatId] = newState;
        await newState.OnEnter(chatId, currency);
    }
}