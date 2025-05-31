using Bybit.Net.Enums;

namespace TradingBot.Extensions
{
    public static class KlineIntervalExtensions
    {
        public static KlineInterval ToKlineInterval(this string interval)
        {
            return interval switch
            {
                "1m" => KlineInterval.OneMinute,
                "3m" => KlineInterval.ThreeMinutes,
                "5m" => KlineInterval.FiveMinutes,
                "15m" => KlineInterval.FifteenMinutes,
                "30m" => KlineInterval.ThirtyMinutes,
                "1h" => KlineInterval.OneHour,
                "2h" => KlineInterval.TwoHours,
                "4h" => KlineInterval.FourHours,
                "6h" => KlineInterval.SixHours,
                "12h" => KlineInterval.TwelveHours,
                "1d" => KlineInterval.OneDay,
                "1w" => KlineInterval.OneWeek,
                "1M" => KlineInterval.OneMonth,
                _ => throw new ArgumentException($"Unknown interval: {interval}")
            };
        }
    }
}