FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["backend_challenge.csproj", "./"]
RUN dotnet restore "backend_challenge.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "backend_challenge.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "backend_challenge.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

RUN mkdir -p /app/Logs && chmod -R 777 /app/Logs

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "backend_challenge.dll"]
