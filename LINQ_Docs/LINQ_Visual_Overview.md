# LINQ — সম্পূর্ণ Visual Overview (বাংলায়)

> LINQ-এর সব Keyword, Method, Element এবং Lifecycle — এক নজরে

---

## ১. LINQ কী নিয়ে তৈরি? (Building Blocks)

```
LINQ
├── Keywords        (Query Syntax-এ ব্যবহার)
├── Methods         (Method Syntax-এ Extension Method)
├── Interfaces      (IEnumerable, IQueryable)
├── Delegates       (Func, Action, Predicate)
├── Expression Tree (IQueryable-এ SQL তৈরি করে)
└── Providers       (Objects, SQL, EF Core, XML, PLINQ)
```

---

## ২. সব LINQ Keywords (Query Syntax)

```
Query Syntax Keywords:
├── from        — ডেটা সোর্স ও range variable নির্ধারণ
├── select      — projection (ফলাফলের shape নির্ধারণ)
├── where       — filtering (শর্ত)
├── orderby     — ascending sort
│   └── descending — descending sort modifier
├── group ... by    — grouping (গ্রুপ করা)
├── join        — inner join
│   └── into   — group join (left outer join)
├── on          — join condition-এর প্রথম অংশ
├── equals      — join condition-এর দ্বিতীয় অংশ
├── let         — intermediate variable
└── in          — range variable-এর সোর্স নির্ধারণ
```

**উদাহরণ:**
```csharp
var result = from s in students          // from + in
             let full = s.First + " " + s.Last // let
             where s.Age > 18             // where
             orderby s.Name descending    // orderby + descending
             join d in depts on s.DeptId equals d.Id // join + on + equals
             select new { Name = full, Dept = d.Name }; // select
```

---

## ৩. সব LINQ Methods — ক্যাটাগরি অনুযায়ী

### ৩.১ Filtering Methods
```
Where(predicate)          — condition অনুযায়ী filter
OfType<T>()               — নির্দিষ্ট type-এর element
```

### ৩.২ Projection Methods
```
Select(selector)          — প্রতিটি element transform
SelectMany(selector)      — nested collection flatten করা
Index()                   — element + index pair (.NET 9+)
```

### ৩.৩ Sorting Methods
```
OrderBy(keySelector)      — ascending sort
OrderByDescending(key)    — descending sort
ThenBy(keySelector)       — secondary ascending sort
ThenByDescending(key)     — secondary descending sort
Reverse()                 — উল্টো ক্রম
```

### ৩.৪ Grouping Methods
```
GroupBy(keySelector)      — key অনুযায়ী গ্রুপ
```

### ৩.৫ Join Methods
```
Join(inner, outerKey, innerKey, result)    — inner join
GroupJoin(inner, outerKey, innerKey, res)  — left outer join
Zip(second, resultSelector)               — pair করে merge
```

### ৩.৬ Set Operation Methods
```
Distinct()                — ডুপ্লিকেট বাদ দেয়
DistinctBy(keySelector)   — key দিয়ে unique (.NET 6+)
Union(second)             — দুটির unique মিলন
UnionBy(second, key)      — key দিয়ে union (.NET 6+)
Intersect(second)         — দুটিতেই আছে এমন (∩)
IntersectBy(second, key)  — key দিয়ে intersect (.NET 6+)
Except(second)            — প্রথমে আছে, দ্বিতীয়তে নেই (A-B)
ExceptBy(second, key)     — key দিয়ে except (.NET 6+)
```

### ৩.৭ Aggregation Methods
```
Count()                   — মোট সংখ্যা
Count(predicate)          — শর্ত পূরণ করে এমন সংখ্যা
LongCount()               — বড় collection-এর জন্য
Sum(selector)             — যোগফল
Average(selector)         — গড়
Min()                     — সর্বনিম্ন
Min(selector)             — নির্বাচিত property-র সর্বনিম্ন
Max()                     — সর্বোচ্চ
Max(selector)             — নির্বাচিত property-র সর্বোচ্চ
MinBy(keySelector)        — minimum key-এর পুরো object (.NET 6+)
MaxBy(keySelector)        — maximum key-এর পুরো object (.NET 6+)
Aggregate(func)           — custom accumulator
Aggregate(seed, func)     — seed সহ custom aggregation
Aggregate(seed,func,res)  — seed + result selector
CountBy(keySelector)      — key অনুযায়ী count (.NET 9+)
AggregateBy(key, seed, func) — key অনুযায়ী aggregation (.NET 9+)
```

