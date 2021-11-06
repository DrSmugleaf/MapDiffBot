FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MapDiffBot/MapDiffBot.csproj", "MapDiffBot/"]
RUN dotnet restore "MapDiffBot/MapDiffBot.csproj"
COPY . .
WORKDIR "/src/MapDiffBot"
RUN dotnet build "MapDiffBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MapDiffBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MapDiffBot.dll"]
