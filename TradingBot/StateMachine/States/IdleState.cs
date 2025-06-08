using Telegram.Bot.Types;

namespace TradingBot.StateMachine.States;

public class IdleState : ChatStateBase
{
    public IdleState(ChatStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("IdleState");
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
}