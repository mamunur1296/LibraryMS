# LINQ ইন্টারভিউ প্রশ্ন ও উত্তর (বাংলায়)

> C# LINQ সংক্রান্ত ৪০টি গুরুত্বপূর্ণ প্রশ্ন ও বিস্তারিত উত্তর | .NET 9 | C# 13

---

## প্রশ্ন ০১ — LINQ (Language Integrated Query) কী?

**LINQ** হলো C# 3.0 (.NET Framework 3.5)-এ প্রবর্তিত একটি শক্তিশালী ফিচার।  
এটি in-memory collection, database, XML ইত্যাদি ডেটা সোর্স থেকে সরাসরি C# কোডে type-safe এবং declarative উপায়ে ডেটা query করার সুবিধা দেয়।

**মূল বৈশিষ্ট্য:**
- **Language Integrated** — query সরাসরি C# কোডের অংশ, আলাদা string নয়
- **Type-Safe** — compile time-এ error ধরা পড়ে
- **Unified Syntax** — যেকোনো ডেটা সোর্সের জন্য একই সিনট্যাক্স

```csharp
// LINQ ছাড়া (পুরনো পদ্ধতি)
var result = new List<string>();
foreach (var s in students)
    if (s.Age > 18) result.Add(s.Name);

// LINQ দিয়ে (আধুনিক পদ্ধতি)
var result = students.Where(s => s.Age > 18).Select(s => s.Name);
```

---

## প্রশ্ন ০২ — C#-এ LINQ-এর বিভিন্ন ধরন কী কী?

- **LINQ to Objects** — `List<T>`, `Array`, `Dictionary<K,V>` জাতীয় in-memory collection
- **LINQ to SQL** — Microsoft SQL Server (পুরনো, এখন Deprecated)
- **LINQ to Entities (EF Core)** — Entity Framework Core দিয়ে যেকোনো রিলেশনাল DB
- **LINQ to XML (XLINQ)** — XML ডকুমেন্ট query ও manipulation
- **LINQ to DataSet** — ADO.NET `DataSet` ও `DataTable`
- **Parallel LINQ (PLINQ)** — Multi-core CPU ব্যবহার করে parallel execution

---

## প্রশ্ন ০৩ — LINQ to Objects, LINQ to SQL এবং LINQ to Entities-এর পার্থক্য?

**LINQ to Objects**
- ডেটা সোর্স: In-memory collection
- Interface: `IEnumerable<T>`
- Execution: Client-side (C# লুপ চালায়)
- অবস্থা: ✅ সক্রিয়

**LINQ to SQL**
- ডেটা সোর্স: শুধু SQL Server
- Interface: `IQueryable<T>`
- Execution: Server-side (SQL তৈরি করে)
- অবস্থা: ⚠️ Deprecated

**LINQ to Entities (EF Core)**
- ডেটা সোর্স: SQL Server, PostgreSQL, MySQL ইত্যাদি
- Interface: `IQueryable<T>`
- Execution: Server-side (SQL তৈরি করে)
- অবস্থা: ✅ সক্রিয়

```csharp
// LINQ to Objects — C# লুপে চলে
var evens = list.Where(x => x % 2 == 0);

// LINQ to Entities — SQL WHERE clause তৈরি করে
var users = await _context.Users.Where(u => u.IsActive).ToListAsync();
```

---

## প্রশ্ন ০৪ — LINQ Query Syntax কী? উদাহরণসহ ব্যাখ্যা করুন।

**Query Syntax** হলো SQL-এর মতো দেখতে একটি declarative পদ্ধতি।  
এটি `from`, `where`, `select`, `orderby`, `group by`, `join` keyword ব্যবহার করে।

**গঠন:**
```
from <variable> in <source>
[where <condition>]
[orderby <key>]
select <result>
```

**উদাহরণ:**
```csharp
var adultAStudents = from s in students
                     where s.Age >= 18 && s.Grade == "A"
                     orderby s.Name
                     select new { s.Name, s.Age };

// ফলাফল:
// { Name = "Jamal", Age = 20 }
// { Name = "Rahim", Age = 22 }
```

> **বিঃদ্রঃ** Query Syntax সবসময় `select` বা `group` দিয়ে শেষ হতে হয়।

---

## প্রশ্ন ০৫ — Method Syntax ও Query Syntax-এর পার্থক্য কী?

**Query Syntax**
- SQL-এর মতো দেখতে
- জটিল join/group-এ বেশি পাঠযোগ্য
- সব operator নেই (যেমন `Count()`, `Take()`)

**Method Syntax**
- Extension method ও lambda ব্যবহার করে
- বেশি নমনীয়, সব operator এখানে আছে
- Simple query-এ বেশি সংক্ষিপ্ত

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6 };

