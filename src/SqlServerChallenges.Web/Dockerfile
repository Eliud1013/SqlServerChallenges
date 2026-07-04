FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SqlServerChallenges.Web/SqlServerChallenges.Web.csproj", "src/SqlServerChallenges.Web/"]
RUN dotnet restore "src/SqlServerChallenges.Web/SqlServerChallenges.Web.csproj"
COPY . .
WORKDIR "/src/src/SqlServerChallenges.Web"
RUN dotnet build "./SqlServerChallenges.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SqlServerChallenges.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SqlServerChallenges.Web.dll"]