### ৩.৮ Element Methods
```
First()                   — প্রথম element (না থাকলে Exception)
First(predicate)          — শর্ত পূরণ করে প্রথম element
FirstOrDefault()          — প্রথম element (না থাকলে default)
FirstOrDefault(pred, def) — কাস্টম default (.NET 6+)
Last()                    — শেষ element (না থাকলে Exception)
LastOrDefault()           — শেষ element (না থাকলে default)
Single()                  — ঠিক একটি element (নয়তো Exception)
SingleOrDefault()         — শূন্য বা একটি element
ElementAt(index)          — নির্দিষ্ট index-এর element
ElementAtOrDefault(index) — safe index access
```

### ৩.৯ Quantifier Methods
```
Any()                     — কোনো element আছে?
Any(predicate)            — শর্ত পূরণ করে এমন কেউ আছে?
All(predicate)            — সবাই শর্ত পূরণ করে?
Contains(value)           — নির্দিষ্ট মান আছে?
Contains(value, comparer) — custom comparer দিয়ে
```

### ৩.১০ Partitioning Methods
```
Take(count)               — প্রথম N element
Take(range)               — Range দিয়ে (.NET 6+)
TakeWhile(predicate)      — শর্ত true থাকা পর্যন্ত
Skip(count)               — প্রথম N element বাদ দাও
SkipWhile(predicate)      — শর্ত true থাকা পর্যন্ত skip
TakeLast(count)           — শেষের N element
SkipLast(count)           — শেষের N element বাদ দাও
Chunk(size)               — নির্দিষ্ট আকারের batch (.NET 6+)
```

### ৩.১১ Concatenation Methods
```
Concat(second)            — দুটি sequence জোড়া দাও (ডুপ্লিকেট রাখে)
Append(element)           — শেষে একটি element যোগ
Prepend(element)          — শুরুতে একটি element যোগ
```

### ৩.১২ Conversion Methods
```
ToList()                  — List<T>-এ রূপান্তর (Immediate)
ToArray()                 — T[]-এ রূপান্তর (Immediate)
ToHashSet()               — HashSet<T>-এ রূপান্তর
ToDictionary(key)         — Dictionary<K,V>-এ রূপান্তর
ToLookup(key)             — Lookup<K,V>-এ রূপান্তর
AsEnumerable()            — IEnumerable<T>-তে cast
AsQueryable()             — IQueryable<T>-তে cast
Cast<T>()                 — নির্দিষ্ট type-এ cast
```

### ৩.১৩ Generation Methods
```
Enumerable.Empty<T>()     — খালি sequence
Enumerable.Range(start,n) — সংখ্যার sequence তৈরি
Enumerable.Repeat(val,n)  — একই মান N বার
DefaultIfEmpty()          — খালি হলে default element
DefaultIfEmpty(default)   — কাস্টম default
```

### ৩.১৪ Equality Methods
```
SequenceEqual(second)         — দুটি sequence সমান?
SequenceEqual(second,comparer)— custom comparer দিয়ে
```

---

## ৪. LINQ-এর মূল Interfaces ও Types

```
Interfaces:
├── IEnumerable<T>     — in-memory iteration (LINQ to Objects)
├── IQueryable<T>      — expression tree (LINQ to DB)
├── IGrouping<K,V>     — GroupBy-এর ফলাফল
├── ILookup<K,V>       — ToLookup()-এর ফলাফল
└── IOrderedEnumerable — OrderBy-এর ফলাফল

Delegates:
├── Func<T, TResult>       — projection/selector
├── Func<T, bool>          — predicate/filter
├── Func<T1,T2,TResult>    — join/zip result selector
└── Action<T>              — forEach-এ ব্যবহার
```

---

## ৫. LINQ-এর Providers (কোথায় চলে)

```
LINQ Providers:
├── LINQ to Objects   — List, Array, IEnumerable (in-memory C#)
├── LINQ to Entities  — EF Core → SQL (যেকোনো DB)
├── LINQ to XML       — XDocument, XElement
├── LINQ to DataSet   — DataSet, DataTable (ADO.NET)
├── PLINQ             — AsParallel() → multi-core CPU
└── Custom Provider   — MongoDB, Cosmos DB, etc.
```

---

## ৬. LINQ Lifecycle — ৬টি ধাপ