// Query Syntax
var evens_q = from n in numbers
              where n % 2 == 0
              select n;

// Method Syntax (একই ফলাফল)
var evens_m = numbers.Where(n => n % 2 == 0);

// শুধু Method Syntax-এ সম্ভব
var count = numbers.Count(n => n % 2 == 0); // 3
var max   = numbers.Max();                   // 6
```

> **উভয়ই একই IL (Intermediate Language) কোডে compile হয়।**

---

## প্রশ্ন ০৬ — LINQ-এ `from` keyword-এর উদ্দেশ্য কী?

`from` keyword Query Syntax-এর শুরুর বিন্দু। এটি দুটি কাজ করে:
1. **ডেটা সোর্স** নির্ধারণ করে
2. **Range variable** সংজ্ঞায়িত করে (প্রতিটি element-এর temporary নাম)

```csharp
// from <range_variable> in <data_source>
var result = from student in students
             where student.Age > 18
             select student.Name;

// একাধিক from — nested query (SelectMany-এর মতো)
var result2 = from dept in departments
              from emp in dept.Employees
              where emp.Salary > 50000
              select new { dept.Name, emp.Name };
```

---

## প্রশ্ন ০৭ — Traditional SQL-এর তুলনায় LINQ-এর সুবিধা কী?

- **Type Safety** — compile time-এ error ধরা পড়ে, SQL string-এ runtime-এ পড়ে
- **IntelliSense** — IDE সাহায্য করে, SQL string-এ করে না
- **Refactoring** — সহজ ও নিরাপদ
- **SQL Injection** — স্বয়ংক্রিয়ভাবে প্রতিরোধ হয়
- **Unified API** — সব ডেটা সোর্সে একই সিনট্যাক্স
- **Debug করা** — সহজ, breakpoint দেওয়া যায়
- **Readable** — C# কোডের সাথে একীভূত

---

## প্রশ্ন ০৮ — LINQ-এ `select` keyword-এর ভূমিকা কী?

`select` হলো LINQ-এর **projection** operator। query-এর ফলাফল কেমন হবে তা নির্ধারণ করে।

```csharp
// সম্পূর্ণ object
var all   = from s in students select s;

// শুধু একটি property
var names = from s in students select s.Name;

// Anonymous type — নতুন shape
var info = students.Select(s => new
{
    FullName = s.FirstName + " " + s.LastName,
    IsAdult  = s.Age >= 18
});

// Data transform
var upper = students.Select(s => s.Name.ToUpper());
```

---

## প্রশ্ন ০৯ — LINQ-এ `where` clause কী করে?

`where` হলো LINQ-এর **filtering** operator। boolean condition নিয়ে শুধু সেই element গুলো রিটার্ন করে যেগুলোর জন্য condition `true`।

```csharp
// একটি শর্ত
var expensive = products.Where(p => p.Price > 1000);

// AND শর্ত
var highEnd = products.Where(p => p.Price > 1000 && p.Stock > 0);

// OR শর্ত
var special = products.Where(p =>
    p.Category == "Electronics" ||
    p.Category == "Mobile");

// Chained Where — AND হিসেবে কাজ করে
var filtered = products
    .Where(p => p.IsAvailable)
    .Where(p => p.Price < 1000);
```

> **বিঃদ্রঃ** `where` deferred execution করে — loop না করা পর্যন্ত filter হয় না।

---

## প্রশ্ন ১০ — LINQ-এ Sorting কীভাবে করবেন?

```csharp
// Ascending sort
var byName = employees.OrderBy(e => e.Salary);

// Descending sort
var bySalDesc = employees.OrderByDescending(e => e.Salary);

// Multi-level sort
var multi = employees
    .OrderBy(e => e.Department)        // প্রথমে department ascending
    .ThenByDescending(e => e.Salary)   // তারপর salary descending
    .ThenBy(e => e.Name);              // তারপর name ascending

