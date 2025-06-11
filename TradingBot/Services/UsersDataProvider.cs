using System.Collections.Concurrent;
using Newtonsoft.Json;
using TradingBot.Data;

namespace TradingBot.Services;

public class UsersDataProvider
{
    private readonly ConcurrentDictionary<long, UserData> _usersData = new();
    private const string FILE_PATH = "AppSettings/Users.json";
    
    public void SaveChatIdIfNotExists(long chatId)
    {
        var chatIds = LoadChatIds();
        
        if (!chatIds.Contains(chatId))
        {
            chatIds.Add(chatId);
            File.WriteAllText(FILE_PATH, JsonConvert.SerializeObject(chatIds, Formatting.Indented));
        }
    }
    
    public List<long> LoadChatIds()
    {
        if (!File.Exists(FILE_PATH))
        {
            return new List<long>();
        }
        
        var json = File.ReadAllText(FILE_PATH);
        
        return JsonConvert.DeserializeObject<List<long>>(json) ?? new List<long>();
    }
}