```
Step 1: DATA SOURCE          (কোথা থেকে data আসবে?)
        ↓
Step 2: QUERY CONSTRUCTION   (query কীভাবে তৈরি হয়?)
        ↓
Step 3: QUERY EXPRESSION     (Expression Tree বা Delegate)
        ↓
Step 4: EXECUTION            (কখন execute হয়?)
        ↓
Step 5: RESULT PROCESSING    (ফলাফল কীভাবে পাওয়া যায়?)
        ↓
Step 6: ITERATION/CONSUME    (ফলাফল কীভাবে ব্যবহার হয়?)
```

---

## ৬.১ Step 1 — DATA SOURCE

LINQ কাজ করে যেকোনো `IEnumerable<T>` বা `IQueryable<T>` সোর্সে।

```
Data Sources:
├── in-memory   → List<T>, Array, Dictionary, HashSet
├── Database    → DbSet<T> (EF Core), DataTable
├── File/Stream → File.ReadLines(), XDocument
├── Network     → HttpClient response
└── Custom      → yield return দিয়ে তৈরি generator
```

```csharp
// বিভিন্ন ধরনের data source
IEnumerable<int>    src1 = new List<int> { 1, 2, 3 };
IEnumerable<string> src2 = File.ReadLines("data.txt");
IQueryable<User>    src3 = _ctx.Users;                  // EF Core
IEnumerable<int>    src4 = Enumerable.Range(1, 100);    // generated
```

---

## ৬.২ Step 2 — QUERY CONSTRUCTION

দুটি উপায়ে query তৈরি করা যায়:

```
Query Syntax    — SQL-এর মতো, declarative
Method Syntax   — Extension method chain, functional
```

```csharp
// Query Syntax
var q1 = from s in students
         where s.Age > 18
         orderby s.Name
         select s.Name;

// Method Syntax (একই ফলাফল)
var q2 = students
    .Where(s => s.Age > 18)
    .OrderBy(s => s.Name)
    .Select(s => s.Name);

// Compose করা — পরে filter যোগ করা
IQueryable<Product> q = _ctx.Products;
if (onlyActive) q = q.Where(p => p.IsActive); // শর্ত অনুযায়ী যোগ
if (maxPrice > 0) q = q.Where(p => p.Price <= maxPrice);
```

---

## ৬.৩ Step 3 — QUERY EXPRESSION (Expression Tree vs Delegate)

```
IEnumerable<T> → Delegate (Func<T, bool>)
                 — C#-এ compile হয়
                 — in-memory-এ চলে

IQueryable<T>  → Expression Tree
                 — data structure হিসেবে রাখা হয়
                 — Provider SQL-এ translate করে
```

```csharp
// Delegate — compile হয়ে C# এ চলে
Func<User, bool> delegate1 = u => u.IsActive;

// Expression Tree — analyze করা যায়, SQL তৈরি হয়
Expression<Func<User, bool>> expr = u => u.IsActive;

// Expression Tree বিশ্লেষণ
var body     = (MemberExpression)expr.Body;
Console.WriteLine(body.Member.Name); // "IsActive"
// EF Core এই tree থেকে: WHERE IsActive = 1 তৈরি করে
```

---

## ৬.৪ Step 4 — EXECUTION (কখন চলে?)

```
Deferred Execution     — query define হলে চলে না, iterate-এ চলে
Immediate Execution    — call করার সাথে সাথে চলে
```

```
Deferred Execution Operators:
├── Where, Select, SelectMany
├── OrderBy, OrderByDescending, ThenBy
├── GroupBy, Join, GroupJoin
├── Take, Skip, TakeWhile, SkipWhile
├── Distinct, Union, Intersect, Except
├── Concat, Append, Prepend, Reverse
└── DefaultIfEmpty, Zip

Immediate Execution Operators:
├── ToList(), ToArray(), ToDictionary(), ToHashSet()
├── Count(), LongCount(), Sum(), Average(), Min(), Max()
├── First(), FirstOrDefault(), Last(), Single()
├── Any(), All(), Contains()
├── ElementAt(), ElementAtOrDefault()
├── Aggregate()
└── SequenceEqual()
```

```csharp
// Deferred — query রাখা হলো, চলেনি
var query = students.Where(s => s.Age > 18);

// Source পরিবর্তন করলে effect পড়বে!
students.Add(new Student { Age = 25 });

// Immediate — এখন চলে, নতুন student-ও আসবে
var list = query.ToList(); // নতুন student সহ
```

---

## ৬.৫ Step 5 — RESULT PROCESSING

