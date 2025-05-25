using System.Collections.Concurrent;
using TradingBot.Data;

namespace TradingBot.Services;

public class UsersDataProvider
{
    private readonly ConcurrentDictionary<long, UserData> _usersData = new();
    
    public void SetTelegramName(long chatId, string? telegramName)
    {
        var userState = _usersData.GetOrAdd(chatId, new UserData());
        userState.TelegramName = telegramName;
    }
    
    public UserData GetUserData(long chatId)
    {
        return _usersData[chatId];
    }

    public void ClearUserData(long chatId)
    {
        _usersData.TryRemove(chatId, out _);
    }

    public void SaveLastQuestion(long chatId, string? question)
    {
        var userState = _usersData.GetOrAdd(chatId, new UserData());
        userState.LastQuestion = question;
    }
}