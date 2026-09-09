# C# & .NET Beginner to Advanced: Complete Exhaustive Topic Checklist

> **Note:** এটি C# এবং পুরো .NET ইকোসিস্টেমের Beginner থেকে Extreme Advanced লেভেল পর্যন্ত সকল টপিক ও সাব-টপিকের একটি স্বয়ংসম্পূর্ণ (Exhaustive) রেফারেন্স চেকলিস্ট।

---

## Level 1: C# Language Fundamentals (Beginner)

### 1. C# & .NET Environment Basics
* .NET Architecture (CLR, BCL, CTS, CLS)
* Compilation Pipeline (Source Code → Roslyn Compiler → CIL/MSIL → JIT Compiler → Native Machine Code)
* CLI Tooling (`dotnet new`, `build`, `run`, `test`, `publish`, `tool`)
* Project Anatomy & Configuration (`.csproj`, `.sln`, `bin/`, `obj/`, Target Framework Moniker / TFM)

### 2. Data Types & Variables
* Value Types vs Reference Types
* CTS Primitives (`byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `decimal`, `bool`, `char`)
* Type Conversion & Casting (Implicit, Explicit, `Convert`, `Parse`, `TryParse`)
* Nullable Value Types (`int?`, `Nullable<T>`)
* Nullable Reference Types (`string?`, `#nullable enable/disable`)
* Implicitly Typed Local Variables (`var`)
* Immutability Modifiers (`const`, `readonly`)

### 3. Operators & Expressions
* Arithmetic, Relational, Logical & Bitwise Operators
* Null-Handling Operators (`??`, `??=`, `?.`, `?[]`, `!`)
* Type Inspection & Cast Operators (`is`, `as`, `typeof`, `sizeof`, `nameof`)
* Pattern Matching Operators & Expressions (Declaration, Type, Constant, Relational, Logical `and`/`or`/`not`)

### 4. Control Flow Statements
* Conditional Branching (`if-else`, traditional `switch`, modern `switch` expression)
* Iteration Loops (`for`, `while`, `do-while`, `foreach`)
* Jump Statements (`break`, `continue`, `return`, `goto`)

### 5. Methods & Parameters
* Method Signatures, Return Values & Overloading
* Parameter Modifiers (`ref`, `out`, `in`, `params`, `ref readonly`)
* Optional & Named Arguments
* Local Functions (Static & Non-static)
* Expression-bodied Members (`=>`)

### 6. Strings & Text Processing
* String Immutability & String Interning
* String Interpolation (`$""`), Verbatim Strings (`@""`), Raw String Literals (`""" """`)
* `StringBuilder` (Capacity, Buffer Management)
* String Comparison, Ordinal vs Linguistic, `StringComparison`, `CultureInfo`
* Regular Expressions (`Regex`, Source-generated `[GeneratedRegex]`)

### 7. Error & Exception Handling
* Structured Exception Handling (`try`, `catch`, `finally`)
* Standard BCL Exceptions (`ArgumentNullException`, `InvalidOperationException`, `IndexOutOfRangeException`, etc.)
* Custom Application Exception Development
* Exception Filters (`catch ... when (...)`)
* Stack Trace Preservation (`throw;` vs `throw ex;`)

---

## Level 2: Object-Oriented Programming (OOP)

### 8. Classes, Objects & Structs
* Classes (Reference Type) vs Structs (Value Type)
* Constructors (Default, Parameterized, Copy, Static, Primary Constructors)
* Object & Collection Initializers
* Deconstructors (`Deconstruct` method)
* Struct Variants (`readonly struct`, `ref struct`)
* Records (`record class`, `record struct`, Non-destructive mutation via `with`)

### 9. Encapsulation & Member Design
* Access Modifiers (`public`, `private`, `protected`, `internal`, `protected internal`, `private protected`)
* Fields vs Properties (Auto-properties, Backing fields)
* Mutation Control (`init`-only setters, `required` properties)
* Indexers (`this[]`, multi-dimensional indexers)

### 10. Inheritance & Polymorphism
* Base & Derived Classes (`base`, `this` keywords)
* Member Modifiers (`virtual`, `override`, `new` method hiding, `sealed`)
* Abstract Classes vs Abstract Methods
* Root Type Mechanics: `System.Object` (`ToString`, `Equals`, `GetHashCode`, `GetType`, `MemberwiseClone`)

### 11. Interfaces
* Interface Declaration & Contract Implementation
* Multiple Interface Inheritance
* Explicit Interface Implementation
* Default Interface Methods (DIM)
* Static Abstract Members in Interfaces (Generic Math / Static Virtuals)

---

## Level 3: Generics, Collections & Functional Features

### 12. Generics
* Generic Classes, Interfaces, Structs & Methods
* Generic Type Constraints (`where T : class`, `struct`, `notnull`, `unmanaged`, `new()`, BaseClass, Interface)
* Variance in Generics (Covariance `out`, Contravariance `in`)

### 13. Collections & Data Structures
* Generic Core Collections (`List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`, `Queue<T>`, `Stack<T>`, `LinkedList<T>`, `SortedSet<T>`)
* Concurrent Collections (`ConcurrentDictionary`, `ConcurrentQueue`, `ConcurrentBag`, `BlockingCollection`)
* Immutable Collections (`ImmutableList<T>`, `ImmutableArray<T>`, `ImmutableDictionary`)
* Frozen Collections (`FrozenDictionary`, `FrozenSet` in .NET 8+)
* Collection Interfaces (`IEnumerable<T>`, `ICollection<T>`, `IList<T>`, `IReadOnlyList<T>`, `IDictionary<K,V>`)

### 14. Delegates, Events & Lambdas
* Custom Delegate Definitions
* Built-in Function Delegates (`Action<...>`, `Func<...>`, `Predicate<T>`)
* Anonymous Methods & Lambda Expressions
* Closures, Variable Capture & Allocation Cost
* Events, Event Accessors & `EventHandler<TEventArgs>`

### 15. Iterators & Enumeration
* `IEnumerable` & `IEnumerator` Protocols
* `yield return` & `yield break`
* Iterator State Machine Generated by Roslyn

### 16. LINQ (Language Integrated Query)
* Query Syntax vs Method Syntax (Fluent API)
* Standard Query Operators (Projection, Filtering, Ordering, Grouping, Joining, Aggregations, Partitioning)
* Deferred Execution (Lazy) vs Immediate Execution (Eager)
* `IEnumerable<T>` vs `IQueryable<T>`
* Custom LINQ Extension Method Authoring

### 17. Modern C# Language Constructs
* Tuples & ValueTuples (`(int, string)`, Named elements, Deconstruction)
* Advanced Pattern Matching (Property, Positional, Tuple, List Patterns)
* Extension Methods & Static Extension Classes
* Global Using Directives & File-Scoped Namespaces
* Null-Coalescing Assignment (`??=`)

---

## Level 4: Concurrency, Asynchrony & Multithreading

### 18. Asynchronous Programming (TAP)
* `async` and `await` Language Semantics
* Return Types: `Task`, `Task<T>`, `ValueTask`, `ValueTask<T>`, `void`
* Async State Machine Under the Hood (Struct-based compiler rewrite)
* Cooperative Cancellation (`CancellationTokenSource`, `CancellationToken`, Linked Tokens)
* Progress Notification (`IProgress<T>`)
* Asynchronous Streams (`IAsyncEnumerable<T>`, `await foreach`, `[EnumeratorCancellation]`)
* Task Combinators (`Task.WhenAll`, `Task.WhenAny`, `Task.Delay`, `Task.Yield`)
* Synchronization & Context Capture (`ConfigureAwait(false)`, `SynchronizationContext`)

### 19. Threading & Thread Pool Mechanics
* `Thread` Class Lifecycle & State
* CLR ThreadPool Engine (Global Queue, Local Work-Stealing Queues, Hill Climbing Algorithm)
* Foreground Threads vs Background Threads
* Thread Affinity & Execution Context Flow (`ExecutionContext.SuppressFlow`)

### 20. Synchronization & Locking Primitives
* Monitor-based Locking (`lock` statement, `Monitor.Enter/Exit`)
* Atomic Operations via `Interlocked` (`Increment`, `CompareExchange`, `Add`)
* Asynchronous Synchronization (`SemaphoreSlim`)
* OS-level Kernel Synchronization (`Mutex`, `Semaphore`, `EventWaitHandle`)
* Signaling Primitives (`AutoResetEvent`, `ManualResetEventSlim`, `CountdownEvent`, `Barrier`)
* Concurrency Hazards (Deadlock, Livelock, Race Conditions, Thread Starvation)

### 21. Parallel Computing & Reactive Pipelines
* Task Parallel Library (TPL)
* Parallel Execution (`Parallel.For`, `Parallel.ForEach`, `Parallel.ForEachAsync`)
* Parallel LINQ (PLINQ, Degree of Parallelism, Buffering)
* Producer-Consumer Channels (`System.Threading.Channels`)
* TPL Dataflow Pipelines (`BufferBlock`, `TransformBlock`, `ActionBlock`)

---

## Level 5: Memory Management, CLR Internals & Low-Level

### 22. Memory Architecture & Garbage Collection (GC)
* Stack Allocation vs Managed Heap Allocation
* GC Generational Model (Generation 0, Generation 1, Generation 2)
* Specialized Heaps (Large Object Heap / LOH, Pinned Object Heap / POH)
* GC Flavors (Workstation GC vs Server GC, Background GC, Concurrent GC)
* GC Phases (Mark, Sweep, Compact)
* Resource Cleanup (`IDisposable`, `IAsyncDisposable`, `Dispose(bool)` Pattern, Finalizers `~ClassName()`)
* GC Programmatic Control (`GC.Collect`, `GC.SuppressFinalize`, `GC.KeepAlive`, `WeakReference<T>`)

### 23. High-Performance & Zero-Allocation C#
* `Span<T>` and `ReadOnlySpan<T>` (Contiguous memory representation)
* `Memory<T>` and `ReadOnlyMemory<T>` (Heap-safe span abstractions)
* Memory Borrowing (`MemoryMarshal`, `MemoryPool<T>`)
* Array Reuse via `ArrayPool<T>.Shared`
* Stack Allocation (`stackalloc`, `InlineArrayAttribute` in .NET 8+)
* Hardware Acceleration (SIMD, `Vector<T>`, Intrinsics)

### 24. Unsafe Code, Pointers & Interoperability
* `unsafe` Blocks & Pointer Arithmetic (`*`, `&`, `->`)
* Buffer Pinning (`fixed` statement)
* Native Interop / P-Invoke (`DllImportAttribute`, Source-generated `LibraryImportAttribute`)
* Unmanaged Function Pointers (`delegate*<...>`)
* Native AOT (Ahead-of-Time Compilation, Trimming, Illink)

---

## Level 6: Metaprogramming, Code Generation & Dynamic Execution

### 25. Reflection & Metadata
* `System.Type` & Metadata Exploration
* Assembly Loading (`Assembly`, `AssemblyLoadContext`, ALC isolation)
* Member Inspection (`PropertyInfo`, `MethodInfo`, `FieldInfo`, `ConstructorInfo`)
* Dynamic Invocation (`Activator.CreateInstance`, `MethodInfo.Invoke`)
* Custom Attributes (`AttributeUsageAttribute`, Reading via Reflection & `CustomAttributeData`)

### 26. Dynamic Code Generation & Expressions
* Expression Trees (`System.Linq.Expressions`, Expression Visitors)
* Dynamic Lambda Compilation (`Compile()`)
* Dynamic Language Runtime (DLR, `dynamic` keyword, `ExpandoObject`, `DynamicObject`)
* Dynamic IL Generation (`System.Reflection.Emit`, `ILGenerator`)
* Roslyn Compiler Platform APIs & Custom Roslyn Analyzers
* Roslyn Source Generators (`IIncrementalGenerator`)
* Interceptors (C# 12+)

---

## Level 7: File I/O, Serialization & Networking

### 27. File System & Stream Architecture
* File and Directory Management (`File`, `Directory`, `Path`, `FileInfo`, `DirectoryInfo`)
* Stream Abstractions (`FileStream`, `MemoryStream`, `BufferedStream`, `DeflateStream`, `GZipStream`)
* Character & Binary Handlers (`StreamReader`, `StreamWriter`, `BinaryReader`, `BinaryWriter`)
* High-Performance I/O Pipelines (`System.IO.Pipelines`)

### 28. Serialization & Data Formats
* JSON Ecosystem (`System.Text.Json`, JSON DOM, Serializer Source Generators)
* XML Processing (`XmlSerializer`, `XDocument`, `XmlReader`)
* Binary & High-Efficiency Serialization (Protobuf, MessagePack)

### 29. Networking & Protocol Communication
* Modern HTTP Client (`HttpClient`, `IHttpClientFactory`, `SocketsHttpHandler`)
* Socket & Transport Programming (`TcpClient`, `TcpListener`, `Socket`)
* Real-Time Web Protocols (WebSockets, Server-Sent Events / SSE)
* Remote Procedure Calls via gRPC & Protobuf Contracts

---

## Level 8: ASP.NET Core & Web API Ecosystem

### 30. Core Web Runtime & Hosting
* ASP.NET Core Request Pipeline & Execution Order
* Middleware Architecture (Built-in, Inline `app.Use`, Class-based, Factory-based Middleware)
* Web Server Hosting (Kestrel, HTTP.sys, Reverse Proxy Setup, In-Process vs Out-of-Process)
* Application Bootstrapping (`WebApplicationBuilder`, `Program.cs`)
* Hierarchical Configuration (`appsettings.json`, Environment Variables, Key-Per-File, Command-line args)
* Strongly-Typed Options Pattern (`IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`)
* Structured Logging (`ILogger<T>`, Serilog, OpenTelemetry Loggers, Log Enrichment)

### 31. Dependency Injection (DI) Engine
* Inversion of Control (IoC) & Service Collection
* Lifetime Management (`Transient`, `Scoped`, `Singleton`)
* Keyed Services (.NET 8+ `[FromKeyedServices]`)
* DI Safety: Captive Dependencies, Root Scope Validation
* Advanced DI Registrations (Factory methods, Open generics, Decorator pattern wiring)

### 32. Web API Architecture
* Controllers vs Minimal APIs (Design differences, performance)
* Routing Engine (Route Templates, Route Constraints, Catch-all, Regex constraints)
* Model Binding & Validation (`[FromBody]`, `[FromQuery]`, FluentValidation integration)
* Filters (Action, Result, Resource, Exception Filters, Endpoint Filters)
* Content Negotiation, Formatters & Problem Details (`RFC 7807`)
* OpenAPI Specification & Documentation (`Scalar`, `Swashbuckle`, Native .NET 9 OpenAPI)
* API Traffic Control (Rate Limiting, Output Caching, Response Caching)
* Real-Time Push with SignalR (Hubs, Groups, Connection Lifecycles, Redis Backplane)

---

## Level 9: Data Access & ORM (EF Core & Dapper)

### 33. Low-Level & Micro-ORM Data Access
* ADO.NET Foundations (`DbConnection`, `DbCommand`, `DbDataReader`, `DbTransaction`)
* Dapper Micro-ORM (Direct SQL Execution, Multi-Mapping, Dynamic Parameters, Stored Procedures)

### 34. Entity Framework Core (EF Core)
* `DbContext` Lifecycle, Pooling & Thread Safety
* Entity Modeling (Data Annotations vs Fluent API, `IEntityTypeConfiguration<T>`)
* Database Migrations, Script Generation & Schema Deployment
* Entity Relationship Modeling (One-to-One, One-to-Many, Many-to-Many, Self-referencing)
* Change Tracker Architecture & Entity States (`Added`, `Modified`, `Deleted`, `Unchanged`, `Detached`)
* Loading Strategies (Eager Loading with `Include`/`ThenInclude`, Explicit Loading, Lazy Loading)
* Query Performance (Split Queries `AsSplitQuery`, Non-tracking `AsNoTracking`, `AsNoTrackingWithIdentityResolution`)
* Shadow Properties, Owned Entity Types & Backing Fields
* Global Query Filters (Soft Delete, Multi-Tenant Isolation)
* Interceptors (`DbCommandInterceptor`, `SaveChangesInterceptor`)
* Concurrency Resolution (Optimistic Concurrency, `RowVersion`, Concurrency Tokens)

---

## Level 10: Security, Identity & Cryptography

### 35. Authentication & Authorization
* Cookie Authentication & Session State
* JWT Token Architecture (Access Tokens, Refresh Tokens, Claims Principal)
* Modern Protocol Standards (OAuth 2.0, OpenID Connect / OIDC)
* Identity Management Frameworks (ASP.NET Core Identity, Duende IdentityServer, Keycloak)
* Role-Based Access Control (RBAC)
* Policy-Based & Resource-Based Authorization (`IAuthorizationRequirement`, `AuthorizationHandler<T>`)

### 36. Application & Network Defense
* Cross-Origin Resource Sharing (CORS Policy Configuration)
* Web Attack Mitigations (CSRF/XSRF, XSS, SQL Injection, Parameter Tampering)
* Data Protection API (DPAPI, Key Persistence & Ring Rotation)
* Secrets & Credential Management (User Secrets, Azure Key Vault, HashiCorp Vault)
* Cryptographic Services (`System.Security.Cryptography`: Symmetric AES, Asymmetric RSA, Hashing SHA256/HMAC, Password Hashing Argon2/BCrypt)

---

## Level 11: Architecture, Design Patterns & Enterprise Practices

### 37. Software Design Principles & GoF Patterns
* SOLID Principles (SRP, OCP, LSP, ISP, DIP)
* Creational Patterns (Factory Method, Abstract Factory, Builder, Singleton)
* Structural Patterns (Adapter, Decorator, Facade, Composite, Proxy)
* Behavioral Patterns (Strategy, Observer, Command, Mediator, Chain of Responsibility)

### 38. Enterprise Architectural Patterns
* Clean Architecture / Onion Architecture / Hexagonal (Ports & Adapters)
* Domain-Driven Design (DDD: Entities, Value Objects, Aggregates, Domain Events, Repositories, Specifications)
* CQRS (Command Query Responsibility Segregation) & Mediator Pattern (MediatR)
* Vertical Slice Architecture (Feature-driven slices)
* Event-Driven Architecture (Message Brokers: RabbitMQ, Apache Kafka, Azure Service Bus, MassTransit)
* Modular Monolith vs Distributed Microservices

### 39. Resilience & Reliability Engineering
* Fault Tolerance via Polly / .NET Resilience Pipeline (Retry, Circuit Breaker, Fallback, Timeout, Rate Limiter, Hedging)
* Health Check Systems (`Microsoft.Extensions.Diagnostics.HealthChecks`, UI Dashboards, Readiness/Liveness Probes)

---

## Level 12: Testing, Observability & Deployment

### 40. Testing & Code Quality Assurance
* Unit Testing (xUnit, NUnit)
* Mocking & Substitution Frameworks (Moq, NSubstitute)
* Fluent Assertions (FluentAssertions)
* Integration Testing (`WebApplicationFactory<TEntryPoint>`, Testcontainers for Dockerized DBs)
* Architecture Validation Testing (NetArchTest)
* Performance Benchmarking (BenchmarkDotNet)

### 41. Observability, Diagnostics & Profiling
* Unified Observability via OpenTelemetry (Traces, Metrics, Logs)
* Distributed Tracing Mechanics (`ActivitySource`, `Activity`, W3C TraceContext)
* High-Performance Application Metrics (`Meter`, `Counter<T>`, `Histogram<T>`)
* Runtime Profiling & Memory Diagnostics (dotMemory, dotTrace, PerfView, `dotnet-dump`, `dotnet-trace`, `dotnet-gcdump`)

### 42. Packaging, Containerization & Cloud Deployment
* NuGet Package Authoring & Publishing (`.nupkg`, Source Link, Symbol Packages)
* Containerization (Multi-stage Dockerfiles, Distroless / Chiseled Ubuntu Images for .NET)
* Continuous Integration & Deployment (GitHub Actions, Azure DevOps CI/CD)
* .NET Aspire (Cloud-ready distributed application orchestration, service discovery, dashboard telemetry)
