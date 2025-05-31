using OpenAI;
using OpenAI.Chat;

namespace TradingBot.Services;

public class ChatGptService
{
    private readonly OpenAIClient _openAiClient;
    private const string Prompt1 = "Ты квалифицированный, опытный трейдер и аналитик, который использует различные инструменты технического анализа графиков (уровни, наклонки, фибоначи, волновой анализ, rsi, macd, объемы). Ты также компетентен во всех вопросах касающихся рынка, экономики. Ты можешь давать прогнозы согласно своему анализу";
    private const string Prompt2 = "Ты супер проффесиональный трейдер с многолетним стажем. Проанализируй следующие данные свечей (OHLC) на основе волн Эллиотта и дай точную точку входа в лонг или шорт только если соблюдается риск минимум 3 к 1 или больше. Пиши только по факту в формате:\nLONG \ud83d\udfe2\nВход: цена\nTP: цена\nSL: цена\n\nили\n\nSHORT \ud83d\udd34\nВход: цена\nTP: цена\nSL: цена или что хорошей сделки не видишь. Дай обоснование своему решению";
    private const string Prompt3 = "Проанализируй следующие данные свечей (OHLC) и построй интерпретацию на основе волн Эллиотта.\n\nТвоя задача:\n1. Обнаружить наличие волн Эллиотта:\n   - Волна 1 (начало импульса), если есть\n   - Волна 3 (сильный импульс, подтверждённый), если есть\n\nЕсли найдена волна 1:\n- Определи предполагаемую точку входа\n- Рассчитай цели для волн 3 и 5 на основе длины волны 1\n- Дай краткий прогноз:\n    - точка входа\n    - цель волны 3\n    - цель волны 5\n    - предложи усреднение на откатах волны 2 и волны 4\n\nЕсли найдена волна 3:\n- Определи предполагаемую точку входа\n- Рассчитай цель по волне 5\n- Дай краткий прогноз:\n    - точка входа\n    - цель волны 5\n    - усреднение на волне 4\n\nВот данные в формате OHLC (формат: ISO-время, open, high, low, close):";
    
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