// Query Syntax
var sorted = from e in employees
             orderby e.Department, e.Salary descending
             select e;
```

---

## প্রশ্ন ১১ — LINQ-এ Deferred Execution কী?

Query সংজ্ঞায়িত করার সময় execute হয় না। প্রথমবার iterate করার সময় (যেমন `foreach` বা `ToList()`) execute হয়।

```csharp
var numbers = new List<int> { 1, 2, 3 };

// Query define হলো — কিন্তু execute হয়নি!
var query = numbers.Where(n => n > 1);

// Define করার পরে list পরিবর্তন
numbers.Add(10);

// এখন execute হবে — নতুন element-ও আসবে!
foreach (var n in query)
    Console.WriteLine(n); // 2, 3, 10

// Immediate Execution-এর operator:
var list   = numbers.Where(n => n > 1).ToList();  // তাৎক্ষণিক
var count  = numbers.Count(n => n > 1);            // তাৎক্ষণিক
var first  = numbers.First(n => n > 1);            // তাৎক্ষণিক
```

---

## প্রশ্ন ১২ — LINQ-এ Lazy Evaluation কী?

**Lazy Evaluation** মানে হলো element by element গণনা — পুরো collection একসাথে নয়।  
প্রয়োজন না হওয়া পর্যন্ত পরবর্তী element process হয় না।

```csharp
IEnumerable<int> GetNumbers()
{
    Console.WriteLine("1 generate হচ্ছে"); yield return 1;
    Console.WriteLine("2 generate হচ্ছে"); yield return 2;
    Console.WriteLine("3 generate হচ্ছে"); yield return 3;
}

var query = GetNumbers().Where(n => n > 1);

// শুধু প্রথম element চাইলে শুধু প্রয়োজনীয়টুকু process
var first = query.First();
// Output: "1 generate হচ্ছে", "2 generate হচ্ছে"
// "3 generate হচ্ছে" আর হবে না!
```

> **সুবিধা:** বড় বা infinite sequence-এ মেমোরি বাঁচায়।

---

## প্রশ্ন ১৩ — LINQ-এ `Take` method কী?

`Take(n)` শুরু থেকে `n` সংখ্যক element নিয়ে রিটার্ন করে।

```csharp
var numbers = new[] { 10, 20, 30, 40, 50, 60 };

var first3    = numbers.Take(3);              // [10, 20, 30]
var page2     = numbers.Skip(3).Take(3);      // [40, 50, 60] — Pagination!
var range     = numbers.Take(2..5);           // [30, 40, 50] — .NET 6+ Range
var takeWhile = numbers.TakeWhile(n => n<40); // [10, 20, 30]

// Pagination formula:
// Page N: Skip((N-1) * pageSize).Take(pageSize)
```

---

## প্রশ্ন ১৪ — LINQ-এ `Skip` method কী করে?

`Skip(n)` শুরু থেকে `n` সংখ্যক element এড়িয়ে বাকিগুলো রিটার্ন করে।

```csharp
var numbers  = new[] { 10, 20, 30, 40, 50, 60 };

var skip3    = numbers.Skip(3);               // [40, 50, 60]
var skipWh   = numbers.SkipWhile(n => n<40);  // [40, 50, 60]

// Pagination:
// Page 1: Skip(0).Take(3)  → [10, 20, 30]
// Page 2: Skip(3).Take(3)  → [40, 50, 60]
```

---

## প্রশ্ন ১৫ — LINQ দিয়ে Filtering কীভাবে করবেন?

```csharp
// Where — condition-based filter
var expensive = products.Where(p => p.Price > 500);

// Complex filter
var result = products.Where(p =>
    p.Price > 500 &&
    p.Category == "Electronics" &&
    p.Stock > 0);

// OfType — type-based filter (mixed collection থেকে)
var mixed      = new List<object> { 1, "Hello", 2.5, "World" };
var strings    = mixed.OfType<string>(); // ["Hello", "World"]
var ints       = mixed.OfType<int>();    // [1]
```

---

## প্রশ্ন ১৬ — LINQ-এ `Distinct` method-এর ব্যবহার কী?

ডুপ্লিকেট মান সরিয়ে unique element গুলো রিটার্ন করে।

```csharp
var numbers = new[] { 1, 2, 2, 3, 3, 4 };
var unique  = numbers.Distinct();         // [1, 2, 3, 4]