```
Transformation:
├── Single object    → First(), Single(), ElementAt()
├── Collection       → ToList(), ToArray()
├── Dictionary       → ToDictionary(), ToLookup()
├── Scalar           → Count(), Sum(), Average()
├── Boolean          → Any(), All(), Contains()
└── Grouped          → GroupBy() → IGrouping<K,V>
```

```csharp
// বিভিন্ন result type
List<User>              list   = query.ToList();
User                    one    = query.First();
Dictionary<int, User>   dict   = query.ToDictionary(u => u.Id);
int                     count  = query.Count();
bool                    any    = query.Any();
IGrouping<string, User> groups = query.GroupBy(u => u.Department).First();
```

---

## ৬.৬ Step 6 — ITERATION/CONSUME

```
ফলাফল ব্যবহারের উপায়:
├── foreach loop
├── LINQ chain-এ পরবর্তী operator
├── UI/API response-এ return
└── Database-এ save
```

```csharp
// foreach
foreach (var user in users.Where(u => u.IsActive))
    Console.WriteLine(user.Name);

// Method chain
var result = users
    .Where(u => u.IsActive)
    .Select(u => u.Name)
    .OrderBy(n => n)
    .ToList(); // ← এখানেই সব execute হয়

// Async iteration
await foreach (var item in _ctx.Products.AsAsyncEnumerable())
    await ProcessAsync(item);
```

---

## ৭. LINQ Execution Flow — সম্পূর্ণ চিত্র

```
Data Source
    │
    ▼
[Define Query] ──── Deferred (চলেনি) ────────────────────┐
    │                                                      │
    │  .Where() .Select() .OrderBy() .GroupBy() ...       │
    │                                                      │
    ▼                                                      │
[Compose Query] ─── আরো operator যোগ করা যায়            │
    │                                                      │
    ▼                                                      │
[Trigger Execution]  ←─────────────────────────────────────┘
    │
    │  ToList() / First() / Count() / Any() / foreach
    │
    ▼
[Provider Execute]
    ├── IEnumerable → C# loop (in-memory)
    └── IQueryable  → SQL generate → Database execute
                                        │
                                        ▼
                                [Result Set]
                                        │
                                        ▼
                              [Map to Objects]
                                        │
                                        ▼
                              [Return / Use]
```

---

## ৮. LINQ-এর ৬টি Provider এবং তাদের কাজ

### ৮.১ LINQ to Objects
```
সোর্স    : List, Array, IEnumerable
Interface: IEnumerable<T>
Execute  : C# loop (client-side)
কখন     : in-memory data, file, generated sequence
```

### ৮.২ LINQ to Entities (EF Core)
```
সোর্স    : DbSet<T>, IQueryable<T>
Interface: IQueryable<T>
Execute  : SQL generate → database-এ
কখন     : SQL Server, PostgreSQL, MySQL, SQLite
```

### ৮.৩ LINQ to XML
```
সোর্স    : XDocument, XElement
Interface: IEnumerable<XElement>
Execute  : in-memory XML parse
কখন     : XML file/API response
```

### ৮.৪ LINQ to DataSet
```
সোর্স    : DataTable, DataSet
Interface: IEnumerable<DataRow>
Execute  : in-memory DataTable loop
কখন     : Legacy ADO.NET code
```

### ৮.৫ Parallel LINQ (PLINQ)
```
সোর্স    : যেকোনো IEnumerable
Interface: ParallelQuery<T>
Execute  : Multi-core parallel loop
কখন     : CPU-bound heavy computation
```

### ৮.৬ Custom Providers (MongoDB, Cosmos)
```
সোর်    : MongoDB Collection, Cosmos Container
Interface: IQueryable<T>
Execute  : MongoDB Query / Cosmos SQL
কখন     : NoSQL database
```

---

## ৯. Method-এর সম্পূর্ণ তালিকা — Quick Reference

