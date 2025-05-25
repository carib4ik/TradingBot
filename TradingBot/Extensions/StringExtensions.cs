namespace TradingBot.Extensions;

public static class StringExtensions
{
    public static string? EscapeMarkdownV2(this string? text)
    {
        var specialCharacters = new[]
            { '_', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };

        foreach (var character in specialCharacters)
        {
            text = text?.Replace(character.ToString(), "\\" + character);
        }

        return text;
    }
}