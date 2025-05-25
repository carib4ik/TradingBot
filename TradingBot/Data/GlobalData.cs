namespace TradingBot.Data;

public class GlobalData
{
    public const string START = "/start";
    public const string ASK = "Задать вопрос ChatGPT";
    public const string MARKET_DATA = "Получить точку входа";
    public const string BTC = "BTCUSDT";
    public const string ETH = "ETHUSDT";
    public const string SOL = "SOLUSDT";
}

enum Currecy
{
    BTC, ETH, SOL, LTC, ADA, XRP
}