| Category | Methods |
|---|---|
| Filter | `Where` `OfType` |
| Project | `Select` `SelectMany` `Index` |
| Sort | `OrderBy` `OrderByDescending` `ThenBy` `ThenByDescending` `Reverse` |
| Group | `GroupBy` |
| Join | `Join` `GroupJoin` `Zip` |
| Set | `Distinct` `DistinctBy` `Union` `UnionBy` `Intersect` `IntersectBy` `Except` `ExceptBy` |
| Aggregate | `Count` `LongCount` `Sum` `Average` `Min` `Max` `MinBy` `MaxBy` `Aggregate` `CountBy` `AggregateBy` |
| Element | `First` `FirstOrDefault` `Last` `LastOrDefault` `Single` `SingleOrDefault` `ElementAt` `ElementAtOrDefault` |
| Quantifier | `Any` `All` `Contains` `SequenceEqual` |
| Partition | `Take` `TakeWhile` `TakeLast` `Skip` `SkipWhile` `SkipLast` `Chunk` |
| Concat | `Concat` `Append` `Prepend` |
| Convert | `ToList` `ToArray` `ToHashSet` `ToDictionary` `ToLookup` `AsEnumerable` `AsQueryable` `Cast` |
| Generate | `Empty` `Range` `Repeat` `DefaultIfEmpty` |

---

## ১০. সবচেয়ে গুরুত্বপূর্ণ বিষয় — Summary

```
LINQ শিখতে হলে ৬টি বিষয় জানতে হবে:

১. SYNTAX        → Query Syntax vs Method Syntax
                   (কোনটি কখন ব্যবহার করবেন)

২. EXECUTION     → Deferred vs Immediate
                   (কখন চলে, কখন চলে না)

৩. INTERFACES    → IEnumerable vs IQueryable
                   (C#-এ চলে নাকি SQL-এ)

৪. OPERATORS     → সব ক্যাটাগরির method
                   (filter, project, sort, group, join, set, aggregate)

৫. EXPRESSION    → Expression Tree
                   (IQueryable-এ কীভাবে SQL তৈরি হয়)

৬. PROVIDERS     → Objects, EF Core, XML, PLINQ
                   (কোন provider কখন ব্যবহার করবেন)
```

---

## ১১. Study Path — ধাপে ধাপে শেখার রোডম্যাপ

```
Week 1 — Foundation
    ├── LINQ কী, কেন দরকার
    ├── Query Syntax vs Method Syntax
    ├── from, where, select, orderby
    └── IEnumerable, Deferred/Immediate Execution

Week 2 — Core Operators
    ├── Filter: Where, OfType
    ├── Project: Select, SelectMany
    ├── Sort: OrderBy, ThenBy, Reverse
    └── Element: First, Last, Single, ElementAt

Week 3 — Grouping & Joining
    ├── GroupBy, IGrouping
    ├── Join (Inner), GroupJoin (Left Outer)
    ├── Zip, Concat, Append
    └── Set: Union, Intersect, Except, Distinct

Week 4 — Aggregation & Conversion
    ├── Count, Sum, Average, Min, Max
    ├── MinBy, MaxBy (NET 6+)
    ├── Aggregate (custom)
    ├── ToList, ToArray, ToDictionary, ToLookup
    └── Any, All, Contains

Week 5 — Advanced
    ├── IQueryable vs IEnumerable (গভীরভাবে)
    ├── Expression Tree
    ├── EF Core + LINQ (N+1, Include, AsNoTracking)
    ├── PLINQ (AsParallel)
    └── Async LINQ (ToListAsync, AsAsyncEnumerable)

Week 6 — Real-world
    ├── Pagination, Dynamic Filtering
    ├── Specification Pattern
    ├── Performance Optimization
    ├── Unit Testing LINQ queries
    └── Custom Extension Methods
```

---

> **মনে রাখুন:**
> - `IEnumerable` → C#-এ চলে → LINQ to Objects
> - `IQueryable` → SQL-এ translate হয় → LINQ to Entities
> - `ToList()` / `First()` / `Count()` → Immediate Execution trigger
> - `Where()` / `Select()` / `OrderBy()` → Deferred Execution
> - `AsNoTracking()` → EF Core-এ read-only query-তে দ্রুত
> - `Any()` → `Count() > 0`-এর চেয়ে সবসময় দ্রুত

---

## ১২. Advanced Topics (Overview-এর বাইরের বিষয়গুলো)

### ১২.১ LINQ to XML — গভীর বিষয়
LINQ ব্যবহার করে XML তৈরি, পড়া এবং পরিবর্তন করা।
```csharp
// XML তৈরি
XElement xml = new XElement("Students",
    new XElement("Student", new XAttribute("Id", 1), new XElement("Name", "Rahim")),
    new XElement("Student", new XAttribute("Id", 2), new XElement("Name", "Karim"))
);

// XML পড়া
var names = xml.Descendants("Student")
               .Where(x => (int)x.Attribute("Id") > 1)
               .Select(x => x.Element("Name").Value);
```

