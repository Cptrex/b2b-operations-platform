# B2B Operations Platform

Микросервисная платформа для управления B2B-операциями, построенная на .NET 10 с использованием современных архитектурных паттернов.

## 📋 Оглавление

- [Обзор](#обзор)
- [Архитектура](#архитектура)
- [Микросервисы](#микросервисы)
- [Аутентификация и авторизация](#аутентификация-и-авторизация)
- [Технологический стек](#технологический-стек)
- [Начало работы](#начало-работы)
- [Конфигурация](#конфигурация)
- [Разработка](#разработка)
- [API документация](#api-документация)

---

## 🎯 Обзор

B2B Operations Platform — это масштабируемая платформа для управления бизнес-процессами в сфере B2B. Платформа реализована как набор независимых микросервисов, каждый из которых отвечает за определенную бизнес-функцию.

### Ключевые особенности

- **Микросервисная архитектура** с независимым развертыванием сервисов
- **Двухуровневая аутентификация**: service-to-service и client-to-service
- **RSA-подпись JWT токенов** для безопасной межсервисной коммуникации
- **Event-driven архитектура** на базе RabbitMQ
- **Distributed caching** с использованием Redis
- **Polyglot persistence**: PostgreSQL + MongoDB

---

## 🏗 Архитектура

Платформа использует микросервисную архитектуру с разделением ответственности:

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway (future)                      │
└─────────────────────────────────────────────────────────────────┘
                                 │
                ┌────────────────┼────────────────┐
                │                │                │
        ┌───────▼───────┐ ┌─────▼─────┐ ┌───────▼────────┐
        │ Auth.Service  │ │Auth.Business│ │Service.Business│
        │  (Internal)   │ │  (Client)   │ │   (Core)       │
        └───────┬───────┘ └─────┬─────┘ └───────┬────────┘
                │               │                │
                └───────────────┼────────────────┘
                                │
                    ┌───────────┼───────────┐
                    │           │           │
            ┌───────▼──┐ ┌─────▼─────┐ ┌──▼──────┐
            │ Orders   │ │  Notify   │ │ Search  │
            │ Service  │ │  Service  │ │ Service │
            └──────────┘ └───────────┘ └─────────┘
```

### Слои архитектуры

Каждый сервис следует принципам чистой архитектуры:

- **API (Presentation)**: Контроллеры, DTO, валидация
- **Application**: Бизнес-логика, use cases, сервисы
- **Domain**: Доменные модели, интерфейсы репозиториев
- **Infrastructure**: Реализация репозиториев, внешние зависимости, БД

---

## 🔧 Микросервисы

### Auth.Service
**Порт**: 5257 (dev) / 8080 (prod)

Сервис аутентификации для межсервисного взаимодействия (service-to-service).

**Функции:**
- Выпуск JWT токенов с RSA-подписью для микросервисов
- Управление сервисными учетными данными
- Распространение публичного ключа RSA

**Эндпоинты:**
- `POST /api/v1/internal/token` - Получение service token
- `GET /api/v1/internal/token/health-check` - Health check

**База данных:** PostgreSQL (`auth_service_db`)

---

### Auth.Business
**Порт**: 8080

Сервис аутентификации для клиентских приложений (client-to-service).

**Функции:**
- Авторизация пользователей (логин/пароль)
- Выпуск JWT access и refresh токенов с RSA-подписью
- Управление пользовательскими сессиями
- Публикация событий пользователей в RabbitMQ

**Эндпоинты:**
- `POST /api/v1/auth/token` - Авторизация пользователя
- `POST /api/v1/auth/token/refresh` - Обновление токена

**База данных:** PostgreSQL (`auth_business_db`)  
**Cache:** Redis (для публичных ключей)  
**Messaging:** RabbitMQ (publisher)

---

### Service.Business
**Порт**: 8080

Основной сервис управления бизнес-процессами.

**Функции:**
- Управление бизнес-сущностями
- Обработка событий пользователей из Auth.Business
- Двухуровневая аутентификация (service + client tokens)

**Эндпоинты:**
- Требуют аутентификации через `ServiceBearer` или `ClientBearer`

**База данных:** PostgreSQL (`platform_business_db`) + MongoDB  
**Cache:** Redis  
**Messaging:** RabbitMQ (consumer + publisher)

**Подписки на события:**
- `auth.business.userCreated`
- `auth.business.userUpdated`
- `auth.business.userDeleted`

---

### Service.Orders
**Порт**: 8080

Сервис управления заказами.

**Функции:**
- Управление жизненным циклом заказов
- Интеграция с Service.Business

**База данных:** TBD

---

### Service.Notify
**Порт**: 8080

Сервис уведомлений.

**Функции:**
- Отправка email/SMS/push уведомлений
- Обработка событий для уведомлений

**Messaging:** RabbitMQ (consumer)

---

### Service.Search
**Порт**: 8080

Сервис полнотекстового поиска.

**Функции:**
- Индексация данных
- Полнотекстовый поиск по сущностям

**База данных:** Elasticsearch (планируется)

---

### Service.Analytics
**Порт**: 8080

Сервис аналитики и отчетности.

**Функции:**
- Агрегация данных
- Формирование отчетов


---

## 🔐 Аутентификация и авторизация

Платформа использует **двухуровневую систему аутентификации** на базе JWT с RSA-подписью.

### Service-to-Service Authentication

Используется для межсервисного взаимодействия.

**Процесс:**
1. Сервис запрашивает токен у `Auth.Service` с `serviceId` и `secret`
2. `Auth.Service` проверяет credentials и выпускает JWT, подписанный приватным ключом RSA
3. `Auth.Service` возвращает токен + публичный ключ RSA
4. Сервис сохраняет публичный ключ (`auth_service_public.pem`)
5. Другие сервисы проверяют токен с помощью публичного ключа

**Схема JWT Bearer:** `ServiceBearer`  
**Claim:** `type=service`  
**TTL:** 5 минут

### Client-to-Service Authentication

Используется для аутентификации клиентских приложений.

**Процесс:**
1. Клиент отправляет логин/пароль в `Auth.Business`
2. `Auth.Business` проверяет credentials и выпускает:
   - Access token (60 минут)
   - Refresh token (600 минут)
3. Токены подписаны приватным ключом RSA `Auth.Business`
4. `Auth.Business` публикует публичный ключ в Redis
5. Сервисы получают публичный ключ и проверяют токены клиентов

**Схема JWT Bearer:** `ClientBearer`  
**Claim:** `type=user`  
**Access Token TTL:** 60 минут  
**Refresh Token TTL:** 600 минут

### RSA Key Management

**Генерация ключей:**
- `Auth.Service`: `service_private.pem`, `service_public.pem`
- `Auth.Business`: `business_private.pem`, `business_public.pem`

**Распространение:**
- Auth.Service: через HTTP API + кеширование на диске
- Auth.Business: через Redis (`AuthRedisKeys.JwtClientPublicKeyV1`)

⚠️ **Важно**: Приватные ключи (`*.pem`) добавлены в `.gitignore` и **не должны** коммититься в репозиторий.

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
- **RabbitMQ** (event-driven communication)
- **Redis** (distributed cache)

### Infrastructure
- **PostgreSQL** - Primary database
- **MongoDB** - Document storage
- **Docker** - Containerization
- **RabbitMQ** - Message broker

### Libraries
- **Polly** - Resilience and retry policies
- **StackExchange.Redis** - Redis client

---

## 🚀 Начало работы

### Предварительные требования

- .NET SDK 10.0+
- Docker & Docker Compose
- PostgreSQL 15+
- RabbitMQ 3.12+
- Redis 7+

### Установка

1. **Клонируйте репозиторий:**

```bash
git clone https://github.com/Cptrex/b2b-operations-platform.git
cd b2b-operations-platform
```

2. **Настройте конфигурацию:**

Скопируйте `.env.example` в `.env` для каждого сервиса и заполните значения:

```bash
cp Platform.Auth.Service/.env.example Platform.Auth.Service/.env
cp Platform.Auth.Business/.env.example Platform.Auth.Business/.env
cp Platform.Service.Business/.env.example Platform.Service.Business/.env
# ... для остальных сервисов
```

3. **Запустите инфраструктуру:**

```bash
docker-compose up -d postgres rabbitmq redis mongodb
```

4. **Примените миграции:**

```bash
cd Platform.Auth.Service
dotnet ef database update

cd ../Platform.Auth.Business
dotnet ef database update

cd ../Platform.Service.Business
dotnet ef database update
```

5. **Запустите сервисы:**

**Вариант 1: Через Visual Studio**
- Откройте `B2BOperationsPlatform.sln`
- Выберите несколько проектов для запуска (Properties → Configure Startup Projects)

**Вариант 2: Через командную строку**

```bash
# Terminal 1
cd Platform.Auth.Service
dotnet run

# Terminal 2
cd Platform.Auth.Business
dotnet run

# Terminal 3
cd Platform.Service.Business
dotnet run
```

**Вариант 3: Docker**

```bash
docker-compose up --build
```

### Проверка работоспособности

```bash
# Auth.Service
curl http://localhost:5257/api/v1/internal/token/health-check

# Auth.Business
curl http://localhost:8080/api/v1/auth/health-check

# Service.Business
curl http://localhost:8080/health
```

---

## ⚙️ Конфигурация

### Переменные окружения

Конфигурация сервисов осуществляется через:
- `appsettings.json` - базовая конфигурация
- `appsettings.Development.json` - для локальной разработки
- `.env` - для Docker окружения

#### Пример конфигурации Auth.Service

**appsettings.Development.json:**
```json
{
  "ServiceJwt": {
    "Issuer": "auth.service",
    "PrivateKeyPath": "service_private.pem",
    "PublicKeyPath": "service_public.pem",
    "ExpiresMinutes": 5
  },
  "SERVICE_CREDENTIALS": {
    "service.orders": "ORDERS_SECRET",
    "service.business": "BUSINESS_SECRET"
  },
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=auth_service_db;Username=auth_user;Password=auth_password"
  }
}
```

**.env:**
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

ServiceJwt__Issuer=auth.service
ServiceJwt__PrivateKeyPath=service_private.pem
ServiceJwt__PublicKeyPath=service_public.pem
ServiceJwt__ExpiresMinutes=5

SERVICE_CREDENTIALS__service.orders=ORDERS_SECRET
SERVICE_CREDENTIALS__service.business=BUSINESS_SECRET

ConnectionStrings__Postgres=Host=postgres;Port=5432;Database=auth_service_db;Username=auth_user;Password=auth_password
```

### RabbitMQ конфигурация

Каждый сервис может быть настроен как publisher и/или consumer:

```json
{
  "RabbitMQ": {
    "Host": "rabbitmq",
    "Port": 5672,
    "Username": "svc_business",
    "Password": "STRONG_PASSWORD",
    "ExchangeName": "platform.events",
    "QueueName": "service-business.events",
    "PrefetchCount": 16,
    "BindingKeys": [
      "auth.business.userCreated",
      "auth.business.userUpdated"
    ],
    "VirtualHost": "/platform"
  }
}
```

### Redis конфигурация

```json
{
  "Redis": {
    "Host": "redis",
    "Port": 6379,
    "Password": ""
  }
}
```

---

## 🛠 Разработка

### Структура проекта

```
B2BOperationsPlatform/
├── Platform.Auth.Service/           # Service-to-service auth
│   ├── Controllers/
│   ├── Services/
│   ├── Dto/
│   └── Program.cs
├── Platform.Auth.Business/          # Client auth
│   ├── Api/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   └── Program.cs
├── Platform.Service.Business/       # Core business service
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   └── Program.cs
├── Platform.Shared.Messaging/       # RabbitMQ shared lib
├── Platform.Shared.Cache/           # Redis shared lib
├── Platform.Shared.Results/         # Result pattern lib
└── Paltform.Auth.Shared/           # JWT/RSA shared lib
    ├── JwtToken/
    │   ├── RsaServiceTokenIssuer.cs
    │   ├── RsaClientTokenIssuer.cs
    │   └── Extensions/
    └── Cryptography/
```

### Добавление нового микросервиса

1. **Создайте проект:**

```bash
dotnet new webapi -n Platform.Service.NewService
cd Platform.Service.NewService
dotnet add reference ../Platform.Shared.Messaging/Platform.Shared.Messaging.csproj
```

2. **Настройте аутентификацию:**

```csharp
// Program.cs
builder.Services.AddAuthentication()
    .AddJwtBearer("ServiceBearer", options => {
        // Configure RSA validation for Auth.Service tokens
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Internal", policy =>
        policy.AddAuthenticationSchemes("ServiceBearer")
              .RequireAuthenticatedUser()
              .RequireClaim("type", "service"));
```

3. **Настройте RabbitMQ (если нужен):**

```csharp
builder.Services.AddRabbitMqConsumer(builder.Configuration);
builder.Services.AddSingleton<IRabbitMqMessageConsumer, NewServiceConsumer>();
```

4. **Добавьте конфигурацию:**

Создайте `appsettings.Development.json`, `.env`, `.env.example`

### Соглашения о коде

- Используйте **Result pattern** для обработки ошибок (Platform.Shared.Results)
- Применяйте **CancellationToken** в асинхронных методах
- Именование: `PascalCase` для публичных членов, `_camelCase` для приватных полей

### Тестирование

```bash
# Запуск unit тестов
dotnet test

# Запуск с покрытием
dotnet test /p:CollectCoverage=true
```

---

## 📚 API документация

### Auth.Service API

#### POST /api/v1/internal/token
Получение service token для межсервисного взаимодействия.

**Request:**
```json
{
  "serviceId": "service.business",
  "secret": "BUSINESS_SECRET"
}
```

**Response 200:**
```json
{
  "token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-01T12:05:00Z",
  "publicKey": "-----BEGIN PUBLIC KEY-----\n..."
}
```

---

### Auth.Business API

#### POST /api/v1/auth/token
Авторизация пользователя.

**Request:**
```json
{
  "login": "user@example.com",
  "password": "SecurePassword123",
  "businessId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response 200:**
```json
{
  "isSuccess": true,
  "value": {
    "accessToken": {
      "token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2024-01-01T13:00:00Z"
    },
    "refreshToken": {
      "token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2024-01-01T22:00:00Z"
    }
  },
  "error": null
}
```

**Response 401:**
```json
{
  "isSuccess": false,
  "value": null,
  "error": {
    "message": "Login or password are incorrect",
    "category": "Unauthorized"
  }
}
```

#### POST /api/v1/auth/token/refresh
Обновление access token с помощью refresh token.

**Request:**
```json
{
  "refreshToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### Service.Business API

Все эндпоинты требуют JWT токен в заголовке `Authorization: Bearer <token>`

**Policies:**
- `Internal` - для service-to-service запросов
- `Client` - для клиентских запросов

---

## 🔒 Безопасность

### Рекомендации

1. **RSA Keys:**
   - Генерируйте уникальные ключи для каждого окружения
   - Храните приватные ключи в безопасном хранилище
   - Ротируйте ключи регулярно

2. **Credentials:**
   - Используйте `.env` файлы для Docker

3. **Database:**
   - Используйте отдельных пользователей БД для каждого сервиса
   - Ограничьте привилегии (только необходимые таблицы)
   - Включите SSL/TLS для подключений к БД

4. **RabbitMQ:**
   - Создайте отдельного пользователя для каждого сервиса
   - Используйте Virtual Hosts для изоляции
   - Настройте SSL/TLS

### Обновление зависимостей

```bash
# Проверка устаревших пакетов
dotnet list package --outdated

# Обновление всех пакетов
dotnet add package <PackageName> --version <NewVersion>
```

---

## 📝 Roadmap

- [ ] Реализация JWKS endpoint для динамического получения публичных ключей
- [ ] Refresh token rotation
- [ ] Добавление API Gateway (Ocelot/YARP)
- [ ] Распределенный трейсинг (OpenTelemetry)
- [ ] Централизованное логирование
- [ ] Health checks dashboard
- [ ] Integration tests
- [ ] API Rate Limiting
- [ ] GraphQL Gateway

---