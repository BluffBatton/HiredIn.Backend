FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HiredIn.Backend.API/HiredIn.Backend.API.csproj", "HiredIn.Backend.API/"]
COPY ["HiredIn.Backend.Application/HiredIn.Backend.Application.csproj", "HiredIn.Backend.Application/"]
COPY ["HiredIn.Backend.Contracts/HiredIn.Backend.Contracts.csproj", "HiredIn.Backend.Contracts/"]
COPY ["HiredIn.Backend.Domain/HiredIn.Backend.Domain.csproj", "HiredIn.Backend.Domain/"]
COPY ["HiredIn.Backend.Infrastructure/HiredIn.Backend.Infrastructure.csproj", "HiredIn.Backend.Infrastructure/"]

RUN dotnet restore "HiredIn.Backend.API/HiredIn.Backend.API.csproj"

COPY . .
WORKDIR "/src/HiredIn.Backend.API"
RUN dotnet publish "HiredIn.Backend.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "HiredIn.Backend.API.dll"]