// .NET 6+ DistinctBy — property অনুযায়ী
var uniqueByAge = people.DistinctBy(p => p.Age);

// Custom comparer (case-insensitive)
var uniqueCI = cities.Distinct(StringComparer.OrdinalIgnoreCase);
```

---

## প্রশ্ন ১৭ — LINQ দিয়ে দুটি Collection কীভাবে Join করবেন?

```csharp
// Method Syntax — Inner Join
var joined = orders.Join(
    customers,
    o => o.CustomerId,   // orders-এর key
    c => c.Id,           // customers-এর key
    (o, c) => new
    {
        OrderId      = o.Id,
        CustomerName = c.Name,
        Amount       = o.Amount
    }
);

// Query Syntax — Inner Join
var joined2 = from o in orders
              join c in customers on o.CustomerId equals c.Id
              select new { o.Id, c.Name, o.Amount };
```

---

## প্রশ্ন ১৮ — `First()` এবং `FirstOrDefault()`-এর পার্থক্য কী?

**`First()`**
- শর্ত না মিললে বা sequence খালি হলে → `InvalidOperationException`
- নিশ্চিত যখন element আছে তখন ব্যবহার করুন

**`FirstOrDefault()`**
- শর্ত না মিললে বা sequence খালি হলে → `default(T)` রিটার্ন
- অনিশ্চিত অবস্থায় ব্যবহার করুন, null check করুন

```csharp
var first = numbers.First();                          // 1
var fd    = numbers.FirstOrDefault(n => n > 100);     // 0 (int default)
var fd2   = numbers.FirstOrDefault(n => n > 100, -1); // -1 (.NET 6+)

// Reference type-এ null check জরুরি!
Student s = students.FirstOrDefault(x => x.Id == 999);
if (s != null) Console.WriteLine(s.Name);
```

---

## প্রশ্ন ১৯ — `Single()` এবং `SingleOrDefault()`-এর পার্থক্য কী?

**`Single()`** — ঠিক একটি element থাকতে হবে:
- ০টি বা ১-এর বেশি match → Exception

**`SingleOrDefault()`** — শূন্য বা একটি হতে পারে:
- ০টি match → default রিটার্ন (Exception নেই)
- ১-এর বেশি match → Exception

```csharp
var s1 = numbers.Single(n => n == 3);           // 3 ✅
// var s2 = numbers.Single(n => n > 3);         // ❌ 4,5 আছে — Exception!

var sd1 = numbers.SingleOrDefault(n => n == 3); // 3 ✅
var sd2 = numbers.SingleOrDefault(n => n > 10); // 0 ✅ (Exception নেই)

// ব্যবহার: Id দিয়ে unique record
var user = users.SingleOrDefault(u => u.Id == userId);
```

---

## প্রশ্ন ২০ — LINQ-এ `GroupBy` operator-এর উদ্দেশ্য কী?

Key-এর উপর ভিত্তি করে collection-এর element গুলোকে গ্রুপে ভাগ করে।  
প্রতিটি গ্রুপ `IGrouping<TKey, TElement>` যার একটি `Key` property আছে।

```csharp
// বিভাগ অনুযায়ী ছাত্র গ্রুপ
var grouped = students.GroupBy(s => s.Department);

foreach (var group in grouped)
{
    Console.WriteLine($"বিভাগ: {group.Key} — {group.Count()} জন");
    foreach (var s in group)
        Console.WriteLine($"  - {s.Name}");
}

// Summary তৈরি
var summary = students
    .GroupBy(s => s.Grade)
    .Select(g => new
    {
        Grade   = g.Key,
        Count   = g.Count(),
        AvgAge  = g.Average(s => s.Age)
    });
```

---

## প্রশ্ন ২১ — LINQ দিয়ে Element গণনা কীভাবে করবেন?

```csharp
int total    = students.Count();
int adults   = students.Count(s => s.Age >= 18);
long bigCnt  = largeList.LongCount();

// .NET 9+ CountBy — group অনুযায়ী গণনা
var byGrade  = students.CountBy(s => s.Grade);
foreach (var (grade, count) in byGrade)
    Console.WriteLine($"Grade {grade}: {count}");

