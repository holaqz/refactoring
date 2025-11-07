# Courier Management System (C#)

Этот репозиторий содержит C#-порт сервиса управления доставками. Ниже описан минимальный набор действий, чтобы локально поднять API и прогнать интеграционные тесты.

## Требования

- **.NET SDK 9.0** (минимум 9.0.100). Проверить: `dotnet --version`
- **SQL Server** или другая СУБД, совместимая с Entity Framework (по умолчанию используется InMemory для тестов и SQLite/SQL Server — см. `appsettings.json`)
- **Git**
- Опционально: **Visual Studio 2022** (17.9+) или **Rider** / **VS Code** с C#-плагином

## Настройка окружения

1. Установите .NET SDK 9:
   ```bash
   winget install Microsoft.DotNet.SDK.9
   ```

2. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/your-org/bad-code-repository-sharp.git
   cd bad-code-repository-sharp
   ```

3. Проверьте конфигурацию строки подключения в `CourierManagementSystem.Api/appsettings.json`. По умолчанию используется локальная база. При необходимости замените `ConnectionStrings:DefaultConnection`.

4. Выполните восстановление зависимостей:
   ```bash
   dotnet restore
   ```

## Запуск API

```bash
dotnet run --project CourierManagementSystem.Api
```

API будет доступен по адресу `https://localhost:5001` (или `http://localhost:5000`). При запуске в профиле Development включается Swagger UI.

## Прогон тестов

```bash
dotnet test CourierManagementSystem.sln
```

Интеграционные тесты (`CourierManagementSystem.Tests`) используют InMemory-базу данных и поднимают минимальный WebApplicationFactory.

## Структура решения

- `CourierManagementSystem.Api` — основной сервис (контроллеры, сервисы, репозитории)
- `CourierManagementSystem.Tests` — интеграционные и сервисные тесты на xUnit/Moq

## Полезные команды

```bash
# сборка
dotnet build

# запуск API с профилем Production
dotnet run --project CourierManagementSystem.Api --configuration Release

# публикация
dotnet publish CourierManagementSystem.Api -c Release -o ./publish
```

## Связанные проекты

Оригинальная версия Kotlin доступна в каталоге `D:\program\backend\kotlin\bad-code-repository`. C# версия синхронизирует тестовые сценарии, данные и API-объекты с исходным проектом.
