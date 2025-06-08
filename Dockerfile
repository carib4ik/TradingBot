# Сборка проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем sln и проект
COPY TradingBot.sln ./
COPY TradingBot ./TradingBot/

# Публикуем
RUN dotnet publish TradingBot/TradingBot.csproj -c Release -o /app/publish

# Финальный контейнер
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TradingBot.dll"]
