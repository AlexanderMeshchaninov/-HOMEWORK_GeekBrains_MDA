FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Restaurant.Notifications/Restaurant.Notifications.csproj", "Restaurant.Notifications/"]
COPY ["Mda.Lessons.Dto/Mda.Lessons.Dto.csproj", "Mda.Lessons.Dto/"]
COPY ["Mda.Lessons.HostingService/Mda.Lessons.HostingService.csproj", "Mda.Lessons.HostingService/"]
COPY ["Restaurant.Messaging/Restaurant.Messaging.csproj", "Restaurant.Messaging/"]
COPY ["Mda.Lessons.Configuration/Mda.Lessons.Configuration.csproj", "Mda.Lessons.Configuration/"]
COPY ["Mda.Lessons.RabbitMQ/Mda.Lessons.RabbitMQ.csproj", "Mda.Lessons.RabbitMQ/"]
COPY ["Mda.Lessons.MassTransit/Mda.Lessons.MassTransit.csproj", "Mda.Lessons.MassTransit/"]
RUN dotnet restore "Restaurant.Notifications/Restaurant.Notifications.csproj"
COPY . .
WORKDIR "/src/Restaurant.Notifications"
RUN dotnet build "Restaurant.Notifications.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Restaurant.Notifications.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Restaurant.Notifications.dll"]
