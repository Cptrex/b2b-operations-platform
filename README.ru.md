# B2B Operations Platform

Микросервисная платформа для управления B2B-операциями, построенная на .NET 10.

[English version](README.md)


---

## 💻 Технологический стек

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
```bash
curl http://localhost:8080/api/v1/auth/health-check
```

### Service.Business
```bash
curl http://localhost:8080/health
```

### Service.Orders
```bash
curl http://localhost:8080/health
```

### Service.Notify
```bash
curl http://localhost:8080/health
```

### Service.Search
```bash
curl http://localhost:8080/health
```

### Service.Analytics
```bash
curl http://localhost:8080/health
```

---

## 🚀 Порядок запуска

### Вариант 1: Docker Compose

```bash
# Клонируйте репозиторий
git clone https://github.com/Cptrex/b2b-operations-platform.git
cd b2b-operations-platform

# Запустите все сервисы
docker-compose up -d

# Проверьте статус
docker-compose ps
```

### Вариант 2: Visual Studio

**Порядок запуска проектов:**

1. **Сначала запустите все Auth.* сервисы:**
   - `Platform.Auth.Service`
   - `Platform.Auth.Business`

2. **Затем запустите остальные сервисы один за одним:**
   - `Platform.Service.Business`
   - `Platform.Service.Orders`
   - `Platform.Service.Notify`
   - `Platform.Service.Search`
   - `Platform.Service.Analytics`

**Шаги:**

1. Откройте `B2BOperationsPlatform.sln` в Visual Studio
2. Щелкните правой кнопкой мыши на Solution → Properties
3. Выберите "Configure Startup Projects" → "Multiple startup projects"
4. Установите Action в "Start" для нужных проектов в указанном порядке
5. Нажмите F5 для запуска

**Или запускайте вручную:**

1. Запустите `Platform.Auth.Service` (F5 или Ctrl+F5)
2. Запустите `Platform.Auth.Business` (F5 или Ctrl+F5)
3. Дождитесь инициализации Auth сервисов
4. Запустите остальные сервисы поочередно

---