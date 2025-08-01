FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5213

ENV ASPNETCORE_URLS=http://*:$PORT

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["StreamNest.API/StreamNest.API.csproj", "StreamNest.API/"]
RUN dotnet restore "StreamNest.API/StreamNest.API.csproj"
COPY . .
WORKDIR "/src/StreamNest.API"
RUN dotnet build "StreamNest.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "StreamNest.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StreamNest.API.dll"]
