FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Restaurant.Booking/Restaurant.Booking.csproj", "Restaurant.Booking/"]
COPY ["Mda.Lessons.Configuration/Mda.Lessons.Configuration.csproj", "Mda.Lessons.Configuration/"]
COPY ["Mda.Lessons.Dto/Mda.Lessons.Dto.csproj", "Mda.Lessons.Dto/"]
COPY ["Restaurant.Messaging/Restaurant.Messaging.csproj", "Restaurant.Messaging/"]
COPY ["Mda.Lessons.RabbitMQ/Mda.Lessons.RabbitMQ.csproj", "Mda.Lessons.RabbitMQ/"]
COPY ["Mda.Lessons.HostingService/Mda.Lessons.HostingService.csproj", "Mda.Lessons.HostingService/"]
RUN dotnet restore "Restaurant.Booking/Restaurant.Booking.csproj"
COPY . .
WORKDIR "/src/Restaurant.Booking"
RUN dotnet build "Restaurant.Booking.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Restaurant.Booking.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Restaurant.Booking.dll"]
