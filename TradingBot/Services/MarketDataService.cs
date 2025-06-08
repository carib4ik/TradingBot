using System.Globalization;
using Bybit.Net.Clients;
using Bybit.Net.Enums;
using CryptoExchange.Net.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScottPlot;
using TradingBot.Extensions;
using Skender.Stock.Indicators;

namespace TradingBot.Services;

public class MarketDataService
{
    private readonly BybitRestClient _bybitClient;
    private const int CandlesLimit = 800;

    public MarketDataService(string apiKey, string apiSecret)
    {
        _bybitClient = new BybitRestClient(options =>
        {
            options.ApiCredentials = new ApiCredentials(apiKey, apiSecret);
        });
    }
    
    public async Task<string> GetDataFromBinance(string symbol, string interval)
    {
        var url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}&limit={CandlesLimit}";

        Console.WriteLine("Get Market Data");
        
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Market Data received successfully");

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetDataFromByBit(string symbol, string interval)
    {
        
            
        var chart = await _bybitClient.V5Api.ExchangeData.GetKlinesAsync(
            category: Category.Spot,         // Spot или Linear/Inverse для деривативов
            symbol: symbol,
            interval: interval.ToKlineInterval(),
            limit: CandlesLimit
        );
        
        var jsonChart = JsonConvert.SerializeObject(chart.Data.List.Select(c => new {
            Time = c.StartTime.ToString("s"),
            Open = c.OpenPrice,
            High = c.HighPrice,
            Low = c.LowPrice,
            Close = c.ClosePrice
        }), Formatting.Indented);
        
        return jsonChart;
    }
    
    public async Task<double?> GetCurrentRsi(string symbol, string interval, int period = 14)
    {
        var json = await GetDataFromByBit(symbol, interval);
        var candles = JArray.Parse(json);

        var quotes = candles.Select(c => new Quote
        {
            Date = DateTime.Parse(c["Time"]!.ToString(), null, DateTimeStyles.RoundtripKind),
            Open = Convert.ToDecimal(c["Open"]!),
            High = Convert.ToDecimal(c["High"]!),
            Low = Convert.ToDecimal(c["Low"]!),
            Close = Convert.ToDecimal(c["Close"]!),
            Volume = 0 // если объём не нужен, можно оставить 0
        }).ToList();

        var rsiResults = quotes.GetRsi(period);
        return rsiResults.LastOrDefault()?.Rsi;
    }
    
    public async Task<string> GetOpenPositionsAsync()
    {
        var result = await _bybitClient.V5Api.Trading.GetPositionsAsync(category: Category.Linear, settleAsset: "USDT");

        if (!result.Success || result.Data == null)
            return "Ошибка при получении позиций с Bybit.";

        var positions = result.Data.List
            .Where(p => p.PositionValue > 0) // Фильтруем только активные позиции
            .Select(p => new
            {
                p.Symbol,
                Size = p.PositionValue,
                EntryPrice = p.AveragePrice,
                Pnl = p.UnrealizedPnl,
                Leverage = p.Leverage,
                Side = p.Side.ToString()
            });

        return JsonConvert.SerializeObject(positions, Formatting.Indented);
    }

    
    public async Task<string> GenerateChart(string symbol, string interval)
    {
        var json = await GetDataFromByBit(symbol, interval);
        var candles = JArray.Parse(json);

        int count = candles.Count;
        var ohlcs = new OHLC[count];
        
        TimeSpan candleSpan = GetIntervalTimeSpan(interval);

        for (int i = 0; i < count; i++)
        {
            var c = candles[i];
            DateTime time = DateTime.Parse(c["Time"]!.ToString(), null, DateTimeStyles.RoundtripKind);
            double open = (double)c["Open"]!;
            double high = (double)c["High"]!;
            double low = (double)c["Low"]!;
            double close = (double)c["Close"]!;

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