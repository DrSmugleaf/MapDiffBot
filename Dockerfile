# Set the base image as the .NET 5.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
COPY . ./
RUN dotnet publish ./MapDiffBot/MapDiffBot.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="DrSmugleaf <DrSmugleaf@users.noreply.github.com>"
LABEL repository="https://github.com/DrSmugleaf/MapDiffBot"
LABEL homepage="https://github.com/DrSmugleaf/MapDiffBot"

# Label as GitHub action
LABEL com.github.actions.name="Map Diff Bot"
# Limit to 160 characters
LABEL com.github.actions.description="Shows map changes for the game Space Station 14."
# See branding:
# https://docs.github.com/actions/creating-actions/metadata-syntax-for-github-actions#branding
LABEL com.github.actions.icon="map"
LABEL com.github.actions.color="blue"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/sdk:5.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/MapDiffBot.dll" ]