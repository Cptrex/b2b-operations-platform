FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Platform.Auth.Business/Platform.Auth.Business.csproj", "Platform.Auth.Business/"]
COPY ["Paltform.Auth.Shared/Paltform.Auth.Shared.csproj", "Paltform.Auth.Shared/"]
COPY ["Platform.Shared.Cache/Platform.Shared.Cache.csproj", "Platform.Shared.Cache/"]
COPY ["Platform.Shared.Cache.Keys/Platform.Shared.Cache.Keys.csproj", "Platform.Shared.Cache.Keys/"]
COPY ["Platform.Shared.Results/Platform.Shared.Results.csproj", "Platform.Shared.Results/"]
COPY ["Platform.Shared.Messaging/Platform.Shared.Messaging.csproj", "Platform.Shared.Messaging/"]

RUN dotnet restore "Platform.Auth.Business/Platform.Auth.Business.csproj"
COPY . .
WORKDIR "/src/Platform.Auth.Business"
RUN dotnet build "Platform.Auth.Business.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Platform.Auth.Business.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN groupadd -r appuser && useradd -r -g appuser appuser
COPY --from=build /app/publish .
RUN chown -R appuser:appuser /app
USER appuser
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "Platform.Auth.Business.dll"]