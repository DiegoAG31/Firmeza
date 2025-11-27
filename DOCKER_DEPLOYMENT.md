# Docker Deployment Guide

This guide explains how to deploy the Firmeza application using Docker.

## Prerequisites

- Docker Engine 20.10+
- Docker Compose 2.0+

## Quick Start

1. **Copy environment file**:
   ```bash
   cp .env.example .env
   ```

2. **Build and start all services**:
   ```bash
   docker-compose up -d --build
   ```

3. **Check service status**:
   ```bash
   docker-compose ps
   ```

4. **View logs**:
   ```bash
   docker-compose logs -f
   ```

## Services

The deployment includes 4 services:

| Service | Port | Description |
|---------|------|-------------|
| postgres | 5432 | PostgreSQL database |
| web-mvc | 5000 | ASP.NET MVC application |
| web-api | 5001 | ASP.NET Web API |
| client | 3000 | Next.js frontend |

## Accessing the Applications

- **Web MVC**: http://localhost:5000
- **Web API**: http://localhost:5001
- **Client**: http://localhost:3000

## Database Migrations

Migrations run automatically when the MVC and API applications start. If you need to run them manually:

```bash
# Access the web-mvc container
docker exec -it firmeza-web-mvc bash

# Run migrations
dotnet ef database update
```

## Stopping Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: deletes database data)
docker-compose down -v
```

## Troubleshooting

### Services not starting

Check logs for specific service:
```bash
docker-compose logs web-mvc
docker-compose logs web-api
docker-compose logs postgres
```

### Database connection issues

Ensure PostgreSQL is healthy:
```bash
docker-compose ps postgres
```

### Rebuild after code changes

```bash
docker-compose up -d --build
```

## Production Deployment

For production:

1. Update `.env` with secure credentials
2. Change `JWT_SECRET_KEY` to a strong random value
3. Update `POSTGRES_PASSWORD`
4. Consider using Docker secrets for sensitive data
5. Set up reverse proxy (nginx) for HTTPS
6. Configure proper backup strategy for PostgreSQL volume
