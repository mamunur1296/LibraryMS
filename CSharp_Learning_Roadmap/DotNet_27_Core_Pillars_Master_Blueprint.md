# 👑 C# & .NET 27 Core Pillars: The Ultimate Master Blueprint (Beginner to 10+ Years Architect Level)

এই ডকুমেন্টে C# এবং .NET ইকোসিস্টেমের **২৭টি মূল পিলার (Pillars)** এবং তাদের অভ্যন্তরীণ সাব-টপিকগুলো অন্তর্ভুক্ত করা হয়েছে। একজন শিক্ষানবিস থেকে শুরু করে **১০+ বছর অভিজ্ঞতাসম্পন্ন টেক লিড, সলিউশন বা প্রিন্সিপাল সফটওয়্যার আর্কিটেক্ট (Principal Architect)** লেভেলের যে কোনো ইন্টারভিউ, প্রোডাকশন ট্রাবলশুটিং এবং সিস্টেম ডিজাইনের জন্য এটি চূড়ান্ত গাইডলাইন।

---

## 📌 সূচিপত্র (Table of Contents)

### ১. Foundation & Language
1. [C# Language Fundamentals & Type System](#1-c-language-fundamentals--type-system)
2. [Object-Oriented Programming (OOP)](#2-object-oriented-programming-oop)
3. [Generics & Variance](#3-generics--variance)
4. [Delegates, Events & Functional C#](#4-delegates-events--functional-c)
5. [LINQ & Expression Trees](#5-linq--expression-trees)

### ২. Data Structures & Collections
6. [Collections Framework & Data Structures](#6-collections-framework--data-structures)
7. [Concurrent & High-Performance Collections](#7-concurrent--high-performance-collections)

### ৩. Runtime, Memory & Performance (Deep Internals)
8. [Memory Management & Garbage Collection (GC)](#8-memory-management--garbage-collection-gc)
9. [Low-Level Memory & Zero-Allocation C#](#9-low-level-memory--zero-allocation-c)
10. [.NET Runtime Internals & Compilation Engine](#10-net-runtime-internals--compilation-engine)
11. [Benchmarking & Micro-Optimization](#11-benchmarking--micro-optimization)

### ৪. Asynchrony, Concurrency & Threading
12. [Asynchronous Programming (TAP)](#12-asynchronous-programming-tap)
13. [Multithreading & Concurrency Control](#13-multithreading--concurrency-control)

### ৫. Metaprogramming & Low-Level Interop
14. [Reflection, Metaprogramming & Attributes](#14-reflection-metaprogramming--attributes)
15. [Roslyn Source Generators & IL Weaving](#15-roslyn-source-generators--il-weaving)
16. [Interoperability & Native P/Invoke](#16-interoperability--native-pinvoke)

### ৬. I/O, Networking & Communication
17. [File I/O, Streams & Serialization](#17-file-io-streams--serialization)
18. [Networking, Web Protocols & Real-Time Comms](#18-networking-web-protocols--real-time-comms)

### ৭. Architecture, Design & Cloud-Native (10+ Years Focus)
19. [SOLID Principles & Clean/Hexagonal Architecture](#19-solid-principles--cleanhexagonal-architecture)
20. [Software Design Patterns & Cloud Resiliency Patterns](#20-software-design-patterns--cloud-resiliency-patterns)
21. [Dependency Injection (DI) & Inversion of Control (IoC)](#21-dependency-injection-di--inversion-of-control-ioc)
22. [Configuration, Options Pattern & Feature Flags](#22-configuration-options-pattern--feature-flags)

### ৮. Reliability, Security & Storage
23. [Data Persistence & ORM Internals](#23-data-persistence--orm-internals)
24. [Caching Strategies & Distributed Caching](#24-caching-strategies--distributed-caching)
25. [Security, Cryptography & Identity](#25-security-cryptography--identity)

### ৯. Observability, Diagnostics & Production Debugging
26. [Diagnostics, Logging & OpenTelemetry](#26-diagnostics-logging--opentelemetry)
27. [Production Troubleshooting & Dump Analysis](#27-production-troubleshooting--dump-analysis)

---

## 🏛️ ১. Foundation & Language

### 1. C# Language Fundamentals & Type System
* CTS (Common Type System) এবং CLS (Common Language Specification)
* Value Types (Stack) vs Reference Types (Heap)
* Primitive Types, Custom Structs, Classes, Interfaces
* Nullable Value Types (`Nullable<T>`, `int?`)
* Nullable Reference Types (NRT - C# 8+), Null-Forgiving (`!`), Null-Coalescing (`??`, `??=`)
* Type Casting (Implicit, Explicit, Custom `implicit`/`explicit` operators)
* Boxing and Unboxing (Performance penalty and mitigation)
* Constants (`const` - compile time) vs Read-only (`readonly` - runtime)
* String Interning Pool, String immutability, `StringBuilder` vs `string.Create()`

### 2. Object-Oriented Programming (OOP)
* Classes, Structs, Records (`record class` vs `record struct`)
* The 4 Pillars: Encapsulation, Abstraction, Inheritance, Polymorphism
* Access Modifiers: `public`, `private`, `protected`, `internal`, `protected internal`, `private protected`, `file`
* Virtual Method Table (VMT/VTable), Method Overriding vs Hiding (`new`)
* Abstract Classes vs Interfaces (Default Interface Methods - DIM)
* Static Abstract Members in Interfaces (Generic Math - C# 11+)
* Primary Constructors (C# 12+), `init`-only properties, `required` properties

### 3. Generics & Variance
* Generic Classes, Structs, Interfaces, Delegates, and Methods
* Generic Constraints: `where T : class`, `struct`, `new()`, `notnull`, `unmanaged`, `BaseClass`, `IInterface`
* Covariance (`out T`) and Contravariance (`in T`) in Interfaces & Delegates
* Generic Specialization at JIT Time (Reference types share code, Value types get distinct JIT instantiation)

### 4. Delegates, Events & Functional C#
* Delegates as Type-Safe Function Pointers (Single-cast vs Multicast)
* Built-in Delegates: `Action<T>`, `Func<T, TResult>`, `Predicate<T>`
* Anonymous Methods, Lambda Expressions, Closures (Variable capture mechanism and allocations)
* Events, Publisher-Subscriber Pattern, Weak Event Pattern
* Functional C#: Pure Functions, Pattern Matching, Switch Expressions, Tuples and Deconstructors

### 5. LINQ & Expression Trees
* LINQ to Objects, LINQ to Entities (EF Core)
* Method Syntax vs Query Syntax
* Deferred Execution (Lazy evaluation) vs Immediate Execution (`ToList()`, `ToArray()`)
* `IEnumerable<T>` vs `IQueryable<T>` (In-memory execution vs SQL translation)
* Expression Trees (`Expression<Func<T, bool>>`), AST generation, Dynamic LINQ

---

## 📦 ২. Data Structures & Collections

### 6. Collections Framework & Data Structures
* Array Types: Single-dimensional, Multi-dimensional (`[,]`), Jagged arrays (`[][]`)
* Standard Collections: `List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`, `Queue<T>`, `Stack<T>`, `LinkedList<T>`
* Modern Collections: `PriorityQueue<TElement, TPriority>` (.NET 6+)
* Collection Interfaces Hierarchy: `IEnumerable<T>` ➡️ `ICollection<T>` ➡️ `IList<T>` / `IDictionary<TKey, TValue>`
* `Dictionary<TKey, TValue>` Internals: Hash functions, Bucket allocation, Collision resolution (Chaining)
* Equality Contract: `GetHashCode()` and `Equals()` rules, `IEqualityComparer<T>`
* Big-O Time & Space Complexity of all .NET collections

### 7. Concurrent & High-Performance Collections
* Thread-Safe Collections (`System.Collections.Concurrent`):
  * `ConcurrentDictionary<TKey, TValue>` (Fine-grained bucket locking)
  * `ConcurrentQueue<T>` (Lock-free circular buffer)
  * `ConcurrentStack<T>` (Interlocked push/pop)
  * `ConcurrentBag<T>` (Thread-local storage + work-stealing algorithm)
  * `BlockingCollection<T>` (Producer-Consumer pattern)
* Channels (`System.Threading.Channels`): High-throughput in-memory messaging
* Immutable Collections (`System.Collections.Immutable`): `ImmutableArray<T>`, `ImmutableList<T>`, `ImmutableDictionary`
* Frozen Collections (.NET 8+): `FrozenDictionary<TKey, TValue>`, `FrozenSet<T>` for ultra-fast, zero-allocation read scenarios

---

## ⚡ ৩. Runtime, Memory & Performance (Deep Internals)

### 8. Memory Management & Garbage Collection (GC)
* Virtual Memory, Process Working Set, Commit vs Reserve
* Stack Allocation (Method execution frame) vs Managed Heap
* Object Memory Layout in CLR: SyncBlock Index (4/8 bytes) + MethodTable Pointer (4/8 bytes) + Data Payload
* GC Generations: Gen 0 (Ephemeral), Gen 1, Gen 2 (Long-lived)
* SOH (Small Object Heap), LOH (Large Object Heap - 85,000+ bytes), POH (Pinned Object Heap)
* GC Phases: Mark ➡️ Plan ➡️ Sweep ➡️ Compact
* Workstation GC vs Server GC (Dedicated heap & thread per CPU core)
* Background GC vs Non-concurrent GC, GC Pauses (STW - Stop-The-World)
* Dispose Pattern: `IDisposable`, `IAsyncDisposable`, `GC.SuppressFinalize`, SafeHandle

### 9. Low-Level Memory & Zero-Allocation C#
* `Span<T>` and `ReadOnlySpan<T>` (Contiguous memory representation on stack)
* `Memory<T>` and `ReadOnlyMemory<T>` (Heap-safe span abstractions)
* `ref struct` (Stack-only guarantee, no boxing, no interfaces)
* `stackalloc` keyword for fast stack allocations
* Buffer Pooling: `ArrayPool<T>.Shared`, `MemoryPool<T>`
* `MemoryMarshal` and `CollectionsMarshal` for zero-overhead direct buffer access
* Unsafe Code: Pointers (`*`, `&`, `->`), `fixed` statement, pinning

### 10. .NET Runtime Internals & Compilation Engine
* Compilation Architecture: C# Source ➡️ Roslyn Compiler (`csc`) ➡️ CIL (Common Intermediate Language) + Metadata ➡️ PE File
* Execution Engine: CoreCLR initialization, `hostfxr`, `AssemblyLoadContext`
* RyuJIT Compiler:
  * Tiered Compilation: Tier 0 (Quick JIT) vs Tier 1 (Optimized JIT)
  * Dynamic PGO (Profile-Guided Optimization - .NET 8/9)
  * Devirtualization, Loop Unrolling, Inlining decisions
* Native AOT (Ahead-of-Time compilation): Zero-JIT instant startup, trim-friendly code

### 11. Benchmarking & Micro-Optimization
* Micro-benchmarking with **BenchmarkDotNet** (Memory diagnosis, Disassembly output)
* Hardware Intrinsics / SIMD (`Vector<T>`, AVX2, AVX-512, ARM Neon)
* False Sharing (CPU cache line contention) and `[StructLayout]` padding
* Branch Prediction optimization, Cold vs Hot path separation

---

## 🔄 ৪. Asynchrony, Concurrency & Threading

### 12. Asynchronous Programming (TAP)
* Task-based Asynchronous Pattern (TAP)
* `Task` vs `Task<T>` vs `ValueTask<T>` (Allocation-free async paths)
* `async/await` Internals: Roslyn-generated State Machine struct
* SynchronizationContext and TaskScheduler
* `ConfigureAwait(false)` in libraries vs UI / Web context
* Async Void trap, Unobserved Task Exceptions, Deadlock conditions
* `CancellationToken` and Cooperative Cancellation

### 13. Multithreading & Concurrency Control
* OS Threads vs Managed Threads vs ThreadPool
* ThreadPool Architecture: Global Queue vs Local Queue, Work-Stealing, Hill-Climbing Algorithm
* Synchronization Primitives:
  * User-mode (Lock-free): `Interlocked` operations, `volatile`
  * Kernel-mode: `Mutex`, `AutoResetEvent`, `ManualResetEvent`
  * Hybrid / Lightweight: `lock` (`Monitor`), `ReaderWriterLockSlim`, `SemaphoreSlim`, `SpinLock`
* Concurrency Hazards: Race Conditions, Deadlocks, Livelocks, Thread Starvation

---

## 🔍 ৫. Metaprogramming & Low-Level Interop

### 14. Reflection, Metaprogramming & Attributes
* `System.Type`, `typeof()` vs `GetType()`
* Member Inspection: `FieldInfo`, `PropertyInfo`, `MethodInfo`, `BindingFlags`
* Dynamic Invocation: `MethodInfo.Invoke()`, `Activator.CreateInstance()`
* Custom Attributes (`AttributeUsageAttribute`) and runtime extraction
* Expression Trees vs Reflection performance

### 15. Roslyn Source Generators & IL Weaving
* Roslyn Compiler API: Syntax Trees, Semantic Models, Symbols
* Incremental Source Generators (`IIncrementalGenerator`)
* Replacing Reflection with Source Generators for Native AOT readiness
* IL Weaving (Fody) and IL Manipulation

### 16. Interoperability & Native P/Invoke
* Platform Invoke (`DllImport`, `[LibraryImport]` in modern .NET)
* Calling C/C++ native libraries, Native memory pointers (`nint`, `nuint`)
* Data Marshaling, Struct layouts (`LayoutKind.Sequential`, `LayoutKind.Explicit`)
* COM Interop, Native AOT exports (`[UnmanagedCallersOnly]`)

---

## 🌐 ৬. I/O, Networking & Communication

### 17. File I/O, Streams & Serialization
* Streams Architecture: `Stream`, `FileStream`, `MemoryStream`, `BufferedStream`
* High-Performance I/O with `System.IO.Pipelines` (Pipe, PipeReader, PipeWriter)
* Modern Serialization: `System.Text.Json` (Source generators, Utf8JsonReader/Writer)
* Binary Serialization, Protobuf serialization

### 18. Networking, Web Protocols & Real-Time Comms
* Low-level: `Socket`, TCP/UDP networking
* `HttpClient` and `IHttpClientFactory` (Socket exhaustion prevention, DNS refreshing)
* Web Protocols: HTTP/1.1, HTTP/2, HTTP/3 (QUIC)
* High-Performance RPC: **gRPC** (HTTP/2 multiplexing, Protobuf contracts)
* Real-time Communication: **SignalR** (WebSockets, Server-Sent Events, Long Polling)

---

## 🏛️ ৭. Architecture, Design & Cloud-Native (10+ Years Focus)

### 19. SOLID Principles & Clean/Hexagonal Architecture
* SOLID Principles deeply applied: SRP, OCP, LSP, ISP, DIP
* Domain-Driven Design (DDD):
  * Entities, Value Objects, Aggregates, Aggregate Roots
  * Domain Events, Repositories, Domain Services
* Architectural Styles: Clean Architecture, Onion Architecture, Hexagonal (Ports & Adapters)
* CQRS (Command Query Responsibility Segregation) & Event Sourcing

### 20. Software Design Patterns & Cloud Resiliency Patterns
* GoF Patterns in Enterprise .NET (Factory, Strategy, Decorator, Adapter, Observer, State)
* Enterprise Patterns: Unit of Work, Repository Pattern, Specification Pattern
* Cloud Resiliency Patterns:
  * Retry with Exponential Backoff and Jitter
  * Circuit Breaker Pattern (Polly)
  * Outbox Pattern (Guaranteed messaging)
  * Saga Pattern (Orchestration vs Choreography for distributed transactions)

### 21. Dependency Injection (DI) & Inversion of Control (IoC)
* Microsoft.Extensions.DependencyInjection container internals
* Service Lifetimes: `Transient`, `Scoped`, `Singleton`
* DI Anti-Patterns: Captive Dependencies, Service Locator Anti-Pattern
* Keyed Services (.NET 8+), Custom DI Containers (Autofac)

### 22. Configuration, Options Pattern & Feature Flags
* Configuration Providers: JSON, Environment Variables, Command Line, Azure App Configuration
* Options Pattern: `IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`
* Secret Management: Secret Manager, Azure Key Vault, HashiCorp Vault
* Feature Flags & Dynamic Toggles (Microsoft.FeatureManagement)

---

## 🔒 ৮. Reliability, Security & Storage

### 23. Data Persistence & ORM Internals
* **Entity Framework Core Internals:**
  * DbContext Lifecycle, Change Tracker, Snapshotting
  * Query Execution Pipeline: LINQ ➡️ Expression Tree ➡️ SQL Generation ➡️ Materialization
  * Performance Optimizations: `AsNoTracking()`, Split Queries, Compiled Queries
  * N+1 Query Problem, Cartesian Explosion
* Micro-ORMs: **Dapper** (High-speed mapping, Raw SQL execution)
* Concurrency Conflicts: Optimistic vs Pessimistic Concurrency

### 24. Caching Strategies & Distributed Caching
* In-Memory Caching: `IMemoryCache`, Expiration policies (Absolute vs Sliding), Size limits
* Distributed Caching: **Redis** (`IDistributedCache`, StackExchange.Redis)
* Output Caching & Response Caching in ASP.NET Core
* Caching Patterns: Cache-Aside, Write-Through, Write-Behind
* Caching Hazards: Cache Stampede (Thundering Herd), Cache Penetration, Cache Avalanche

### 25. Security, Cryptography & Identity
* Authentication & Authorization: Cookie, JWT Bearer, OAuth 2.0, OpenID Connect (OIDC)
* ASP.NET Core Identity, Claims-based & Policy-based Authorization
* Cryptography: `System.Security.Cryptography`, AES Encryption, RSA, Hashing (SHA256, HMAC, BCrypt/Argon2)
* Protection against OWASP Top 10: XSS, CSRF, SQL Injection, SSRF
* ASP.NET Core Data Protection API (Key ring management)

---

## 📊 ৯. Observability, Diagnostics & Production Debugging

### 26. Diagnostics, Logging & OpenTelemetry
* Structured Logging: `ILogger`, Serilog, Structured Message Templates
* OpenTelemetry in .NET:
  * Traces: `ActivitySource`, `Activity`, Distributed Tracing Context Propagation
  * Metrics: `Meter`, `Counter`, `Histogram`, Prometheus integration
* Health Checks: `IHealthCheck`, Liveness and Readiness probes for Kubernetes

### 27. Production Troubleshooting & Dump Analysis (Principal Level)
* Diagnostic CLI Tools:
  * `dotnet-dump` (Capturing and analyzing memory dumps on Linux/Windows)
  * `dotnet-trace` (CPU profiling via Speedscope/PerfView)
  * `dotnet-gcdump` (Lightweight memory heap inspection)
  * `dotnet-counters` (Real-time GC, ThreadPool, and ASP.NET Core metrics)
* Deep Debugging: WinDbg, SOS (Son of Strike) debugging extension
* Real-World Troubleshooting:
  * Diagnosing High CPU usage (Spinning threads, infinite loops, regex catastrophic backtracking)
  * Diagnosing OutOfMemoryException (OOM) and Memory Leaks (Event handlers, Static roots, LOH fragmentation)
  * Diagnosing ThreadPool Starvation and Deadlocks (Sync-over-async)
