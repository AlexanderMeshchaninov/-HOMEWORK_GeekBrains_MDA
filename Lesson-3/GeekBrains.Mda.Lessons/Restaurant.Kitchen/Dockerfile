FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Restaurant.Kitchen/Restaurant.Kitchen.csproj", "Restaurant.Kitchen/"]
RUN dotnet restore "Restaurant.Kitchen/Restaurant.Kitchen.csproj"
COPY . .
WORKDIR "/src/Restaurant.Kitchen"
RUN dotnet build "Restaurant.Kitchen.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Restaurant.Kitchen.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Restaurant.Kitchen.dll"]
