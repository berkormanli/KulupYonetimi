# syntax=docker/dockerfile:1.6

# ---------- Base runtime image ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
# .NET 8 containers default to 8080; keep it explicit for clarity and PaaS integration
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# ---------- Build image ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy sln and project file(s) first to leverage Docker layer caching
COPY ["KulupYonetimi.sln", "."]
COPY ["KulupYonetimi/KulupYonetimi.csproj", "KulupYonetimi/"]

# Restore dependencies
RUN dotnet restore "KulupYonetimi/KulupYonetimi.csproj"

# Copy the rest of the source
COPY . .
WORKDIR /src/KulupYonetimi

# Build
RUN dotnet build "KulupYonetimi.csproj" -c Release -o /app/build --no-restore

# ---------- Publish image ----------
FROM build AS publish
RUN dotnet publish "KulupYonetimi.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Final runtime image ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# If Coolify maps a different external port, set ASPNETCORE_URLS at runtime via env
# or configure Coolify's "HTTP Exposed Port" to 8080.

ENTRYPOINT ["dotnet", "KulupYonetimi.dll"]
