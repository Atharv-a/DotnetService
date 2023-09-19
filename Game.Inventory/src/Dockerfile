# Build the app
FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /source
COPY . .
WORKDIR /source/Game.Inventory.Service
RUN dotnet publish -c release -o /app


# Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Game.Inventory.Service.dll"]