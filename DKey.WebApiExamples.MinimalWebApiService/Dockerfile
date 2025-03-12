FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DKey.WebApiExamples.MinimalWebApiService/DKey.WebApiExamples.MinimalWebApiService.csproj", "DKey.WebApiExamples.MinimalWebApiService/"]
RUN dotnet restore "DKey.WebApiExamples.MinimalWebApiService/DKey.WebApiExamples.MinimalWebApiService.csproj"
COPY . .
WORKDIR "/src/DKey.WebApiExamples.MinimalWebApiService"
RUN dotnet build "DKey.WebApiExamples.MinimalWebApiService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DKey.WebApiExamples.MinimalWebApiService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DKey.WebApiExamples.MinimalWebApiService.dll"]
