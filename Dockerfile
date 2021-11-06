FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base

LABEL maintainer="DrSmugleaf <DrSmugleaf@users.noreply.github.com>"
LABEL repository="https://github.com/DrSmugleaf/MapDiffBot"
LABEL homepage="https://github.com/DrSmugleaf/MapDiffBot"

LABEL com.github.actions.name="Map Diff Bot"
LABEL com.github.actions.description="Shows map changes for the game Space Station 14."
LABEL com.github.actions.icon="map"
LABEL com.github.actions.color="blue"

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
