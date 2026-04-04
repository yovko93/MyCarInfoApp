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
- Data Protection keys are persisted in a Docker volume (`app-dpkeys`) so authentication/antiforgery tokens remain valid across container restarts.

## Stop containers

```bash
docker compose down
```

To remove DB data volume too:

```bash
docker compose down -v
```

> Note: `down -v` also removes Data Protection keys. Existing login/antiforgery cookies become invalid after that, which is expected.