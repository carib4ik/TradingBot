using System.Collections.Concurrent;
using Newtonsoft.Json;
using TradingBot.Data;

namespace TradingBot.Services;

public class UsersDataProvider
{
    private readonly ConcurrentDictionary<long, UserData> _usersData = new();
    private const string FilePath = "AppSettings/Users.json";
    
    public void SaveChatIdIfNotExists(long chatId)
    {
        var chatIds = LoadChatIds();
        
        if (!chatIds.Contains(chatId))
        {
            chatIds.Add(chatId);
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(chatIds, Formatting.Indented));
        }
    }
    
    public List<long> LoadChatIds()
    {
        if (!File.Exists(FilePath))
        {
            return new List<long>();
        }
        
        var json = File.ReadAllText(FilePath);
        
        return JsonConvert.DeserializeObject<List<long>>(json) ?? new List<long>();
    }
}