# PhantomGG 👻⚽

A full-stack web application to manage and track soccer tournaments with real-time updates, team management, and comprehensive tournament administration.

## Quick Start

### Prerequisites
- [Docker](https://www.docker.com/get-started) and Docker Compose
- [Node.js](https://nodejs.org/) (for local development)
- [.NET 9 SDK](https://dotnet.microsoft.com/download) (for local development)

### Running with Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/Massamba505/PhantomGG.git
   cd PhantomGG
   ```

2. **Set up environment variables**
   ```bash
   cp .env.example .env
   # Edit .env and update the following required values:
   # - SQL_PASSWORD (strong password for SQL Server)
   # - JWT_SECRET_KEY (at least 32 characters)
   # - Email settings
   ```

3. **Start all services**
   ```bash
   docker-compose up -d
   ```

4. **Access the application**
   - **Frontend**: http://localhost:4200
   - **Backend API**: http://localhost:5000
   - **Seq Logs**: http://localhost:5341

### Local Development

**Backend (.NET API)**
```bash
cd Server
dotnet restore
dotnet run --project PhantomGG.API
```

**Frontend (Angular)**
```bash
cd Client
npm install
ng serve
```

## Tech Stack

- **Frontend**: Angular 20 + TypeScript
- **Backend**: .NET 9 Web API + C#
- **Database**: SQL Server 2022
- **Migrations**: Flyway
- **Logging**: Seq
- **DevOps**: Docker & Docker Compose

## Features

- **User Management**: Registration, authentication with JWT tokens
- **Tournament Management**: Create, configure, and manage tournaments
- **Team Registration**: Team creation and tournament enrollment
- **Match Management**: Schedule matches, record scores and events
- **Live Updates**: Real-time score tracking
- **Standings**: Automatic calculation of points, wins, losses, draws
- **Email Notifications**: Tournament updates and notifications
- **Image Storage**: Upload and manage images for profiles, tournaments, and teams with support for local file storage and Azure Blob Storage

## Image Storage Configuration

PhantomGG supports two image storage providers:
- **LocalFile**: For local development (stores images in `wwwroot/images/`)
- **AzureBlob**: For Docker and production (uses Azurite locally or Azure Blob Storage in production)

## Project Structure

```
PhantomGG/
├── Client/
├── Server/
│   ├── PhantomGG.API/
│   ├── PhantomGG.Service/
│   ├── PhantomGG.Repository/
│   ├── PhantomGG.Models/
│   └── PhantomGG.UnitTests/
├── Database/
└── docker-compose.yaml
```

## Testing

**Run Backend Tests**
```bash
cd Server
dotnet test
```

**Run E2E Tests**
```bash
cd Client
npx cypress open
```

## Troubleshooting

- **Database connection issues**: Ensure SQL_PASSWORD is set correctly in `.env`
- **Port conflicts**: Check if ports 4200, 5000, 1433, or 5341 are already in use
- **Docker issues**: Try `docker-compose down -v` and restart
