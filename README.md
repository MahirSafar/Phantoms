# ðŸ‘» Phantoms API

A clean-architecture .NET 10 Web API with PostgreSQL, JWT authentication, Google OAuth, and email support.

---

## 🚀 Quick Start (For New Developers)

> **Required tools — install these first:**
> - [Git](https://git-scm.com/downloads)
> - [Docker Desktop](https://www.docker.com/products/docker-desktop/) — must be **running**
> - [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Step 1 — Clone the repository

```bash
git clone https://github.com/MahirSafar/Phantoms.git
cd Phantoms
```

### Step 2 — Start the database

```bash
docker compose up -d
```

This starts **PostgreSQL** (port `5432`) and **pgAdmin** (http://localhost:5050) in the background.
Wait ~10 seconds for the database to be ready.

### Step 3 — Apply database migrations

**Windows (PowerShell):**
```powershell
dotnet ef database update --project src\Infrastructure\Phantoms.Persistence --startup-project src\Presentation\Phantoms.API
```

**Mac / Linux:**
```bash
dotnet ef database update --project src/Infrastructure/Phantoms.Persistence --startup-project src/Presentation/Phantoms.API
```

> If `dotnet ef` is not found, install it first:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### Step 4 — Run the API

**Option A — Visual Studio:**
Open `Phantoms.slnx` → press **F5** (or the Run button). Swagger opens automatically.

**Option B — Terminal:**
```bash
dotnet run --project src/Presentation/Phantoms.API
```

API: **https://localhost:7054**
Swagger UI: **https://localhost:7054/swagger**

### ✅ Done! All dev settings (DB, JWT, Google OAuth, SMTP) are pre-configured in `appsettings.Development.json`.

---

## ðŸ—ï¸ Project Structure

```
Phantoms/
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ Core/
â”‚   â”‚   â”œâ”€â”€ Phantoms.Domain          # Entities, value objects, domain logic
â”‚   â”‚   â””â”€â”€ Phantoms.Application     # Use cases, interfaces, DTOs
â”‚   â”œâ”€â”€ Infrastructure/
â”‚   â”‚   â”œâ”€â”€ Phantoms.Infrastructure  # Email, Google OAuth, external services
â”‚   â”‚   â””â”€â”€ Phantoms.Persistence     # EF Core, migrations, repositories
â”‚   â””â”€â”€ Presentation/
â”‚       â””â”€â”€ Phantoms.API             # Controllers, middleware, startup
â”œâ”€â”€ Dockerfile
â”œâ”€â”€ docker-compose.yml
â””â”€â”€ deploy.ps1                       # Google Cloud Run deployment script
```

---

## ðŸ³ Running with Docker

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running

### 1. Start PostgreSQL + pgAdmin (database only)

This spins up the database and pgAdmin without building the API image:

```bash
docker compose up -d
```

Services started:

| Service   | URL                        | Purpose              |
|-----------|----------------------------|----------------------|
| PostgreSQL | `localhost:5432`           | Main database        |
| pgAdmin   | http://localhost:5050      | DB management UI     |

### 2. Build and run the full stack (API + database)

First add the `api` service to `docker-compose.yml` if needed, or run the API directly:

```bash
# Build the API image
docker build -t phantoms-api .

# Run the API container linked to the compose network
docker run -d \
  --name phantoms_api \
  --network phantoms_default \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=phantoms_postgres;Port=5432;Database=PhantomsDb;Username=phantoms_user;Password=phantoms_pass" \
  phantoms-api
```

API will be available at: **http://localhost:8080**  
Swagger UI: **http://localhost:8080/swagger**

### 3. Stop all containers

```bash
docker compose down
```

To also remove the database volume (âš ï¸ deletes all data):

```bash
docker compose down -v
```

---

## ðŸ—„ï¸ Database Setup

### Connection Details (Docker)

| Field    | Value            |
|----------|------------------|
| Host     | `localhost`      |
| Port     | `5432`           |
| Database | `PhantomsDb`     |
| Username | `phantoms_user`  |
| Password | `phantoms_pass`  |

### pgAdmin Access

1. Open **http://localhost:5050**
2. Login with:
   - **Email:** `admin@phantoms.com`
   - **Password:** `admin`
3. Add a new server:
   - **Host:** `phantoms_postgres`
   - **Port:** `5432`
   - **Database:** `PhantomsDb`
   - **Username:** `phantoms_user`
   - **Password:** `phantoms_pass`

### Run EF Core Migrations

Make sure the database is running, then from the solution root:

```bash
dotnet ef database update \
  --project src/Infrastructure/Phantoms.Persistence \
  --startup-project src/Presentation/Phantoms.API
```

---

## âš™ï¸ Local Development (without Docker)

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL 16+ running locally

### 1. Configure appsettings

Edit `src/Presentation/Phantoms.API/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Host=localhost;Port=5432;Database=PhantomsDb;Username=postgres;Password=yourpassword"
  }
}
```

### 2. Apply migrations

```bash
dotnet ef database update \
  --project src/Infrastructure/Phantoms.Persistence \
  --startup-project src/Presentation/Phantoms.API
```

### 3. Run the API

```bash
dotnet run --project src/Presentation/Phantoms.API
```

API available at: **http://localhost:5000**  
Swagger UI: **http://localhost:5000/swagger**

---

## ðŸ” Environment Variables

When running in Docker or production, configure these environment variables:

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `JwtSettings__Key` | JWT signing key (min 32 chars) |
| `JwtSettings__Issuer` | JWT issuer |
| `JwtSettings__Audience` | JWT audience |
| `GoogleAuthSettings__ClientId` | Google OAuth Client ID |
| `GoogleAuthSettings__ClientSecret` | Google OAuth Client Secret |
| `SmtpSettings__UserName` | SMTP email address |
| `SmtpSettings__Password` | SMTP app password |

---

## â˜ï¸ Deploy to Google Cloud Run

Make sure you have the [gcloud CLI](https://cloud.google.com/sdk/docs/install) installed and authenticated, then:

```powershell
.\deploy.ps1
```

---

## ðŸ“ License

MIT
