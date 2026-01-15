# Platform.Service.Search

Сервис поиска для платформы B2B Operations Platform.

## Функциональность

Сервис предоставляет возможность поиска:
1. **Бизнеса** по имени
2. **Пользователя** по имени (username)
3. **Аккаунта** по логину, email или имени

## Архитектура

Сервис использует DDD (Domain-Driven Design) паттерн:
- **Domain Entities** - модели данных (Business, User, Account)
- **Repositories** - доступ к данным
- **Application Services** - бизнес-логика поиска
- **Infrastructure** - БД и обработка событий RabbitMQ
- **API** - REST endpoints

## Transactional Outbox Pattern

Сервис реализует паттерн Transactional Outbox для надежной публикации событий:

### Inbox Messages
- Все входящие события сохраняются в таблицу `inbox_messages`
- Гарантируется идемпотентность обработки событий (по `event_id`)

### Outbox Messages
- События для публикации сохраняются в таблицу `outbox_messages`
- Background service `OutboxPublisherBackgroundService` периодически читает непубликованные сообщения
- После успешной публикации в RabbitMQ, заполняется поле `published_at`
- Поддержка retry механизма (до 5 попыток) с записью ошибок в `last_error`
- Publisher использует `mandatory=true` для гарантии доставки

### ACK и надежность
- RabbitMQ настроен на persistent сообщения
- Канал использует `mandatory=true` флаг
- При успешной отправке в RabbitMQ, outbox сообщение помечается как опубликованное (`published_at`)
- В случае ошибки, увеличивается счетчик retry и сохраняется текст ошибки

## События RabbitMQ

Сервис слушает следующие события:

### business.businessCreated
Событие создания бизнеса из Service.Business

### auth.service.userCreated
Событие создания пользователя из Auth.Service

### auth.business.accountCreated
Событие создания аккаунта из Auth.Business

### auth.business.accountDeleted
Событие удаления аккаунта из Auth.Business

## API Endpoints

### GET /api/search/business?name={name}
Поиск бизнеса по имени

### GET /api/search/user?name={name}
Поиск пользователя по username

### GET /api/search/account/login?login={login}
Поиск аккаунта по логину

### GET /api/search/account/email?email={email}
Поиск аккаунта по email

### GET /api/search/account/name?name={name}
Поиск аккаунта по имени

## База данных

Сервис использует PostgreSQL для хранения данных поиска.

### Таблицы:
- **businesses** - бизнесы
- **users** - пользователи
- **accounts** - аккаунты
- **inbox_messages** - входящие события (для идемпотентности)
- **outbox_messages** - исходящие события (для гарантированной публикации)

## Настройка

В appsettings.json необходимо настроить:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=search_db;Username=postgres;Password=password"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "ExchangeName": "platform.events",
    "QueueName": "search-queue",
    "BindingKeys": [
      "business.businessCreated",
      "auth.service.userCreated",
      "auth.business.accountCreated",
      "auth.business.accountDeleted"
    ]
  }
}
```

## Миграции БД

Для создания базы данных выполните:

```bash
dotnet ef migrations add InitialCreate --project Platform.Service.Search
dotnet ef database update --project Platform.Service.Search
```

