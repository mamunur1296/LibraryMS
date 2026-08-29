# Language Integrated Query (LINQ) — স্ট্যান্ডার্ড Query Operators ও আধুনিক ফিচার গাইড

Language Integrated Query (LINQ) হলো C# 3.0 (.NET Framework 3.5)-এ প্রবর্তিত একটি শক্তিশালী ফিচার। এটি বিভিন্ন ডেটা সোর্স থেকে (যেমন: in-memory কালেকশন, ডেটাবেজ, XML ফাইল ইত্যাদি) সরাসরি C# কোডে ডেটা query, filter, sort, group এবং transform করার জন্য একটি consistent, type-safe এবং declarative সিনট্যাক্স প্রদান করে।

এই ডকুমেন্টটি সমস্ত Standard Query Operators, Execution Model, Syntax Type এবং **.NET 9 (2026)** পর্যন্ত সব আধুনিক সংযোজনের একটি সম্পূর্ণ রেফারেন্স হিসেবে তৈরি করা হয়েছে।

---

## বিষয়সূচি (Table of Contents)
1. [কোর আর্কিটেকচার ও প্রোভাইডার](#1-কোর-আর্কিটেকচার-ও-প্রোভাইডার)
2. [LINQ সিনট্যাক্সের ধরন](#2-linq-সিনট্যাক্সের-ধরন)
3. [Execution Model (Deferred বনাম Immediate)](#3-execution-model-deferred-বনাম-immediate)
4. [ক্যাটাগরি অনুযায়ী Standard Query Operators](#4-ক্যাটাগরি-অনুযায়ী-standard-query-operators)
5. [আধুনিক LINQ আপডেট (.NET 6 থেকে .NET 9)](#5-আধুনিক-linq-আপডেট-net-6-থেকে-net-9)
6. [Best Practices ও পারফরম্যান্স টিপস](#6-best-practices-ও-পারফরম্যান্স-টিপস)

---

## 1. কোর আর্কিটেকচার ও প্রোভাইডার

LINQ বিভিন্ন স্টোরেজ ফরম্যাটের উপরে একটি abstraction layer হিসেবে কাজ করে — এটি query কে সেই স্টোরেজের উপযুক্ত কমান্ডে রূপান্তরিত করে। সবচেয়ে বেশি ব্যবহৃত প্রোভাইডারগুলো হলো:

*   **LINQ to Objects:** `List<T>`, array, `Dictionary<TKey, TValue>` বা যেকোনো `IEnumerable<T>` implement করা in-memory কালেকশন query করতে ব্যবহৃত হয়।
*   **LINQ to Entities (Entity Framework Core):** LINQ query কে SQL query-তে রূপান্তরিত করে `IQueryable<T>` ব্যবহার করে SQL Server, PostgreSQL-এর মতো relational ডেটাবেজে execute করে।
*   **LINQ to XML (XLINQ):** XML ডেটা load, query, modify এবং serialize করার জন্য ডিজাইন করা API।
*   **LINQ to DataSet:** ADO.NET `DataSet` ও `DataTable` অবজেক্ট query করতে ব্যবহৃত হয়।

---

## 2. LINQ সিনট্যাক্সের ধরন

C# LINQ query লেখার জন্য দুটি প্রধান সিনট্যাক্স সাপোর্ট করে। দুটোই একই IL (Intermediate Language) কোডে compile হয়।

### A. Query Syntax (Declarative Expression Syntax)
SQL-এর মতো দেখতে। জটিল join ও grouping-এর জন্য পরিষ্কার ও পাঠযোগ্য।
```csharp
var query = from student in students
            where student.Age > 18
            orderby student.Name
            select student;
```

### B. Method Syntax (Fluent / Lambda Syntax)
Extension method ও lambda expression ব্যবহার করে। এটি বেশি versatile কারণ অনেক operator (যেমন `Take`, `Skip`, `FirstOrDefault`) শুধু Method Syntax-এই ব্যবহার করা যায়।
```csharp
var query = students.Where(s => s.Age > 18)
                    .OrderBy(s => s.Name);
```

### C. Mixed Syntax (মিশ্র সিনট্যাক্স)
উভয়ের সমন্বয়। Query expression-এ element operator কল করার সময় কখনো কখনো এটি দরকার হয়:
```csharp
var firstAdult = (from student in students
                  where student.Age > 18
                  select student).FirstOrDefault();
```

---

## 3. Execution Model (Deferred বনাম Immediate)

Execution আচরণ বোঝা পারফরম্যান্স উন্নয়ন এবং বাগ প্রতিরোধের জন্য অত্যন্ত গুরুত্বপূর্ণ।

### A. Deferred Execution (অলস মূল্যায়ন / Lazy Evaluation)
Query সংজ্ঞায়িত করার সময় execute হয় না; ডেটা iterate করার সময় (যেমন `foreach` loop-এ) execute হয়।

*   **Operators:** `Where`, `Select`, `SelectMany`, `Take`, `Skip`, `OrderBy`, `GroupBy`।
*   **সুবিধা:** Query পুনর্ব্যবহার এবং dynamic query তৈরিকে সমর্থন করে।

```csharp
var list = new List<int> { 1, 2, 3 };
var query = list.Where(x => x > 1); // Query সংজ্ঞায়িত হয়েছে, কিন্তু execute হয়নি

list.Add(4); // সংজ্ঞায়িত করার পরে তালিকায় পরিবর্তন

foreach (var item in query) // এখানেই Execution শুরু হয়
{
    Console.WriteLine(item); // Output: 2, 3, 4
}
```

> **গুরুত্বপূর্ণ লক্ষ্য:** query define করার পরে list-এ `4` যোগ করা হয়েছে, কিন্তু তারপরও সেটি result-এ এসেছে — এটাই Deferred Execution-এর শক্তি।

### B. Immediate Execution (তাৎক্ষণিক মূল্যায়ন / Eager Evaluation)
Query সংজ্ঞায়িত হওয়ার সাথে সাথেই execute হয় এবং ফলাফল cache হয়।

*   **Operators:** Element operators (`First`, `Last`, `Single`), Aggregation operators (`Count`, `Sum`, `Max`, `Average`), এবং Conversion operators (`ToList`, `ToArray`, `ToDictionary`)।

```csharp
var list = new List<int> { 1, 2, 3 };
var count = list.Where(x => x > 1).Count(); // তাৎক্ষণিকভাবে Execute হয়
```

---

## 4. ক্যাটাগরি অনুযায়ী Standard Query Operators

নিচে C# Standard Query Operators-এর সম্পূর্ণ তালিকা দেওয়া হলো:

---

### 1. Filtering (ফিল্টারিং / বাছাই করা)
কোনো শর্ত বা টাইপের উপর ভিত্তি করে একটি sequence ফিল্টার করে।

| Operator | কাজ |
|---|---|
| `Where` | একটি predicate function-এর উপর ভিত্তি করে element ফিল্টার করে। |
| `OfType` | নির্দিষ্ট type-এ cast করার যোগ্যতার উপর ভিত্তি করে element ফিল্টার করে। |

```csharp
// Method Syntax
var adults = people.Where(p => p.Age >= 18);
var stringOnly = mixedList.OfType<string>();

// Query Syntax
var adults = from p in people
             where p.Age >= 18
             select p;
```

---

### 2. Projection (রূপান্তর / ম্যাপিং)
একটি sequence-এর element গুলোকে নতুন রূপে রূপান্তরিত করে।

| Operator | কাজ |
|---|---|
| `Select` | প্রতিটি element কে নতুন রূপে project করে (mapping)। |
| `SelectMany` | নেস্টেড sequence-এর sequence কে একটি একক flat sequence-এ পরিণত করে। |

```csharp
// Select: প্রতিটি ব্যক্তির নাম বের করা
var names = people.Select(p => p.Name);

// SelectMany: নেস্টেড array/list flatten করে
// প্রতিটি Employee-এর একাধিক PhoneNumber আছে
var allPhones = employees.SelectMany(emp => emp.PhoneNumbers);
```

---

### 3. Sorting (সাজানো)
একটি sequence-এর element গুলোকে নির্দিষ্ট ক্রমে সাজায়।

| Operator | কাজ |
|---|---|
| `OrderBy` | উর্ধ্বক্রমে (ascending) সাজায়। |
| `OrderByDescending` | অবরোহক্রমে (descending) সাজায়। |
| `ThenBy` | প্রাথমিক sort-এর পরে secondary ascending sort করে। |
| `ThenByDescending` | প্রাথমিক sort-এর পরে secondary descending sort করে। |
| `Reverse` | element গুলোর ক্রম উল্টে দেয়। |

```csharp
// প্রথমে LastName দিয়ে ascending sort, তারপর Age দিয়ে descending sort
var sorted = people.OrderBy(p => p.LastName)
                   .ThenByDescending(p => p.Age);
```

---

### 4. Grouping (গ্রুপ করা)
একটি common key শেয়ার করা element গুলোকে একসাথে গ্রুপ করে।

| Operator | কাজ |
|---|---|
| `GroupBy` | Element গ্রুপ করে এবং `IEnumerable<IGrouping<TKey, TSource>>` রিটার্ন করে (Deferred)। |
| `ToLookup` | Element গ্রুপ করে একটি lookup container রিটার্ন করে (Immediate — তাৎক্ষণিক)। |

```csharp
// বয়স অনুযায়ী মানুষ গ্রুপ করা
var groupedByAge = people.GroupBy(p => p.Age);

foreach (var group in groupedByAge)
{
    Console.WriteLine($"বয়স {group.Key}: {group.Count()} জন");
}
```

---

### 5. Joining (যুক্ত করা)
Matching key-এর উপর ভিত্তি করে দুটি sequence কে correlate করে।

| Operator | কাজ |
|---|---|
| `Join` | দুটি কালেকশনের inner join। |
| `GroupJoin` | দুটি কালেকশনের left outer join, matching element গুলো গ্রুপ করে। |

```csharp
// Orders এবং Customers inner join
var innerJoin = orders.Join(
    customers,
    o => o.CustomerId,   // orders-এর key
    c => c.Id,           // customers-এর key
    (order, customer) => new { order.Id, customer.Name }
);
```

---

### 6. Set Operations (সেট অপারেশন)
কালেকশনের মধ্যে element তুলনা করে।

| Operator | কাজ |
|---|---|
| `Distinct` | ডুপ্লিকেট মান সরিয়ে দেয়। |
| `Union` | উভয় কালেকশন থেকে unique element একত্রিত করে রিটার্ন করে। |
| `Intersect` | উভয় কালেকশনে common element রিটার্ন করে। |
| `Except` | প্রথম কালেকশনের element যা দ্বিতীয়টিতে নেই সেগুলো রিটার্ন করে। |

```csharp
var a = new[] { 1, 2, 3, 4 };
var b = new[] { 3, 4, 5, 6 };

var distinct   = a.Distinct();          // [1, 2, 3, 4]
var union      = a.Union(b);            // [1, 2, 3, 4, 5, 6]
var intersect  = a.Intersect(b);        // [3, 4]
var except     = a.Except(b);           // [1, 2]
```

---

### 7. Aggregation (একত্রীকরণ)
একটি কালেকশন থেকে একটি একক মান গণনা করে।

| Operator | কাজ |
|---|---|
| `Count` | Element সংখ্যা গণনা করে (int)। |
| `LongCount` | বড় কালেকশনের জন্য Element সংখ্যা গণনা করে (long)। |
| `Sum` | সব numeric element-এর যোগফল বের করে। |
| `Average` | সব numeric element-এর গড় বের করে। |
| `Min` | সবচেয়ে ছোট element খুঁজে বের করে। |
| `Max` | সবচেয়ে বড় element খুঁজে বের করে। |
| `Aggregate` | কাস্টম accumulator function প্রয়োগ করে। |

```csharp
// Aggregate দিয়ে Factorial (গুণফল) বের করা
var numbers = new[] { 1, 2, 3, 4 };
int factorial = numbers.Aggregate((prev, next) => prev * next); // ফলাফল: 24

// Aggregate দিয়ে সংখ্যার সমষ্টি
int total = numbers.Aggregate(0, (sum, n) => sum + n); // ফলাফল: 10
```

---

### 8. Element Operations (নির্দিষ্ট Element বের করা)
Collection থেকে একটি নির্দিষ্ট single element বের করে।

| Operator | কাজ |
|---|---|
| `First` | প্রথম element রিটার্ন করে; sequence খালি হলে Exception দেয়। |
| `FirstOrDefault` | প্রথম element রিটার্ন করে; খালি হলে `default(T)` রিটার্ন করে। |
| `Last` | শেষ element রিটার্ন করে; sequence খালি হলে Exception দেয়। |
| `LastOrDefault` | শেষ element রিটার্ন করে; খালি হলে `default(T)` রিটার্ন করে। |
| `Single` | একমাত্র element রিটার্ন করে; 0 বা 1-এর বেশি থাকলে Exception দেয়। |
| `SingleOrDefault` | একমাত্র element রিটার্ন করে; 1-এর বেশি থাকলে Exception দেয়। |
| `ElementAt` | নির্দিষ্ট index-এর element রিটার্ন করে। |
| `ElementAtOrDefault` | নির্দিষ্ট index-এর element রিটার্ন করে; না থাকলে `default(T)` রিটার্ন করে। |

```csharp
var numbers = new[] { 5, 10, 15, 20 };

int first  = numbers.First();            // 5
int last   = numbers.Last();             // 20
int third  = numbers.ElementAt(2);      // 15

// শর্ত দিয়ে:
int firstBig = numbers.First(n => n > 10); // 15
```

---

### 9. Partitioning (ভাগ করা)
একটি sequence ভাগ করে বা অংশ বিশেষ বের করে।

| Operator | কাজ |
|---|---|
| `Take` | শুরু থেকে `N` সংখ্যক element নেয়। |
| `TakeWhile` | কোনো শর্ত সত্য থাকা পর্যন্ত element নেয়। |
| `Skip` | শুরু থেকে `N` সংখ্যক element এড়িয়ে বাকিগুলো রিটার্ন করে। |
| `SkipWhile` | কোনো শর্ত সত্য থাকা পর্যন্ত element এড়িয়ে যায়। |

```csharp
var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };

var firstThree  = data.Take(3);               // [1, 2, 3]
var afterThree  = data.Skip(3);               // [4, 5, 6, 7, 8]
var page2       = data.Skip(3).Take(3);       // [4, 5, 6] — Pagination-এর জন্য দরকারী!
```

---

### 10. Conversion Operators (রূপান্তর)
LINQ result কে বিভিন্ন collection type-এ রূপান্তর করে।

| Operator | কাজ |
|---|---|
| `ToList` | `IEnumerable<T>` কে `List<T>`-তে রূপান্তর করে এবং তাৎক্ষণিকভাবে execute করে। |
| `ToArray` | `IEnumerable<T>` কে `T[]`-তে রূপান্তর করে এবং তাৎক্ষণিকভাবে execute করে। |
| `ToDictionary` | Key selector দিয়ে `Dictionary<TKey, TValue>`-তে রূপান্তর করে। |
| `ToHashSet` | `HashSet<T>`-তে রূপান্তর করে (duplicate স্বয়ংক্রিয়ভাবে বাদ যায়)। |
| `AsEnumerable` | `IQueryable<T>` কে `IEnumerable<T>`-এ নামিয়ে আনে (client-side evaluation-এর জন্য)। |
| `AsQueryable` | `IEnumerable<T>` কে `IQueryable<T>`-এ উন্নীত করে। |
| `Cast` | Non-generic sequence কে নির্দিষ্ট type-এ cast করে। |

```csharp
// ToDictionary উদাহরণ
var dict = people.ToDictionary(p => p.Id, p => p.Name);
```

---

### 11. Quantifier Operations (পরিমাণ নির্ধারক)
কালেকশনের কিছু বা সব element সম্পর্কে boolean ফলাফল দেয়।

| Operator | কাজ |
|---|---|
| `Any` | যদি কোনো element শর্ত পূরণ করে তাহলে `true` রিটার্ন করে। |
| `All` | যদি সব element শর্ত পূরণ করে তাহলে `true` রিটার্ন করে। |
| `Contains` | কালেকশনে নির্দিষ্ট element আছে কিনা check করে। |

```csharp
bool hasAdult   = people.Any(p => p.Age >= 18);   // যেকোনো একজন প্রাপ্তবয়স্ক আছে?
bool allAdults  = people.All(p => p.Age >= 18);   // সবাই প্রাপ্তবয়স্ক?
bool hasBob     = names.Contains("Bob");           // "Bob" আছে?
```

---

### 12. Generation Operators (নতুন Sequence তৈরি করা)
নতুন sequence জেনারেট করার জন্য ব্যবহৃত হয়।

| Operator | কাজ |
|---|---|
| `Range` | একটি নির্দিষ্ট range-এর integer sequence তৈরি করে। |
| `Repeat` | একটি মান নির্দিষ্ট সংখ্যকবার পুনরাবৃত্তি করে sequence তৈরি করে। |
| `Empty` | একটি empty sequence তৈরি করে। |
| `DefaultIfEmpty` | যদি sequence খালি থাকে তাহলে default value সহ sequence রিটার্ন করে। |

```csharp
var rangeSeq  = Enumerable.Range(1, 5);     // [1, 2, 3, 4, 5]
var repeated  = Enumerable.Repeat("Hi", 3); // ["Hi", "Hi", "Hi"]
var empty     = Enumerable.Empty<int>();    // []
```

---

## 5. আধুনিক LINQ আপডেট (.NET 6 থেকে .NET 9)

Microsoft ক্রমাগত LINQ-কে optimize ও expand করছে পারফরম্যান্স এবং কোডের পাঠযোগ্যতা উন্নত করতে।

---

### A. .NET 6-এর নতুন Operators

**1. `Chunk`** — Sequence ভাগ করে chunk তৈরি করা
নির্দিষ্ট সর্বোচ্চ আকারের chunk-এ একটি sequence বিভক্ত করে। Batch processing-এর জন্য অত্যন্ত উপযোগী।
```csharp
int[] numbers = { 1, 2, 3, 4, 5, 6, 7 };
IEnumerable<int[]> chunks = numbers.Chunk(3);
// ফলাফল: [[1, 2, 3], [4, 5, 6], [7]]
```

**2. `*By` Operators** — Key-based অপারেশন
`Select` chain করা বা জটিল mapping ছাড়াই সরাসরি key extract করার সুবিধা দেয়।

| Operator | কাজ |
|---|---|
| `DistinctBy` | নির্দিষ্ট key-এর উপর ভিত্তি করে unique element বের করে। |
| `ExceptBy` | Key-এর উপর ভিত্তি করে দুটি sequence-এর পার্থক্য বের করে। |
| `IntersectBy` | Key-এর উপর ভিত্তি করে common element বের করে। |
| `UnionBy` | Key-এর উপর ভিত্তি করে unique element combine করে। |
| `MinBy` | নির্দিষ্ট key সবচেয়ে ছোট যে element তাকে রিটার্ন করে। |
| `MaxBy` | নির্দিষ্ট key সবচেয়ে বড় যে element তাকে রিটার্ন করে। |

```csharp
// সর্বোচ্চ বয়সের ব্যক্তি সরাসরি বের করা:
Person oldest = people.MaxBy(p => p.Age);

// বয়সের উপর ভিত্তি করে unique ব্যক্তি বের করা:
var uniqueByAge = people.DistinctBy(p => p.Age);
```

**3. `FirstOrDefault` / `LastOrDefault` / `SingleOrDefault`-এ কাস্টম Default Value**
`default(T)` এর পরিবর্তে কাস্টম fallback value নির্দিষ্ট করা যায়।
```csharp
int firstOrMinusOne = numbers.FirstOrDefault(n => n > 100, -1);
// কোনো element 100-এর বেশি না হলে -1 রিটার্ন করবে
```

**4. `Take`-এ Range সাপোর্ট**
C# `Range` অবজেক্ট pass করা যায়।
```csharp
var middleItems = numbers.Take(2..5); // index 2 থেকে 5 পর্যন্ত element নেয়
```

---

### B. .NET 9-এর নতুন Operators (2024 সালের শেষে Released / 2026 পর্যন্ত Current)

এই সংযোজনগুলো পারফরম্যান্স অপ্টিমাইজ করে এবং boilerplate কোড দূর করে।

---

**1. `Index()`** — Index সহ Iteration
প্রতিটি element-কে তার index-এর সাথে pair করে, `.Select((item, index) => ...)` লেখার প্রয়োজনীয়তা দূর করে।
```csharp
var lines = new[] { "Apple", "Banana", "Cherry" };
foreach (var (index, item) in lines.Index())
{
    Console.WriteLine($"লাইন {index}: {item}");
}
// Output:
// লাইন 0: Apple
// লাইন 1: Banana
// লাইন 2: Cherry
```

---

**2. `CountBy()`** — Key অনুযায়ী গণনা
Key অনুযায়ী element গণনা করে, `GroupBy` → `Select` → `Count` chain ছাড়াই। অনেক কম মেমোরি ব্যবহার করে।
```csharp
var words = new[] { "apple", "banana", "apricot", "banana" };

foreach (var group in words.CountBy(w => w[0]))
{
    Console.WriteLine($"'{group.Key}' দিয়ে শুরু: {group.Value} টি");
}
// Output:
// 'a' দিয়ে শুরু: 2 টি
// 'b' দিয়ে শুরু: 2 টি

// পুরনো পদ্ধতি (verbose):
// words.GroupBy(w => w[0]).Select(g => new { Key = g.Key, Count = g.Count() });
```

---

**3. `AggregateBy()`** — Key-based একক পদক্ষেপে Aggregation
Seed state সহ key-based aggregation একটি single step-এ করে। intermediate grouping allocation এড়িয়ে চলায় অত্যন্ত দ্রুত।
```csharp
var transactions = new[]
{
    new { Department = "Sales", Amount = 100 },
    new { Department = "IT",    Amount = 250 },
    new { Department = "Sales", Amount = 50  }
};

// Department অনুযায়ী মোট Amount গণনা
var summary = transactions.AggregateBy(
    keySelector: t => t.Department,
    seed: 0,
    func: (currentSum, transaction) => currentSum + transaction.Amount
);

foreach (var (key, total) in summary)
{
    Console.WriteLine($"{key}: {total} টাকা");
}
// Output:
// Sales: 150 টাকা
// IT: 250 টাকা
```

---

## 6. Best Practices ও পারফরম্যান্স টিপস

**1. `Count() > 0`-এর পরিবর্তে `Any()` ব্যবহার করুন:**
`Count()` সম্পূর্ণ collection check করে, যা deferred sequence-এর জন্য O(N) হতে পারে। `Any()` প্রথম element খুঁজে পাওয়ার সাথে সাথেই `true` রিটার্ন করে — O(1)।
```csharp
// খারাপ পদ্ধতি ❌
if (people.Count() > 0) { ... }

// ভালো পদ্ধতি ✅
if (people.Any()) { ... }
```

**2. Multiple Enumeration এড়িয়ে চলুন:**
একটি deferred query একাধিকবার iterate করলে query একাধিকবার execute হয়। `.ToList()` বা `.ToArray()` ডেকে result মেমোরিতে cache করুন।
```csharp
// খারাপ পদ্ধতি ❌ — দুইবার query execute হবে
var results = data.Where(x => x.IsActive);
var count = results.Count();     // এখানে একবার execute
foreach (var item in results) { } // আবার এখানে execute

// ভালো পদ্ধতি ✅
var results = data.Where(x => x.IsActive).ToList(); // একবারই execute
var count = results.Count;       // O(1), আর execute হয় না
```

**3. `.NET 6+`-এর `*By` Operators ব্যবহার করুন:**
`.OrderBy(x => x.Age).First()` এর পরিবর্তে `.MinBy(x => x.Age)` ব্যবহার করুন — দ্রুততর এবং পরিষ্কার।

**4. `Where` আগে রাখুন (Filter Early):**
`Where` statement কে query chain-এর যতটা সম্ভব শুরুতে রাখুন, যাতে পরবর্তী projection বা sorting-এ কম element প্রক্রিয়া করতে হয়।
```csharp
// ভালো পদ্ধতি ✅ — আগে filter, তারপর project
var result = people.Where(p => p.Age > 18).Select(p => p.Name);

// অপেক্ষাকৃত কম দক্ষ ❌
var result = people.Select(p => new { p.Name, p.Age }).Where(p => p.Age > 18).Select(p => p.Name);
```

**5. EF Core-এ Async alternative ব্যবহার করুন:**
Database query-তে সবসময় `ToListAsync()`, `FirstOrDefaultAsync()`, `AnyAsync()` ইত্যাদি ব্যবহার করুন, যাতে thread pool responsive থাকে।
```csharp
// Web API / EF Core-এ সবসময় Async ✅
var users = await _context.Users.Where(u => u.IsActive).ToListAsync();
```

**6. IQueryable বনাম IEnumerable বুঝুন:**
EF Core-এ `IQueryable<T>` ব্যবহার করলে LINQ query SQL-এ convert হয়ে server-এ execute হয়। `AsEnumerable()` বা `ToList()` call করলে বাকি query client-side-এ (C#-এ) execute হয়।

---

> **সতর্কতা:** `Select`, `Where` ইত্যাদি operator গুলোতে কোনো `null` reference আসতে পারে কিনা সর্বদা যাচাই করুন। Nullable reference type (`?`) এবং null-conditional operator (`?.`) ব্যবহার করুন।

---

*সর্বশেষ আপডেট: .NET 9 (2026) | রেফারেন্স: Microsoft Docs — System.Linq.Enumerable*
