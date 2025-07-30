# ConfigMapster Project Documentation

## Overview
ConfigMapster is a .NET 5-based configuration management API for storing, updating, and retrieving application configuration records. It uses MongoDB for persistent storage, Redis for caching, and RabbitMQ for event messaging.

## Prerequisites
- .NET 5 SDK
- Docker & Docker Compose

## Project Structure
- **ConfigMapster.API**: ASP.NET Core Web API entry point
- **ConfigMapster.API.ApplicationService**: Application logic and services
- **ConfigMapster.API.Domain**: Domain models and events
- **ConfigMapster.API.Infrastructure**: Infrastructure utilities and validators
- **ConfigMapster.API.Persistence**: Data access and repository implementations

## Configuration
Edit `ConfigMapster.API/appsettings.json` to set connection strings and other settings:
```json
{
  "RedisConfig": {
    "Url": "localhost:6379",
    "Database": 0,
    "ExpireTimeSpan": 1000
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://root:example@localhost:27017",
    "DatabaseName": "configuration-db"
  },
  "RabbitMq": {
    "HostName": "amqp://guest:guest@localhost:5672/",
    "Exchange": "domain_events",
    "QueueName": "domain_events_queue"
  }
}
```

## Running Dependencies with Docker Compose
Start MongoDB, Redis, and RabbitMQ using Docker Compose:
```sh
docker-compose -f docker-compose.yaml up -d
```

## Building and Running
1. Clone the repository.
2. Ensure dependencies are running via Docker Compose.
3. Build and run the API project:
   ```sh
   dotnet build
   dotnet run --project ConfigMapster.API/ConfigMapster.API.csproj
   ```
4. Access API endpoints for configuration management.

## API Endpoints
- `POST /api/config/insert`: Create a new configuration record
- `PUT /api/config/update`: Update an existing configuration record
- `DELETE /api/config/delete?id={id}`: Delete a configuration record
- `GET /api/config/list?applicationName={name}`: List configuration records by application name

## Testing
Unit tests are located in the `ConfigMapsterApplicationService.Tests` project. Run tests with:
```sh
dotnet test
```

## Contribution
Pull requests and issues are welcome. Please follow standard .NET coding conventions.

---
For more details, see the source code and comments in each project folder.