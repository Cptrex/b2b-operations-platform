# B2B Operations Platform

Microservices platform for B2B operations management built on .NET 10.

[Русская версия](README.ru.md)

---

## 💻 Technology Stack

### Backend
- **.NET 10** (C# 14.0)
- **ASP.NET Core** (Kestrel)
- **Entity Framework Core** (PostgreSQL)
- **MongoDB Driver**

### Security
- **Microsoft.IdentityModel.Tokens** - JWT validation
- **System.IdentityModel.Tokens.Jwt** - JWT generation
- **RSA 2048-bit** - Asymmetric cryptography

### Messaging & Caching
- **RabbitMQ** - Event-driven communication
- **Redis** - Distributed cache

### Infrastructure
- **PostgreSQL** - Primary database
- **MongoDB** - Document storage
- **Docker** - Containerization

### Libraries
- **Polly** - Resilience and retry policies
- **StackExchange.Redis** - Redis client

---

## 🏥 Health Check

### Auth.Service
```bash
curl http://localhost:5257/api/v1/internal/token/health-check
```

### Auth.Business
Use the health endpoint for `Platform.Auth.Business`:
```bash
curl http://localhost:8081/api/v1/auth/health-check
```

### Service.Business
Use the health endpoint for `Platform.Service.Business` (mapped to host port 8082):
```bash
curl http://localhost:8082/api/v1/business/health-check
```

### Service.Orders
```bash
curl http://localhost:8083/api/v1/orders/health-check
```

### Service.Notify
```bash
curl http://localhost:8084/api/v1/notify/health-check
```

### Service.Search
```bash
curl http://localhost:8085/api/v1/search/health-check
```

### Service.Analytics
```bash
curl http://localhost:8086/api/v1/analytics/health-check
```

---

## 🚀 Startup Order

### Option 1: Docker Compose

```bash
# Clone the repository
git clone https://github.com/Cptrex/b2b-operations-platform.git
cd b2b-operations-platform

# Start all services
docker-compose up -d

# Check status
docker-compose ps
```

### Option 2: Visual Studio

**Project startup order:**

1. **First, start all Auth.* services:**
   - `Platform.Auth.Service`
   - `Platform.Auth.Business`

2. **Then start other services one by one:**
   - `Platform.Service.Business`
   - `Platform.Service.Orders`
   - `Platform.Service.Notify`
   - `Platform.Service.Search`
   - `Platform.Service.Analytics`

**Steps:**

1. Open `B2BOperationsPlatform.sln` in Visual Studio
2. Right-click on Solution → Properties
3. Select "Configure Startup Projects" → "Multiple startup projects"
4. Set Action to "Start" for required projects in specified order
5. Press F5 to launch

**Or start manually:**

1. Start `Platform.Auth.Service` (F5 or Ctrl+F5)
2. Start `Platform.Auth.Business` (F5 or Ctrl+F5)
3. Wait for Auth services initialization
4. Start other services sequentially

---
