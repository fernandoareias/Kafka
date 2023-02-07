FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Kafka.API/Kafka.API.csproj", "Kafka.API/"]
RUN dotnet restore "Kafka.API/Kafka.API.csproj"
COPY . .
WORKDIR "/src/Kafka.API"
RUN dotnet build "Kafka.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kafka.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kafka.API.dll"]
