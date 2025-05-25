using System.Globalization;
using Newtonsoft.Json.Linq;
using ScottPlot;

namespace TradingBot.Services;

public class MarketDataService
{
    private const int CandlesLimit = 600;
    
    public async Task<string> GetData(string symbol, string interval)
    {
        var url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}&limit={CandlesLimit}";

        Console.WriteLine("Get Market Data");
        
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Market Data received successfully");

        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> GenerateChart(string symbol, string interval)
    {
        var json = await GetData(symbol, interval);
        var candles = JArray.Parse(json);

        int count = candles.Count;
        var ohlcs = new OHLC[count];
        
        TimeSpan candleSpan = GetIntervalTimeSpan(interval);

        for (int i = 0; i < count; i++)
        {
            var c = candles[i];
            long unixTime = (long)c[0];
            double open = double.Parse((string)c[1], CultureInfo.InvariantCulture);
            double high = double.Parse((string)c[2], CultureInfo.InvariantCulture);
            double low = double.Parse((string)c[3], CultureInfo.InvariantCulture);
            double close = double.Parse((string)c[4], CultureInfo.InvariantCulture);

            DateTime time = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).DateTime;
            ohlcs[i] = new OHLC(open, high, low, close, time, candleSpan);
        }

        // Создание графика
        var plt = new Plot();
        plt.Title($"{symbol} — {interval} Candlestick Chart");
        plt.Add.Candlestick(ohlcs);
        plt.Axes.DateTimeTicksBottom();
        plt.YLabel("Price (USDT)");

        // Сохраняем изображение
        string chartsDir = "Charts";
        Directory.CreateDirectory(chartsDir);
        string filePath = Path.Combine(chartsDir, $"{symbol}_{interval}.png");
        plt.SavePng(filePath, 1000, 600);

        Console.WriteLine($"График {filePath} сохранен");

        return filePath;
    }
    
    private TimeSpan GetIntervalTimeSpan(string interval)
    {
        return interval switch
        {
            "1m" => TimeSpan.FromMinutes(1),
            "3m" => TimeSpan.FromMinutes(3),
            "5m" => TimeSpan.FromMinutes(5),
            "15m" => TimeSpan.FromMinutes(15),
            "30m" => TimeSpan.FromMinutes(30),
            "1h" => TimeSpan.FromHours(1),
            "2h" => TimeSpan.FromHours(2),
            "4h" => TimeSpan.FromHours(4),
            "6h" => TimeSpan.FromHours(6),
            "12h" => TimeSpan.FromHours(12),
            "1d" => TimeSpan.FromDays(1),
            _ => throw new ArgumentException($"Unknown interval: {interval}")
        };
    }
}