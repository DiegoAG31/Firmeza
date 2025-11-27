# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Presentation/Firmeza.Web.Mvc/Firmeza.Web.Mvc.csproj", "Presentation/Firmeza.Web.Mvc/"]
COPY ["src/Core/Firmeza.Domain/Firmeza.Domain.csproj", "Core/Firmeza.Domain/"]
COPY ["src/Core/Firmeza.Application/Firmeza.Application.csproj", "Core/Firmeza.Application/"]
COPY ["src/Infrastructure/Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Infrastructure/Firmeza.Infrastructure/"]

RUN dotnet restore "Presentation/Firmeza.Web.Mvc/Firmeza.Web.Mvc.csproj"

# Copy all source code
COPY src/ .

# Build the application
WORKDIR "/src/Presentation/Firmeza.Web.Mvc"
RUN dotnet build "Firmeza.Web.Mvc.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Firmeza.Web.Mvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Firmeza.Web.Mvc.dll"]
