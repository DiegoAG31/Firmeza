# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Presentation/Firmeza.Web.Api/Firmeza.Web.Api.csproj", "Presentation/Firmeza.Web.Api/"]
COPY ["src/Core/Firmeza.Domain/Firmeza.Domain.csproj", "Core/Firmeza.Domain/"]
COPY ["src/Core/Firmeza.Application/Firmeza.Application.csproj", "Core/Firmeza.Application/"]
COPY ["src/Infrastructure/Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Infrastructure/Firmeza.Infrastructure/"]

RUN dotnet restore "Presentation/Firmeza.Web.Api/Firmeza.Web.Api.csproj"

# Copy all source code
COPY src/ .

# Build the application
WORKDIR "/src/Presentation/Firmeza.Web.Api"
RUN dotnet build "Firmeza.Web.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Firmeza.Web.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5001

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Firmeza.Web.Api.dll"]