// Any() — শুধু আছে কিনা check (Count()-এর চেয়ে দ্রুত)
bool hasFail = students.Any(s => s.Grade == "F");
```

---

## প্রশ্ন ২২ — LINQ দিয়ে গড় (Average) কীভাবে বের করবেন?

```csharp
var scores    = new[] { 80, 90, 75, 95, 88 };

double avg    = scores.Average();                   // 85.6
double avgAge = students.Average(s => s.Age);
double highAv = scores.Where(s => s >= 80).Average();

// Safe average — empty collection-এ default দেবে
double safe   = students.Any() ? students.Average(s => s.Age) : 0;
```

---

## প্রশ্ন ২৩ — LINQ-এ `Sum` ও `Min` method-এর ব্যবহার ব্যাখ্যা করুন।

```csharp
decimal total  = orders.Sum(o => o.Amount);
decimal min    = orders.Min(o => o.Amount);
decimal max    = orders.Max(o => o.Amount);

// .NET 6+ MinBy/MaxBy — পুরো object রিটার্ন করে
Order cheapest   = orders.MinBy(o => o.Amount);
Order expensive  = orders.MaxBy(o => o.Amount);
// cheapest.CustomerName — property access করা যাবে!

// একসাথে সব stats
var stats = new
{
    Total   = orders.Sum(o => o.Amount),
    Min     = orders.Min(o => o.Amount),
    Max     = orders.Max(o => o.Amount),
    Average = orders.Average(o => o.Amount)
};
```

---

## প্রশ্ন ২৪ — LINQ দিয়ে Inner Join কীভাবে করবেন?

Inner Join — **উভয় collection-এ matching key আছে** এমন element গুলোই ফলাফলে আসে।

```csharp
// Method Syntax
var result = employees.Join(
    departments,
    emp  => emp.DeptId,  // employees-এর key
    dept => dept.Id,     // departments-এর key
    (emp, dept) => new
    {
        Employee   = emp.Name,
        Department = dept.Name
    }
);

// Query Syntax
var result2 = from e in employees
              join d in departments on e.DeptId equals d.Id
              select new { Employee = e.Name, Department = d.Name };

// দ্রষ্টব্য: matching key না থাকলে সেই element বাদ পড়ে
```

---

## প্রশ্ন ২৫ — `ToList()` এবং `ToArray()`-এর পার্থক্য কী?

**`ToList()`** → `List<T>` রিটার্ন করে
- `Add()`, `Remove()` করা যায় (mutable)
- পরে element যোগ/বাদ দেওয়ার দরকার হলে ব্যবহার করুন

**`ToArray()`** → `T[]` রিটার্ন করে
- Fixed size, পরিবর্তন করা যায় না
- কম মেমোরি, iteration দ্রুত

```csharp
List<Student>  list  = query.ToList();   // mutable ✅
Student[]      array = query.ToArray();  // fixed size

// অন্য conversion operator
HashSet<string>       set  = query.Select(s => s.Name).ToHashSet();
Dictionary<int, User> dict = users.ToDictionary(u => u.Id);

// উভয়ই Immediate Execution করে!
```

---

## প্রশ্ন ২৬ — LINQ দিয়ে Aggregate Operation কীভাবে করবেন?

`Aggregate()` সবচেয়ে নমনীয় aggregation — custom accumulator function প্রয়োগ করে।

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// সমষ্টি
int sum     = numbers.Aggregate((acc, n) => acc + n);     // 15

// গুণফল (Factorial)
int product = numbers.Aggregate((acc, n) => acc * n);     // 120

// Seed সহ
int sum10   = numbers.Aggregate(10, (acc, n) => acc + n); // 25

// String join
var words    = new[] { "Language", "Integrated", "Query" };
string text  = words.Aggregate((acc, w) => acc + " " + w);
// "Language Integrated Query"
```

---

## প্রশ্ন ২৭ — LINQ-এ `SelectMany` method-এর ভূমিকা কী?

Nested collection গুলোকে **flatten** করে একটি single flat sequence-এ পরিণত করে।

