# .NET (ASP.NET Core Web API) Master Roadmap

এই রোডম্যাপটি .NET (বিশেষ করে Web API) শেখা এবং ইন্টারভিউ প্রস্তুতির জন্য ধাপে ধাপে সাজানো হয়েছে। আমরা ধরে নিচ্ছি আপনি C# এর বেসিক জানেন।

## ১. Introduction & Setup (বেসিক ও আর্কিটেকচার)
*   **Project Structure:** একটি নতুন Web API প্রজেক্টের ফোল্ডার স্ট্রাকচার।
*   **Program.cs:** Kestrel Server, WebApplicationBuilder, এবং অ্যাপ্লিকেশন কীভাবে স্টার্ট হয়।
*   **Configuration:** `appsettings.json`, `appsettings.Development.json` থেকে ডেটা রিড করা, Environment Variables, User Secrets।

## ২. Controllers & Routing (রাউটিং ও কন্ট্রোলার)
*   **Controllers:** `ControllerBase` vs `Controller`, API Controller অ্যাট্রিবিউট (`[ApiController]`)।
*   **Routing:** Attribute Routing (`[Route("api/[controller]")]`), HTTP Methods (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`, `[HttpPatch]`)।
*   **Action Results:** `IActionResult`, `ActionResult<T>`, `Ok()`, `NotFound()`, `BadRequest()`, `CreatedAtAction()`।

## ৩. Dependency Injection (DI) (ডিপেন্ডেন্সি ইনজেকশন)
*   **IoC Container:** Inversion of Control কনসেপ্ট এবং .NET এর বিল্ট-ইন DI কন্টেইনার।
*   **Service Lifetimes:** `AddTransient()`, `AddScoped()`, `AddSingleton()` (কখন কোনটা ব্যবহার করবেন - ইন্টারভিউয়ের সবচেয়ে ইম্পর্ট্যান্ট প্রশ্ন)।
*   **Injection Methods:** Constructor Injection, Method Injection (`[FromServices]`)।

## ৪. Middleware Pipeline (মিডলওয়্যার)
*   **Concept:** Request এবং Response এর মাঝে মিডলওয়্যার কীভাবে কাজ করে (Pipeline/Chain)।
*   **Built-in Middlewares:** Routing, Authentication, Authorization, Static Files, CORS।
*   **Custom Middleware:** নিজের কাস্টম মিডলওয়্যার তৈরি করা (`Use()`, `Run()`, `Map()`)।

## ৫. Model Binding & Validation (ডেটা রিসিভ ও ভ্যালিডেশন)
*   **Model Binding:** ক্লায়েন্ট থেকে ডেটা কীভাবে API-তে আসে (`[FromBody]`, `[FromQuery]`, `[FromRoute]`, `[FromHeader]`)।
*   **Validation:** Data Annotations (`[Required]`, `[MaxLength]`), `ModelState.IsValid` চেক করা।
*   **Advanced Validation:** FluentValidation লাইব্রেরি ব্যবহার করে প্রফেশনাল ভ্যালিডেশন।

## ৬. Data Access & Entity Framework Core (EF Core)
*   **Setup:** EF Core ইন্সটল করা, Database Provider (SQL Server, PostgreSQL) কনফিগার করা।
*   **DbContext & DbSet:** ডেটাবেস কন্টেক্সট তৈরি করা এবং টেবিল ম্যাপ করা।
*   **Migrations:** Code-First অ্যাপ্রোচ, `Add-Migration`, `Update-Database` কমান্ড।
*   **Querying:** LINQ to Entities, Eager Loading (`Include`), Lazy Loading, Tracking vs No-Tracking (`AsNoTracking`)।

## ৭. Exception Handling & Logging (ত্রুটি ও লগ ম্যানেজমেন্ট)
*   **Exception Handling:** Global Exception Handler তৈরি করা (Custom Exception Middleware)।
*   **Logging:** .NET এর বিল্ট-ইন `ILogger`, লগ লেভেল (Info, Warning, Error), থার্ড-পার্টি লগার (Serilog বা NLog) ইন্টিগ্রেশন।

## ৮. Authentication & Authorization (সিকিউরিটি)
*   **Authentication:** ইউজার কে তা যাচাই করা। JWT (JSON Web Tokens) এর বেসিক এবং ইমপ্লিমেন্টেশন, Bearer Token।
*   **Authorization:** ইউজারের পারমিশন আছে কি না তা চেক করা। Role-based (`[Authorize(Roles = "Admin")]`), Claims-based, এবং Policy-based Authorization।

## ৯. Performance & Optimization (পারফরম্যান্স বৃদ্ধি)
*   **Caching:** In-Memory Caching, Distributed Caching (Redis)।
*   **Asynchronous API:** কন্ট্রোলারে Async/Await এর সঠিক ব্যবহার।
*   **Data Shaping:** Pagination (পেজিনেশন), Filtering, Sorting।

## ১০. Architecture & Design Patterns (সফটওয়্যার ডিজাইন)
*   **Clean Architecture:** Onion Architecture, লেয়ারিং (Domain, Application, Infrastructure, Web API)।
*   **Design Patterns:** 
    *   Repository Pattern (Generic vs Specific)।
    *   Unit of Work Pattern।
    *   CQRS (Command Query Responsibility Segregation) প্যাটার্ন এবং MediatR লাইব্রেরির ব্যবহার।

## ১১. Testing (টেস্টিং)
*   **Unit Testing:** xUnit বা NUnit ফ্রেমওয়ার্ক দিয়ে কোড টেস্ট করা।
*   **Mocking:** Moq ফ্রেমওয়ার্ক ব্যবহার করে ডিপেন্ডেন্সি মক করা।
*   **Integration Testing:** TestServer ব্যবহার করে API এর এন্ডপয়েন্ট টেস্ট করা।

## ১২. Deployment, API Documentation & DevOps
*   **Documentation:** Swagger/OpenAPI কনফিগারেশন।
*   **Containerization:** Dockerfile তৈরি করে .NET অ্যাপ ডকারাইজ করা।
*   **Deployment:** IIS, Linux সার্ভার বা ক্লাউডে (Azure/AWS) পাবলিশ করা।
