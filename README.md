# Library Management System (LibraryMS)

Welcome to the **Library Management System (LibraryMS)**. This project is a modern, enterprise-grade, full-stack application built using a clean architecture pattern with .NET 10 on the backend and React (Vite) on the frontend.

---

## 🏗️ Architecture & Design Patterns

The backend follows **Clean Architecture** (Domain-Driven Design principles) and incorporates several advanced patterns:

1. **Transactional Outbox Pattern (with Queue & Retry):**
   - Domain events (e.g. borrowing confirmations, reservation notifications) are serialized and saved to the `OutboxMessages` table in the same transaction as the business data, guaranteeing **at-least-once delivery**.
   - A Hangfire background worker polls this table, dispatches events via MediatR, and tracks retries with exponential backoff.
2. **Chain of Responsibility (CoR) / Pipeline Pattern:**
   - MediatR pipeline behaviors process requests sequentially through a pipeline: `LoggingBehavior` ➡️ `RetryBehavior` (with exponential backoff) ➡️ `ValidationBehavior` ➡️ request handler.
3. **Adapter / Wrapper Pattern:**
   - External dependencies are decoupled using interfaces:
     - `IEmailService` (wrapped with MailKit)
     - `ICacheService` (wrapped with StackExchange.Redis)
     - `IReportExportService` (strategy wrapping ClosedXML for Excel and QuestPDF for PDF)
4. **Options Pattern:**
   - Configuration settings (SMTP, Redis) are strongly-typed and bound using `IOptions<T>`.

---

## 🚀 Quick Start with Docker Compose (Recommended)

You can run the entire ecosystem (Database, Redis, Migrations, Backend API, and Frontend App) with a single command.

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

### Spin up the Application
Run the following command in the root directory:
```bash
docker-compose up --build
```

Docker Compose will launch the containers in the correct dependency order:
1. **`library_ms_db` (PostgreSQL 15)**: Starts up and runs a healthcheck.
2. **`library_ms_redis` (Redis 7)**: Starts up and runs a healthcheck.
3. **`library_ms_migrator` (DbMigrator)**: Runs database migrations and seeds initial database records (Admin user, seed data) once the database is healthy, then exits.
4. **`library_ms_api` (Web API Host)**: Starts up once the migrations successfully run. It binds to port `8080` (Hangfire dashboard available at `http://localhost:8080/hangfire`).
5. **`library_ms_frontend` (React/Vite)**: Starts up and serves the React frontend on `http://localhost:3000`.

---

## 🛠️ Local Development Setup

If you prefer to run services manually for local development:

### 1. Database & Caching Setup
Configure your local PostgreSQL and Redis instances, and update the connection strings in:
- `src/LibraryMS.HttpApi.Host/appsettings.json`

Default PostgreSQL credentials:
- **Host:** `localhost`
- **Port:** `5432`
- **Database:** `LibraryMS`
- **User:** `postgres`
- **Password:** `2025`

### 2. Apply Migrations & Seed Data
Run the DbMigrator console application to initialize your database schema and seed initial data:
```bash
dotnet run --project src/LibraryMS.DbMigrator
```

### 3. Run Backend API Host
Start the backend web host:
```bash
dotnet run --project src/LibraryMS.HttpApi.Host
```
- Swagger API Docs: `http://localhost:8080/swagger`
- Hangfire Dashboard: `http://localhost:8080/hangfire`

### 4. Run Frontend
Navigate to the `frontend` directory, install packages, and start the Vite development server:
```bash
cd frontend
npm install
npm run dev
```
- Frontend Client: `http://localhost:3000`

---

## 📈 System Features
- **Book Catalog:** Full CRUD operations with author/category tagging, advanced search, and real-time availability checking.
- **Member Management:** Manage library membership, status tracking, and history logs.
- **Borrowing Engine:** Borrow, return, fine calculations, and automated overdue detection.
- **Reservation Queue:** Real-time queue for popular books with reservation expiration.
- **Reporting Engine:** Generate and export overdue books and popular books reports to Excel (ClosedXML) or PDF (QuestPDF).
- **Background Automation:** Hangfire-managed jobs for overdue checking, reservation expiry, and Outbox dispatch.