```csharp
var students = new List<Student>
{
    new() { Name = "Rahim", Courses = new[] { "Math", "Physics" } },
    new() { Name = "Karim", Courses = new[] { "Chemistry", "Math" } },
};

// Select → nested: IEnumerable<IEnumerable<string>>
var nested = students.Select(s => s.Courses);

// SelectMany → flat: IEnumerable<string>
var all    = students.SelectMany(s => s.Courses);
// ["Math", "Physics", "Chemistry", "Math"]

// Parent সহ
var detail = students.SelectMany(
    s => s.Courses,
    (s, course) => new { s.Name, Course = course }
);

// Query Syntax
var result = from s in students
             from c in s.Courses
             select new { s.Name, Course = c };
```

---

## প্রশ্ন ২৮ — LINQ-এ `Any()` এবং `All()`-এর পার্থক্য কী?

**`Any()`** — যেকোনো একটি element শর্ত পূরণ করে?
- Empty collection → `false`
- প্রথম `true` পেলেই থামে (short-circuit)

**`All()`** — সব element শর্ত পূরণ করে?
- Empty collection → `true` (vacuous truth)
- প্রথম `false` পেলেই থামে (short-circuit)

```csharp
var scores = new[] { 75, 80, 90, 65, 55 };

bool anyPassed = scores.Any(s => s >= 60);  // true
bool allPassed = scores.All(s => s >= 60);  // false (55 fail)

// Any() — Count()>0 এর চেয়ে দ্রুত!
if (students.Any()) { }        // O(1) ✅
if (students.Count() > 0) { } // O(N) ❌
```

---

## প্রশ্ন ২৯ — `OrderBy` এবং `ThenBy` কীভাবে ব্যবহার করবেন?

```csharp
// Single sort
var byName    = employees.OrderBy(e => e.Name);
var bySalDesc = employees.OrderByDescending(e => e.Salary);

// Multi-level sort
var multi = employees
    .OrderBy(e => e.Department)
    .ThenByDescending(e => e.Salary)
    .ThenBy(e => e.Name);

// Query Syntax
var sorted = from e in employees
             orderby e.Department, e.Salary descending, e.Name
             select e;
```

---

## প্রশ্ন ৩০ — LINQ-এ `ElementAt` method কী করে?

নির্দিষ্ট index-এর element বের করে। Index না থাকলে Exception দেয়।

```csharp
var fruits = new[] { "Apple", "Banana", "Cherry", "Date" };

var second     = fruits.ElementAt(1);           // "Banana"
var safe       = fruits.ElementAtOrDefault(10); // null (Exception নেই)
var last       = fruits.ElementAt(^1);          // "Date" (.NET 8+)
var secondLast = fruits.ElementAt(^2);          // "Cherry"

// দ্রষ্টব্য: List/Array-এ indexer (fruits[1]) আরো দ্রুত।
// ElementAt শুধু IEnumerable-এর জন্য।
```

---

## প্রশ্ন ৩১ — LINQ query-তে Null Values কীভাবে হ্যান্ডেল করবেন?

```csharp
// Null filter
var withEmail = students.Where(s => s.Email != null);

// Null coalescing
var names = students.Select(s =>
    s.DisplayName ?? s.Username ?? "অজানা");

// Null-safe collection
IEnumerable<Student>? maybe = GetOrNull();
var result = maybe?.Where(s => s.Age > 18)
             ?? Enumerable.Empty<Student>();

// OfType দিয়ে null skip
var items  = new List<string?> { "Hi", null, "Bye", null };
var nonNull = items.OfType<string>(); // ["Hi", "Bye"]
```

---

## প্রশ্ন ৩২ — LINQ-এ Anonymous Types কী?

`new { }` syntax দিয়ে তৈরি temporary unnamed class। LINQ-এ প্রয়োজনীয় shape-এ ডেটা project করতে ব্যবহার হয়।

```csharp
var projected = students.Select(s => new
{
    FullName = s.FirstName + " " + s.LastName,
    s.Age,
    IsAdult  = s.Age >= 18
});

// বৈশিষ্ট্য:
// ✅ Immutable
// ✅ IDE IntelliSense কাজ করে
// ❌ Method-এর বাইরে return করা যায় না

// আধুনিক বিকল্প — Named Tuple (C# 7+)
var result = students.Select(s => (
    FullName: s.FirstName,
    s.Age
));

// আধুনিক বিকল্প — Record (C# 9+)
record StudentDto(string FullName, int Age);
var dtos = students.Select(s => new StudentDto(s.FirstName, s.Age));
```

