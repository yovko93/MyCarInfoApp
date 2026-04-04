# MyCarInfoApp

## Run with Docker (app + SQL Server)

Start everything with Docker Compose:

```bash
docker compose up --build
```

The app will be available at:

- `http://localhost:8080`

### What this starts

- `mycarinfo-app` (ASP.NET Core app)
- `mycarinfo-db` (SQL Server 2022)

### Important

- Default SQL password in `docker-compose.yml` is for local development only. Change it before using outside local/dev.
- The app now applies EF Core migrations automatically on startup (with retry), so database tables are created when SQL Server becomes available.
- Data Protection keys are persisted in a Docker volume (`app-dpkeys`, mounted at `/var/mycarinfo/dpkeys`) so authentication/antiforgery tokens remain valid across container restarts.
- The containerized SQL connection uses `Server=tcp:db,1433` to force TCP DNS resolution for the `db` service on the Compose network.

## Deploying to Render (important)

Render does **not** run `docker-compose.yml` when you deploy this repo as a web service.  
It only builds and runs the `Dockerfile` container, so the `db` service from Compose is never created on Render.

That means this Compose-only connection string will fail on Render:

- `Server=tcp:db,1433;...` (`db` is only resolvable inside the Compose network)

### Solution options

#### Option A (recommended): deploy app + SQL Server with `render.yaml`

This repo now includes a `render.yaml` Blueprint that creates:

- `mycarinfo-app` (web service from `MyCarInfo/Dockerfile`)
- `mycarinfo-db` (private SQL Server service with persistent disk)

Deploy it from Render with **New + → Blueprint** and select this repository.

> After first deploy, change the default SA password in `render.yaml`/Render env vars before using outside development.

#### Option B: use an external SQL Server provider

1. Provision a separate SQL Server database (Render-managed databases are PostgreSQL; for this app you need SQL Server compatible hosting).
2. In your Render service environment variables, set:
   - `ConnectionStrings__DefaultConnection=<your real SQL Server connection string>`
3. Redeploy the service.

If `ConnectionStrings__DefaultConnection` is not set for Render, the app falls back to `appsettings.json` (local machine defaults), which also will not work in Render.

## Stop containers

```bash
docker compose down
```

To remove DB data volume too:

```bash
docker compose down -v
```

> Note: `down -v` also removes Data Protection keys. Existing login/antiforgery cookies become invalid after that, which is expected.

## Render deployment notes (database)

If the app deploys but crashes with:

`Microsoft.Data.SqlClient.SqlException ... server was not found or was not accessible`

it means the app cannot reach a **SQL Server** instance from Render.

### Required environment variable

Set this env var on your Render Web Service:

- `ConnectionStrings__DefaultConnection`

Example:

```text
Server=<HOST>,1433;Database=MyCarInfoDB;User Id=<USER>;Password=<PASSWORD>;TrustServerCertificate=True;Encrypt=True;
```

### Important

- This project uses `Microsoft.EntityFrameworkCore.SqlServer`, so it requires **Microsoft SQL Server** (not PostgreSQL).
- Render's native managed database is PostgreSQL; if you use Render DB, this app will not connect without code changes to the EF provider.
- Use an externally hosted SQL Server (Azure SQL, SQL Server VM/container reachable from Render, etc.).
- Keep `ConnectionStrings__DefaultConnection` as a Render secret env var (do not hardcode credentials).

Migrations run automatically on startup, so once the connection string points to a reachable SQL Server, the schema is created/updated during boot.