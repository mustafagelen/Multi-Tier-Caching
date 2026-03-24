# Multi-Tier Caching

Multi-tier caching to handle high read volumes.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 10 / ASP.NET Core Minimal APIs |
| ORM | Entity Framework Core 10 + PostgreSQL 17 |
| Caching | HybridCache (L1 in-memory + L2 Redis) |
| Auth | JWT Bearer |
| API Docs | Scalar |
| Versioning | `Asp.Versioning.Http` — Header-based (`X-API-Version`) |
| Containers | Docker Compose (postgres, redis, seq, web-api) |

---

## Running with Docker Compose

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/mustafagelen/Multi-Tier-Caching
cd Multi-Tier-Caching

# 2. Build and start all containers
docker-compose up --build

# 3. Open Scalar API docs
# http://localhost:5000/scalar
```

> The database is automatically seeded with **100,000 transactions** on first startup using Bogus.

---

## Authentication

All transaction endpoints require a token. Use the `/api/login` endpoint to obtain one.

### Credentials

| Field | Value |
|---|---|
| `username` | `admin` |
| `password` | `password` |
| `isPremium` | `false` / `true` |

### Standard token
```json
{ "username": "admin", "password": "password", "isPremium": false }
```

### Premium token => V2
```json
{ "username": "admin", "password": "password", "isPremium": true }
```

---

### POST body — ready to use

```json
{
  "amount": 249.99,
  "currency": "USD",
  "description": "Wireless Headphones",
  "status": "Completed",
  "merchantName": "Amazon",
  "category": "Electronics"
}
```

---

## v1 vs v2 Performance

Both endpoints query the same **100,000 transactions** for ~31 MB of data.

| | v1 — Buffered | v2 — Streaming |
|---|---|---|
| **Header** | `X-API-Version: 1` | `X-API-Version: 2` |
| **Strategy** | `ToListAsync()` → full buffer in memory | `AsAsyncEnumerable()` → row-by-row stream |
| **Response size** | ~31.66 MB | ~31.66 MB |
| **Time to first byte** | ~1600 ms | **~305 ms** |

---

## Caching — HybridCache

The `/api/transactions/summary` endpoint demonstrates two-tier caching:

- **L1 :** `LocalCacheExpiration = 2 minutes` — zero-latency on same instance
- **L2 :** `Expiration = 5 minutes` — shared across instances