### ১২.২ PLINQ (Parallel LINQ) — বিস্তারিত
CPU-bound কাজের জন্য multi-threading।
```csharp
var numbers = Enumerable.Range(1, 1000000);
var primes = numbers.AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .AsOrdered() // অরিজিনাল ক্রম বজায় রাখে
                    .Where(n => IsPrime(n))
                    .ToList();
```

### ১২.৩ Expression Tree — কোড থেকে SQL
Runtime-এ dynamically query তৈরি করা।
```csharp
// Expression তৈরি
ParameterExpression param = Expression.Parameter(typeof(User), "u");
MemberExpression property = Expression.Property(param, "Age");
ConstantExpression value = Expression.Constant(18);
BinaryExpression greaterThan = Expression.GreaterThan(property, value);

Expression<Func<User, bool>> expr = Expression.Lambda<Func<User, bool>>(greaterThan, param);
// ফলাফল: u => u.Age > 18
```

### ১২.৪ Custom LINQ Provider
নিজের ডেটাবেস বা API-এর জন্য `IQueryable<T>` এবং `IQueryProvider` implement করা। সাধারণত `ExpressionVisitor` ব্যবহার করে Expression Tree-কে SQL বা API request-এ রূপান্তর করা হয়।

### ১২.৫ Async LINQ — বিস্তারিত (IAsyncEnumerable)
Network বা I/O bound কাজের জন্য async streaming।
```csharp
public async IAsyncEnumerable<User> GetUsersAsync()
{
    // API বা Database থেকে ধাপে ধাপে আনা
    await Task.Delay(100);
    yield return new User { Name = "Rahim" };
}

// ব্যবহার
await foreach(var user in GetUsersAsync())
{
    Console.WriteLine(user.Name);
}
```

### ১২.৬ EF Core-specific LINQ
EF Core-এ LINQ-এর কিছু বিশেষ ব্যবহার:
```csharp
// Global Query Filter (যেমন: Soft Delete)
modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

// Raw SQL + LINQ Hybrid
var users = await _ctx.Users
                      .FromSqlRaw("SELECT * FROM Users")
                      .Where(u => u.Age > 18)
                      .ToListAsync();

// JSON Column query (.NET 8+)
var query = _ctx.Orders.Where(o => o.Details.ShippingAddress.City == "Dhaka");
```

### ১২.৭ Custom `IEqualityComparer`
`Distinct`, `Union`, `Intersect`-এ custom object compare করার জন্য।
```csharp
public class UserComparer : IEqualityComparer<User>
{
    public bool Equals(User x, User y) => x.Id == y.Id;
    public int GetHashCode(User obj) => obj.Id.GetHashCode();
}

var uniqueUsers = users.Distinct(new UserComparer());
// .NET 6+ এ সহজে DistinctBy ব্যবহার করা যায়: users.DistinctBy(u => u.Id);
```

### ১২.৮ `ILookup<K,V>`
`Dictionary<K,V>`-এ একটি key-এর একটিই value থাকে, কিন্তু `ILookup<K,V>`-এ একটি key-এর অধীনে একাধিক value থাকতে পারে।
```csharp
var lookup = students.ToLookup(s => s.Department);
// Department "CS"-এর সব স্টুডেন্ট:
var csStudents = lookup["CS"]; 
```

### ১২.৯ Query Continuation (`into` keyword)
Query-র ফলাফলকে একটি নতুন ভেরিয়েবলে রেখে তার ওপর আবার query চালানো।
```csharp
var query = from s in students
            group s by s.Department into deptGroup
            where deptGroup.Count() > 5 // into-র পর নতুন query
            select new { Dept = deptGroup.Key, Count = deptGroup.Count() };
```

### ১২.১০ Reactive Extensions (Rx.NET)
Event stream বা asynchronous data sequence-এর ওপর LINQ। `IEnumerable<T>` (Pull) এর বদলে `IObservable<T>` (Push) ব্যবহার করে।
```csharp
IObservable<Event> clicks = GetClicksObservable();
clicks.Where(c => c.Button == MouseButton.Left)
      .Subscribe(c => Console.WriteLine("Left clicked!"));
```

### ১২.১১ LINQ + `Span<T>` / `Memory<T>`
High-performance memory manipulation। যদিও Span-এ সরাসরি LINQ চলে না, তবে কিছু extension বা custom loop দিয়ে memory-efficient কাজ করা হয়।

---
*সর্বশেষ আপডেট: .NET 9 | C# 13 | 2026*
