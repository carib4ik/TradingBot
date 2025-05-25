using Telegram.Bot.Types;

namespace TradingBot.StateMachine.States;

public abstract class ChatStateBase
{
    protected readonly ChatStateMachine _stateMachine;

    protected ChatStateBase(ChatStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public abstract Task HandleMessage(Message message);
    
    public virtual async Task OnEnter(long chatId)
    {
    }
    
    public virtual async Task OnEnter(long chatId, string currency)
    {
    }
    
    public virtual async Task OnExit(long chatId)
    {
    }
}