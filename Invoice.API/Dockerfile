# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["Invoice.sln", "./"]
COPY ["Invoice.Domain/Invoice.Domain.csproj", "Invoice.Domain/"]
COPY ["Invoice.Application/Invoice.Application.csproj", "Invoice.Application/"]
COPY ["Invoice.Infrastructure/Invoice.Infrastructure.csproj", "Invoice.Infrastructure/"]
COPY ["Invoice.API/Invoice.API.csproj", "Invoice.API/"]

# Restore dependencies
RUN dotnet restore "Invoice.sln"

# Copy the rest of the source code
COPY . .

# Build the project
RUN dotnet build "Invoice.API/Invoice.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish

RUN dotnet publish "Invoice.API/Invoice.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create directories for uploads
RUN mkdir -p /app/wwwroot/uploads

# Expose ports
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y ca-certificates && update-ca-certificates

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Invoice.API.dll"]
