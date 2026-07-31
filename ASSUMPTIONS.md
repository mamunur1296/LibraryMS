# Design Decisions & Assumptions

This document outlines the key assumptions and design decisions made during the development of the **Library Management System (LibraryMS)**.

## 1. Domain Modeling
- **Physical Book Copies:** We assumed that a library operates with multiple physical copies of the same book. The aggregate root is `Book`, and it contains a collection of `BookCopy` entities. Borrowing operations explicitly target a specific `BookCopy` ID, not just a `Book` ID, to simulate reality.
- **Member Fines:** We assumed fines accumulate on a daily basis. The `DailyFineAccumulationJob` runs nightly to identify overdue active borrows and calculates fines. Once a book is returned, the fine is finalized in the `BorrowRecord`.

## 2. Technical Stack & Infrastructure
- **Framework:** .NET 10 (preview) for the API and React 18 with Vite for the frontend.
- **Data Persistence:** We utilized PostgreSQL as the primary data store due to its robustness and JSONB support if needed for future analytics.
- **Caching:** Redis is used for caching read-heavy endpoints, like the book catalog, and to store temporary user session markers.
- **Background Jobs:** Hangfire is configured using PostgreSQL storage to guarantee that scheduled background tasks (like sending due date reminders and expiring stale reservations) are executed reliably.

## 3. Architecture & Patterns
- **Clean Architecture:** We adhered strictly to the Clean Architecture layout. The domain layer has no external dependencies.
- **CQRS:** All requests to the application layer are segregated into Commands (writes) and Queries (reads).
- **Outbox Pattern:** Sending emails directly within a business transaction is brittle. We assumed high reliability is required for notifications; therefore, the Transactional Outbox pattern is implemented. Events are serialized to the database alongside business changes and dispatched asynchronously.

## 4. Frontend Application
- **Single Page Application (SPA):** We opted for a React SPA architecture rather than server-side rendering, leveraging client-side routing (`react-router-dom`) for a snappy user experience.
- **State Management:** We used standard React Context for Authentication, but relied largely on React Query or raw Axios requests for remote state since the application relies heavily on real-time server truth.
- **Routing:** The application is split into a Public Layout (for guests browsing the catalog) and a Protected Dashboard Layout (for authenticated operations).

## 5. Security & Authorization
- **JWT Authentication:** Authentication is stateless via JSON Web Tokens. Access tokens expire quickly (e.g., 60 minutes), while a long-lived refresh token is securely managed in the database to maintain sessions without compromising security.
- **Role-Based Access Control (RBAC):** We assumed 3 primary roles: `Admin`, `Librarian`, and `Member`. System endpoints are heavily guarded with claims-based authorization attributes depending on the requested action.