---

## প্রশ্ন ৩৩ — LINQ Query Syntax-এ `let` keyword কীসের জন্য?

Intermediate variable সংজ্ঞায়িত করতে — complex expression-এর পুনরাবৃত্তি কমায়।

```csharp
// let ছাড়া — fullName দুইবার লিখতে হচ্ছে
var r1 = from s in students
         where (s.FirstName + " " + s.LastName).Length > 10
         select s.FirstName + " " + s.LastName;

// let দিয়ে — একবার define, বারবার ব্যবহার
var r2 = from s in students
         let fullName = s.FirstName + " " + s.LastName
         let age      = s.Age
         where fullName.Length > 10
         orderby fullName
         select new { FullName = fullName, Age = age };
```

---

## প্রশ্ন ৩৪ — LINQ-এ Empty Sequence কীভাবে হ্যান্ডেল করবেন?

```csharp
var students = new List<Student>(); // খালি list

// ❌ Exception দেবে:
// students.First()
// students.Min(s => s.Age)

// ✅ Safe উপায়:
var first   = students.FirstOrDefault();  // null
bool has    = students.Any();             // false

// DefaultIfEmpty
var result  = students
    .DefaultIfEmpty(new Student { Name = "কেউ নেই" });

// Empty sequence তৈরি
var empty   = Enumerable.Empty<Student>();

// Safe aggregate
double avg  = students.Any() ? students.Average(s => s.Age) : 0;
```

---

## প্রশ্ন ৩৫ — LINQ-এ `Concat` method-এর উদ্দেশ্য কী?

দুটি sequence কে একের পর এক জোড়া দেয়। `Union`-এর মতো নয় — **ডুপ্লিকেট সরায় না।**

```csharp
var a = new[] { 1, 2, 3 };
var b = new[] { 3, 4, 5 };

var concat = a.Concat(b); // [1, 2, 3, 3, 4, 5] — ডুপ্লিকেট থাকে
var union  = a.Union(b);  // [1, 2, 3, 4, 5]    — ডুপ্লিকেট সরে

// একাধিক
var all = list1.Concat(list2).Concat(list3);
```

---

## প্রশ্ন ৩৬ — LINQ দিয়ে দুটি Sequence কীভাবে Merge করবেন?

```csharp
// Concat — সব element, ডুপ্লিকেট সহ
var merged = list1.Concat(list2);

// Union — unique, ডুপ্লিকেট ছাড়া
var unique = list1.Union(list2);

// Zip — pair করে merge
var names  = new[] { "Rahim", "Karim" };
var scores = new[] { 90, 75 };
var zipped = names.Zip(scores, (n, s) => new { Name = n, Score = s });
// { Name="Rahim", Score=90 }
// { Name="Karim", Score=75 }

// Append / Prepend — একটি element যোগ
var withNew   = list.Append(newItem);
var withFirst = list.Prepend(firstItem);
```

---

## প্রশ্ন ৩৭ — LINQ-এ `Reverse` method কীসের জন্য?

Sequence-এর element গুলোর ক্রম উল্টে দেয়।

```csharp
var numbers  = new[] { 1, 2, 3, 4, 5 };
var reversed = numbers.Reverse(); // [5, 4, 3, 2, 1]

// String reverse
string word = "LINQ";
string rev  = new string(word.Reverse().ToArray()); // "QNIL"

// শেষ N element (TakeLast বেশি দক্ষ)
var last3 = numbers.TakeLast(3);          // [3, 4, 5] — ক্রম ঠিক থাকে
var last3R = numbers.Reverse().Take(3);   // [5, 4, 3] — উল্টো ক্রম

// ⚠️ Array.Reverse() ≠ LINQ Reverse()
// Array.Reverse(arr) — in-place (মূল array পরিবর্তন করে)
// arr.Reverse()      — নতুন sequence তৈরি করে
```

---

## প্রশ্ন ৩৮ — LINQ দিয়ে Collection কীভাবে Transform করবেন?

