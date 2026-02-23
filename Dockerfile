# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj và restore dependencies
COPY ["FPTTelecomBE.csproj", "./"]
RUN dotnet restore "FPTTelecomBE.csproj"

# Copy toàn bộ source code
COPY . .

# Build project
RUN dotnet build "FPTTelecomBE.csproj" -c Release -o /app/build

# Publish project
RUN dotnet publish "FPTTelecomBE.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy file đã publish từ build stage
COPY --from=build /app/publish .

# Expose port 8080 (Render sẽ tự động map)
EXPOSE 8080

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "FPTTelecomBE.dll"]