FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_ENVIROMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /BYDPlatform
COPY ["BYDPlatform.Api/BYDPlatform.Api.csproj", "BYDPlatform.Api/"]
COPY ["BYDPlatform.Application/BYDPlatform.Application.csproj", "BYDPlatform.Application/"]
COPY ["BYDPlatform.Domain/BYDPlatform.Domain.csproj", "BYDPlatform.Domain/"]
COPY ["BYDPlatform.Infrastructure/BYDPlatform.Infrastructure.csproj", "BYDPlatform.Infrastructure/"]
RUN dotnet restore "BYDPlatform.Api/BYDPlatform.Api.csproj"
COPY . .
#WORKDIR /BYDPlatform.Api
RUN dotnet build "BYDPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BYDPlatform.Api/BYDPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BYDPlatform.Api.dll"]
