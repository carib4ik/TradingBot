using OpenAI;
using OpenAI.Chat;

namespace TradingBot.Services;

public class ChatGptService
{
    private readonly OpenAIClient _openAiClient;
    private const string Prompt1 = "Ты квалифицированный, опытный трейдер и аналитик, который использует различные инструменты технического анализа графиков (уровни, наклонки, фибоначи, волновой анализ, rsi, macd, объемы). Ты также компетентен во всех вопросах касающихся рынка, экономики. Ты можешь давать прогнозы согласно своему анализу";
    private const string Prompt2 = "Проанализируй предоставленные графики с помощью волнового анализа элиота и дай точную точку входа в лонг или шорт для быстрой краткосрочной сделки. Пиши только по факту в формате:\nLONG \ud83d\udfe2\nВход: цена\nTP: цена\nSL: цена\n\nили\n\nSHORT \ud83d\udd34\nВход: цена\nTP: цена\nSL: цена + короткое обоснование + Разметь предоставленные тебе графики и пришли мне обратно";
    
    public ChatGptService(string openAiApiKey)
    {
        _openAiClient = new OpenAIClient(openAiApiKey);
    }

    
    // public async Task<string?> GetTradeFromChatGpt(string[] filePaths)
    // {
    //     var chatClient = _openAiClient.GetChatClient("gpt-4o");
    //     
    //     var userContent = new ChatMessageContent();
    //     
    //     foreach (var filePath in filePaths)
    //     {
    //         if (File.Exists(filePath))
    //         {
    //             userContent.Add(filePath);
    //         }
    //         else
    //         {
    //             Console.WriteLine($"Файл не найден: {filePath}");
    //         }
    //     }
    //     
    //     if (userContent.Count == 0)
    //     {
    //         return "Нет изображений для отправки.";
    //     }
    //     
    //     var messages = new List<ChatMessage>
    //     {
    //         ChatMessage.CreateSystemMessage(Prompt2),
    //         ChatMessage.CreateUserMessage(userContent)
    //     };
    //     
    //     try
    //     {
    //         Console.WriteLine("Отправка запроса к OpenAI...");
    //
    //         var result = await chatClient.CompleteChatAsync(messages).ConfigureAwait(false);
    //
    //         Console.WriteLine("Ответ получен");
    //
    //         if (result.Value != null)
    //         {
    //             return result.Value.Content[0].Text;
    //         }
    //
    //         Console.WriteLine("OpenAI API: ответ пустой или недоступен.");
    //         return "OpenAI API: ответ пустой или недоступен.";
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Ошибка при обращении к OpenAI: {ex.Message}");
    //         return $"Ошибка при обращении к OpenAI: {ex.Message}";
    //     }
    // }
    
    // public async Task<string?> GetTradeFromChatGpt(string[] filePaths)
    // {
    //     var chatClient = _openAiClient.GetChatClient("gpt-4o");
    //
    //     var userContent = new ChatMessageContent();
    //
    //     foreach (var filePath in filePaths)
    //     {
    //         if (!File.Exists(filePath))
    //         {
    //             Console.WriteLine($"Файл не найден: {filePath}");
    //             continue;
    //         }
    //
    //         try
    //         {
    //             byte[] bytes = await File.ReadAllBytesAsync(filePath);
    //             string base64String = Convert.ToBase64String(bytes);
    //
    //             userContent.Add(base64String);
    //             
    //             Console.WriteLine($"Добавлено изображение из файла: {filePath}");
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"Ошибка при чтении или кодировании файла {filePath}: {ex.Message}");
    //         }
    //     }
    //
    //     if (userContent.Count == 0)
    //     {
    //         return "Нет изображений для отправки.";
    //     }
    //
    //     var messages = new List<ChatMessage>
    //     {
    //         ChatMessage.CreateSystemMessage(Prompt2),
    //         ChatMessage.CreateUserMessage(userContent)
    //     };
    //
    //     try
    //     {
    //         Console.WriteLine("Отправка запроса к OpenAI...");
    //
    //         var result = await chatClient.CompleteChatAsync(messages).ConfigureAwait(false);
    //
    //         Console.WriteLine("Ответ получен");
    //
    //         if (result?.Value?.Content?.Count > 0)
    //         {
    //             return result.Value.Content[0].Text;
    //         }
    //
    //         Console.WriteLine("OpenAI API: ответ пустой или недоступен.");
    //         return "OpenAI API: ответ пустой или недоступен.";
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Ошибка при обращении к OpenAI: {ex.Message}");
    //         return $"Ошибка при обращении к OpenAI: {ex.Message}";
    //     }
    // }

    public async Task<string?> GetTradeFromChatGpt(string chartData)
    {
        var chatClient = _openAiClient.GetChatClient("gpt-4o");
        
        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(Prompt2),
            ChatMessage.CreateUserMessage(chartData)
        };
        
        try
        {
            Console.WriteLine("Отправка запроса к OpenAI...");

            var result = await chatClient.CompleteChatAsync(messages).ConfigureAwait(false);

            Console.WriteLine("Ответ получен");

            if (result.Value != null)
            {
                return result.Value.Content[0].Text;
            }

            Console.WriteLine("OpenAI API: ответ пустой или недоступен.");
            return "OpenAI API: ответ пустой или недоступен.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обращении к OpenAI: {ex.Message}");
            return $"Ошибка при обращении к OpenAI: {ex.Message}";
        }
    }
    
    
    public async Task<string?> GetAnswerFromChatGpt(string? question)
    {
        var chatClient = _openAiClient.GetChatClient("gpt-4o");
        
        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(Prompt1),
            ChatMessage.CreateUserMessage(question)
        };
        
        try
        {
            Console.WriteLine("Отправка запроса к OpenAI...");

            var result = await chatClient.CompleteChatAsync(messages).ConfigureAwait(false);

            Console.WriteLine("Ответ получен");

            if (result.Value != null)
            {
                return result.Value.Content[0].Text;
            }

            Console.WriteLine("OpenAI API: ответ пустой или недоступен.");
            return "OpenAI API: ответ пустой или недоступен.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обращении к OpenAI: {ex.Message}");
            return $"Ошибка при обращении к OpenAI: {ex.Message}";
        }
    }
}