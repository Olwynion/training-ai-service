# training-ai-service

Микросервис генерации тренировочных планов через AI.

## Назначение

Генерация тренировочных планов на основе промпта пользователя и его доступных упражнений. Использует OpenAI API (GPT-4o). Хранит историю генераций в PostgreSQL через Dapper.

## Как работает

Onion-архитектура (Domain → Infrastructure → Services → API).
gRPC запросы принимаются на порту 5004, проходят через MediatR CQRS Handlers, бизнес-логика выполняется в сервисах (OpenAI) и репозиториях (Dapper SQL).

## ID

Все ID сущностей — `long` (BIGSERIAL в БД). Автоинкремент.

## Язык

Интерфейсы и содержимое планов — на русском языке. Все названия дней, упражнений, описания генерируются на русском.

## Команды

```bash
# Build
dotnet build

# Test
dotnet test

# Run (требуется Docker с БД и OpenAI API key)
dotnet run --project src/Training.AI

# Миграции
goose -dir src/Training.AI.Domain/Migrations postgres "host=localhost port=5432 user=postgres password=postgres dbname=training_ai sslmode=disable" up
```

Подробнее: см. `proto/contracts/AGENTS.md`
