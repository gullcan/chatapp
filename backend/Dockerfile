# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY backend/ChatBackend.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Optionally set AI service URL here (can be overridden in Render env vars)
ENV AI_SERVICE_URL="https://gulcan9-ai-service.hf.space"

# Bind to the PORT provided by Render, default to 5002 locally
ENV ASPNETCORE_URLS="http://0.0.0.0:5002"

# Copy published output
COPY --from=build /app/out ./

# Expose container port
EXPOSE 5002

# Use a shell entrypoint to honor PORT if provided by Render
# Falls back to 5002 if PORT is not set
ENTRYPOINT ["/bin/sh", "-c", "dotnet ChatBackend.dll --urls http://0.0.0.0:${PORT:-5002}"]