```csharp
// DTO-তে রূপান্তর
var dtos = students.Select(s => new StudentDto
{
    FullName = $"{s.FirstName} {s.LastName}",
    IsAdult  = s.Age >= 18
});

// Numeric transform — ১০% বেতন বৃদ্ধি
var increased = employees.Select(e => e.Salary * 1.10m);

// Conditional transform
var status = students.Select(s => new
{
    s.Name,
    Status = s.Score >= 60 ? "পাস" : "ফেল"
});

// Index সহ transform (.NET 9+)
var indexed = students.Index()
    .Select(x => $"{x.Index + 1}. {x.Item.Name}");

// Flatten + transform
var allCourses = departments
    .SelectMany(d => d.Courses)
    .Select(c => c.Name.ToUpper());
```

---

## প্রশ্ন ৩৯ — `OrderBy` এবং `OrderByDescending`-এর পার্থক্য কী?

**`OrderBy`** — Ascending (ছোট → বড়, A → Z, পুরনো → নতুন)

**`OrderByDescending`** — Descending (বড় → ছোট, Z → A, নতুন → পুরনো)

```csharp
// Price ascending — সস্তা আগে
var cheapFirst = products.OrderBy(p => p.Price);

// Price descending — দামি আগে
var dearFirst  = products.OrderByDescending(p => p.Price);

// Blog/social media — নতুন post আগে
var latest = posts.OrderByDescending(p => p.CreatedAt);

// Multi-level
var result = employees
    .OrderBy(e => e.Department)
    .ThenByDescending(e => e.Salary)
    .ThenBy(e => e.Name);
```

---

## প্রশ্ন ৪০ — LINQ-এ Common Performance Pitfalls কী কী?

### ⚠️ সমস্যা ১ — Multiple Enumeration

```csharp
// ❌ খারাপ — query দুইবার execute হয়
var query = GetData().Where(x => x.IsActive);
int count = query.Count();   // ১ম execute
var list  = query.ToList();  // ২য় execute

// ✅ ভালো — একবার cache করুন
var cached = GetData().Where(x => x.IsActive).ToList();
int count  = cached.Count; // O(1), আর execute নেই
```

### ⚠️ সমস্যা ২ — N+1 Query (EF Core)

```csharp
// ❌ খারাপ — প্রতিটি order-এর জন্য আলাদা SELECT!
var orders = _context.Orders.ToList();
foreach (var o in orders)
    _ = o.Customer.Name; // N টি extra query!

// ✅ ভালো — Eager Loading
var orders = _context.Orders
    .Include(o => o.Customer)
    .ToList();
```

### ⚠️ সমস্যা ৩ — Count() > 0 এর বদলে Any() না ব্যবহার করা

```csharp
if (students.Count() > 0) { }  // ❌ O(N) — ধীর
if (students.Any()) { }         // ✅ O(1) — দ্রুত
```

### ⚠️ সমস্যা ৪ — AsEnumerable() তাড়াতাড়ি কল করা (EF Core)

```csharp
// ❌ খারাপ — সব row আনে তারপর C#-এ filter!
var r = _context.Users.AsEnumerable().Where(u => u.IsActive);

// ✅ ভালো — server-এ filter, তারপর আনো
var r = _context.Users.Where(u => u.IsActive).ToList();
```

### ⚠️ সমস্যা ৫ — Filter দেরিতে দেওয়া

```csharp
// ❌ খারাপ — আগে সব project করে, তারপর filter
var r = employees
    .Select(e => new { e.Name, e.Salary })
    .Where(e => e.Salary > 50000);

// ✅ ভালো — আগে filter, তারপর project
var r = employees
    .Where(e => e.Salary > 50000)
    .Select(e => new { e.Name, e.Salary });
```

### ⚠️ সমস্যা ৬ — Sync ব্যবহার EF Core-এ

```csharp
// ❌ Thread block করে
var users = _context.Users.ToList();

// ✅ Non-blocking
var users = await _context.Users.ToListAsync();
```

---

> **সারসংক্ষেপ:**  
> LINQ অত্যন্ত শক্তিশালী। Deferred Execution, `IQueryable` বনাম `IEnumerable` পার্থক্য এবং EF Core integration সম্পর্কে সচেতন থাকলে performance-friendly ও clean কোড লেখা সম্ভব।

---
*সর্বশেষ আপডেট: .NET 9 | C# 13 | 2026*
