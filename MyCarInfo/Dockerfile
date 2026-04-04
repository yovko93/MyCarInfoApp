FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN if [ -f "MyCarInfo/MyCarInfo.csproj" ]; then \
      dotnet restore "MyCarInfo/MyCarInfo.csproj"; \
    elif [ -f "MyCarInfo.csproj" ]; then \
      dotnet restore "MyCarInfo.csproj"; \
    else \
      echo "MyCarInfo.csproj not found in build context" && exit 1; \
    fi

RUN if [ -f "MyCarInfo/MyCarInfo.csproj" ]; then \
      dotnet publish "MyCarInfo/MyCarInfo.csproj" -c Release -o /app/publish /p:UseAppHost=false; \
    else \
      dotnet publish "MyCarInfo.csproj" -c Release -o /app/publish /p:UseAppHost=false; \
    fi

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyCarInfo.dll"]