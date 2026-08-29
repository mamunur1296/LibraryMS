# LINQ ইন্টারভিউ প্রশ্ন ও উত্তর — Intermediate Level (বাংলায়)

> C# LINQ সংক্রান্ত ৪০টি Intermediate প্রশ্ন ও বিস্তারিত উত্তর | .NET 9 | C# 13

---

## প্রশ্ন ০১ — LINQ to SQL এবং LINQ to Entities-এর পার্থক্য কী?

**LINQ to SQL**
- শুধুমাত্র **Microsoft SQL Server** সমর্থন করে
- `System.Data.Linq` namespace ব্যবহার করে
- বর্তমানে **Deprecated** — নতুন project-এ ব্যবহার করা উচিত নয়

**LINQ to Entities (Entity Framework Core)**
- SQL Server, PostgreSQL, MySQL, SQLite — **যেকোনো DB** সমর্থন
- Code First বা DB First উভয় approach সমর্থিত
- Migration, Lazy/Eager Loading, Change Tracking সমর্থিত
- বর্তমানে **সক্রিয়ভাবে maintained**

```csharp
// LINQ to Entities (EF Core)
var activeUsers = await _context.Users
    .Where(u => u.IsActive && u.Age > 18)
    .OrderBy(u => u.Name)
    .ToListAsync();
// SQL: SELECT * FROM Users WHERE IsActive=1 AND Age>18 ORDER BY Name
```

---

## প্রশ্ন ০২ — LINQ-এ "Projection" ধারণাটি ব্যাখ্যা করুন।

**Projection** মানে প্রতিটি element-কে **নতুন রূপে রূপান্তরিত** করা।
LINQ-এ এটি `Select` ও `SelectMany` operator দিয়ে করা হয়।

```csharp
var employees = GetEmployees();

// ১. Property projection
var names = employees.Select(e => e.Name);

// ২. Anonymous type projection
var summary = employees.Select(e => new
{
    FullName  = $"{e.FirstName} {e.LastName}",
    IsManager = e.Title.Contains("Manager")
});

// ৩. DTO projection (EF Core-এ N+1 এড়াতে কার্যকর)
var dtos = employees.Select(e => new EmployeeDto
{
    Id   = e.Id,
    Name = e.Name
});
```

---

## প্রশ্ন ০৩ — LINQ-এ `Union` method-এর উদ্দেশ্য কী?

`Union` দুটি sequence একত্রিত করে **unique element** রিটার্ন করে।
ডুপ্লিকেট স্বয়ংক্রিয়ভাবে বাদ দেয়।

```csharp
var a = new[] { 1, 2, 3, 4 };
var b = new[] { 3, 4, 5, 6 };

var union  = a.Union(b);   // [1, 2, 3, 4, 5, 6] — ডুপ্লিকেট বাদ
var concat = a.Concat(b);  // [1, 2, 3, 4, 3, 4, 5, 6] — ডুপ্লিকেট থাকে

// .NET 6+ UnionBy — key দিয়ে unique করা
var uniqueByAge = listA.UnionBy(listB, p => p.Age);

// String — case-insensitive union
var cities = listA.Union(listB, StringComparer.OrdinalIgnoreCase);
```

---

## প্রশ্ন ০৪ — LINQ কীভাবে Multiple Collections নিয়ে কাজ করে?

```csharp
// Join (inner join)
var joined = orders.Join(customers,
    o => o.CustomerId, c => c.Id,
    (o, c) => new { o.Id, c.Name });

// GroupJoin (left outer join)
var grouped = customers.GroupJoin(orders,
    c => c.Id, o => o.CustomerId,
    (c, os) => new { Customer = c.Name, Orders = os });

// SelectMany (flatten nested)
var allCourses = students.SelectMany(s => s.Courses);

// Zip (pair করে merge)
var paired = names.Zip(scores, (n, s) => new { Name = n, Score = s });

// Set operators
var combined = listA.Union(listB);      // unique মিলন
var common   = listA.Intersect(listB);  // intersection
var diff     = listA.Except(listB);     // পার্থক্য
```

---

## প্রশ্ন ০৫ — `Select` এবং `SelectMany`-এর পার্থক্য কী?

**`Select`** — প্রতিটি element → একটি নতুন element (1-to-1)
**`SelectMany`** — প্রতিটি element → শূন্য বা একাধিক element, flat করে (1-to-many)

```csharp
var students = new[]
{
    new { Name = "Rahim", Courses = new[] { "Math", "Physics" } },
    new { Name = "Karim", Courses = new[] { "Chemistry", "Math" } },
};

// Select — nested result: IEnumerable<string[]>
var nested = students.Select(s => s.Courses);
// [["Math","Physics"], ["Chemistry","Math"]]

// SelectMany — flat result: IEnumerable<string>
var flat = students.SelectMany(s => s.Courses);
// ["Math", "Physics", "Chemistry", "Math"]

// SelectMany + parent info
var detail = students.SelectMany(
    s => s.Courses,
    (s, course) => new { s.Name, Course = course }
);
```

---

## প্রশ্ন ০৬ — LINQ-এ `Aggregate` দিয়ে Custom Aggregation কীভাবে করবেন?

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// Seed ছাড়া
int sum     = numbers.Aggregate((acc, n) => acc + n);      // 15
int product = numbers.Aggregate((acc, n) => acc * n);      // 120

// Seed সহ
int result  = numbers.Aggregate(100, (acc, n) => acc + n); // 115

// resultSelector সহ
string res  = numbers.Aggregate(
    seed: 0,
    func: (acc, n) => acc + n,
    resultSelector: total => $"মোট: {total}"
); // "মোট: 15"

// Complex — শব্দের frequency count
var words = new[] { "apple", "banana", "apple", "cherry" };
var freq  = words.Aggregate(
    new Dictionary<string, int>(),
    (dict, word) =>
    {
        dict[word] = dict.GetValueOrDefault(word) + 1;
        return dict;
    }
); // { "apple":2, "banana":1, "cherry":1 }
```

---

## প্রশ্ন ০৭ — `TakeWhile` এবং `SkipWhile`-এর পার্থক্য কী?

**`TakeWhile`** — শর্ত `true` থাকা পর্যন্ত element নেয়, প্রথম `false`-এ সব বাদ
**`SkipWhile`** — শর্ত `true` থাকা পর্যন্ত skip করে, প্রথম `false` থেকে সব রাখে

```csharp
var numbers = new[] { 2, 4, 6, 7, 8, 10 };

var taken   = numbers.TakeWhile(n => n % 2 == 0);
// [2, 4, 6] — 7 odd, তাই থামে

var skipped = numbers.SkipWhile(n => n % 2 == 0);
// [7, 8, 10] — 7 থেকে বাকি সব রাখে

// গুরুত্বপূর্ণ: শর্ত একবার false হলে আর check করে না!
var data = new[] { 1, 3, 2, 4, 5 };
var tw   = data.TakeWhile(n => n % 2 != 0);
// [1, 3] — 2 even, তাই থামে (4,5 বাদ এমনকি 5 odd হলেও)
```

---

## প্রশ্ন ০৮ — `GroupJoin` operator কী এবং কীভাবে ব্যবহার হয়?

`GroupJoin` একটি **Left Outer Join** — বাম দিকের প্রতিটি element-এর সাথে ডান দিকের matching element গুলো গ্রুপ করে। কোনো match না থাকলে empty collection পাওয়া যায়।

```csharp
var customers = new[]
{
    new { Id = 1, Name = "Rahim" },
    new { Id = 2, Name = "Karim" },
    new { Id = 3, Name = "Jamal" }, // কোনো order নেই
};
var orders = new[]
{
    new { CustomerId = 1, Item = "Phone" },
    new { CustomerId = 1, Item = "Laptop" },
    new { CustomerId = 2, Item = "Tablet" },
};

// Method Syntax
var result = customers.GroupJoin(
    orders,
    c => c.Id,
    o => o.CustomerId,
    (c, os) => new
    {
        Customer   = c.Name,
        OrderCount = os.Count(),
        Orders     = os.Select(o => o.Item)
    }
);
// { Customer="Rahim", OrderCount=2 }
// { Customer="Karim", OrderCount=1 }
// { Customer="Jamal", OrderCount=0 } ← match নেই, তবুও আসে

// Query Syntax
var r2 = from c in customers
         join o in orders on c.Id equals o.CustomerId into g
         select new { c.Name, Orders = g };
```

---

## প্রশ্ন ০৯ — LINQ দিয়ে Full Outer Join কীভাবে করবেন?

LINQ-এ built-in Full Outer Join নেই। Left Join + Right-only Union দিয়ে তৈরি করতে হয়।

```csharp
// Left Join
var leftJoin = left.GroupJoin(right, l => l.Id, r => r.Id,
    (l, rs) => rs.DefaultIfEmpty()
               .Select(r => new { l.Id, Name = l.Name, Score = r?.Score })
).SelectMany(x => x);

// Right-only
var rightOnly = right
    .Where(r => !left.Any(l => l.Id == r.Id))
    .Select(r => new { r.Id, Name = (string?)null, Score = (int?)r.Score });

// Full Outer Join
var fullOuter = leftJoin.Union(rightOnly);
```

---

## প্রশ্ন ১০ — `Concat` এবং `Union`-এর পার্থক্য কী?

- **`Concat`** — দুটি sequence জোড়া দেয়, ডুপ্লিকেট **রাখে**, দ্রুত
- **`Union`** — unique element রিটার্ন করে, ডুপ্লিকেট **সরায়**, তুলনামূলক ধীর

```csharp
var a = new[] { 1, 2, 3 };
var b = new[] { 3, 4, 5 };

var concat = a.Concat(b); // [1, 2, 3, 3, 4, 5]
var union  = a.Union(b);  // [1, 2, 3, 4, 5]

// কখন কোনটি:
// Concat — ডুপ্লিকেট গ্রহণযোগ্য, performance দরকার
// Union  — uniqueness দরকার
```

---

## প্রশ্ন ১১ — Deferred Execution Query-এর বাস্তব উদাহরণ দিন।

```csharp
// বাস্তব উদাহরণ — Dynamic query composition
IQueryable<Product> query = _context.Products.AsQueryable();

if (!string.IsNullOrEmpty(searchTerm))
    query = query.Where(p => p.Name.Contains(searchTerm));

if (minPrice.HasValue)
    query = query.Where(p => p.Price >= minPrice.Value);

if (categoryId.HasValue)
    query = query.Where(p => p.CategoryId == categoryId.Value);

// এখন execute হয় — একটিমাত্র optimized SQL query
var products = await query.OrderBy(p => p.Name).ToListAsync();
```

---

## প্রশ্ন ১২ — LINQ দিয়ে Left Outer Join কীভাবে করবেন?

```csharp
// Method Syntax
var leftJoin = customers
    .GroupJoin(orders, c => c.Id, o => o.CustomerId,
        (c, os) => new { Customer = c, Orders = os })
    .SelectMany(
        x => x.Orders.DefaultIfEmpty(),
        (x, order) => new
        {
            CustomerName = x.Customer.Name,
            OrderAmount  = order?.Amount ?? 0
        });

// Query Syntax
var leftJoin2 = from c in customers
                join o in orders on c.Id equals o.CustomerId into g
                from order in g.DefaultIfEmpty()
                select new
                {
                    CustomerName = c.Name,
                    OrderAmount  = order?.Amount ?? 0
                };
```

---

## প্রশ্ন ১৩ — `ToList()` এবং `ToArray()`-এর কার্যকর পার্থক্য কী?

**`ToList()`** — `List<T>`, Dynamic size, `Add()`/`Remove()` সম্ভব  
**`ToArray()`** — `T[]`, Fixed size, সামান্য কম মেমোরি

```csharp
var query = students.Where(s => s.IsActive);

var list  = query.ToList();          // mutable
list.Add(new Student());             // ✅

var array = query.ToArray();         // fixed
// array.Add(...) ❌

// অন্য option
var set  = query.Select(s => s.Name).ToHashSet();
var dict = students.ToDictionary(s => s.Id);
```

---

## প্রশ্ন ১৪ — LINQ-এ Case-Insensitive Search কীভাবে করবেন?

```csharp
string search = "karim";

// StringComparison (সর্বোত্তম)
var r1 = names.Where(n => n.Equals(search, StringComparison.OrdinalIgnoreCase));

// Contains — substring
var r2 = names.Where(n => n.Contains(search, StringComparison.OrdinalIgnoreCase));

// EF Core-এ
var r3 = await _context.Users
    .Where(u => u.Name.ToLower() == search.ToLower())
    .ToListAsync();

// EF.Functions.Like
var r4 = await _context.Users
    .Where(u => EF.Functions.Like(u.Name, $"%{search}%"))
    .ToListAsync();
```

---

## প্রশ্ন ১৫ — LINQ-এ `Intersect` operator কীভাবে ব্যবহার করবেন?

```csharp
var a = new[] { 1, 2, 3, 4, 5 };
var b = new[] { 3, 4, 5, 6, 7 };

var common = a.Intersect(b); // [3, 4, 5]

// Case-insensitive intersect
var common2 = cities1.Intersect(cities2, StringComparer.OrdinalIgnoreCase);

// .NET 6+ IntersectBy
var byId = listA.IntersectBy(listB.Select(x => x.Id), x => x.Id);

// বাস্তব ব্যবহার — common permission
var commonPerms = user1.Permissions.Intersect(user2.Permissions);
```

---

## প্রশ্ন ১৬ — `Intersect` এবং `Except`-এর পার্থক্য কী?

- **`Intersect`** — উভয় collection-এ **আছে** এমন element (A ∩ B)
- **`Except`** — প্রথমে **আছে কিন্তু দ্বিতীয়তে নেই** এমন element (A − B)

```csharp
var a = new[] { 1, 2, 3, 4, 5 };
var b = new[] { 3, 4, 5, 6, 7 };

var intersect = a.Intersect(b); // [3, 4, 5]
var exceptAB  = a.Except(b);    // [1, 2]
var exceptBA  = b.Except(a);    // [6, 7]

// .NET 6+ key-based
var newUsers = allUsers.ExceptBy(
    existingUsers.Select(u => u.Email), u => u.Email);
```

---

## প্রশ্ন ১৭ — Database Context-এ LINQ Query কীভাবে Execute করবেন?

```csharp
public class ProductService
{
    private readonly AppDbContext _context;

    // Async query
    public async Task<List<Product>> GetActiveAsync()
        => await _context.Products.Where(p => p.IsActive).ToListAsync();

    // Projection
    public async Task<List<ProductDto>> GetSummaryAsync()
        => await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
            .ToListAsync();

    // Eager Loading
    public async Task<List<Order>> GetWithCustomerAsync()
        => await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ToListAsync();

    // Pagination
    public async Task<List<Product>> GetPagedAsync(int page, int size)
        => await _context.Products
            .OrderBy(p => p.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
}
```

---

## প্রশ্ন ১৮ — LINQ-এ Complex Data Types কীভাবে Handle করবেন?

```csharp
var departments = GetDepartments();

// Nested property filter
var dhakaDepts = departments.Where(d => d.Location.City == "Dhaka");

// Nested collection filter
var withSenior = departments
    .Where(d => d.Employees.Any(e => e.Years > 5));

// Nested aggregation
var stats = departments.Select(d => new
{
    Name      = d.Name,
    HeadCount = d.Employees.Count,
    AvgSalary = d.Employees.Average(e => e.Salary),
    City      = d.Location.City
});

// Deeply nested flatten
var allSkills = departments
    .SelectMany(d => d.Employees)
    .SelectMany(e => e.Skills)
    .Distinct();
```

---

## প্রশ্ন ১৯ — LINQ দিয়ে Filtering এবং Pagination কীভাবে করবেন?

```csharp
public async Task<PagedResult<ProductDto>> GetProductsAsync(
    string? search, decimal? minPrice, int page, int pageSize)
{
    var query = _context.Products.AsQueryable();

    if (!string.IsNullOrEmpty(search))
        query = query.Where(p => p.Name.Contains(search));

    if (minPrice.HasValue)
        query = query.Where(p => p.Price >= minPrice.Value);

    int total = await query.CountAsync();

    var items = await query
        .OrderBy(p => p.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
        .ToListAsync();

    return new PagedResult<ProductDto>
    {
        Items      = items,
        TotalCount = total,
        TotalPages = (int)Math.Ceiling(total / (double)pageSize)
    };
}
```

---

## প্রশ্ন ২০ — `DefaultIfEmpty` method কীভাবে কাজ করে?

```csharp
var numbers = new[] { 1, 2, 3 };
var empty   = new int[] { };

var r1 = numbers.DefaultIfEmpty(); // [1, 2, 3]
var r2 = empty.DefaultIfEmpty();   // [0] — int default
var r3 = empty.DefaultIfEmpty(-1); // [-1] — কাস্টম default

// Left Outer Join-এ ব্যবহার
var result = from c in customers
             join o in orders on c.Id equals o.CustomerId into g
             from order in g.DefaultIfEmpty()
             select new
             {
                 Customer = c.Name,
                 Amount   = order?.Amount ?? 0
             };
```

---

## প্রশ্ন ২১ — `Any()` এবং `Contains()`-এর পার্থক্য কী?

**`Any(predicate)`** — Custom condition দিয়ে check, complex predicate সমর্থন
**`Contains(value)`** — নির্দিষ্ট মান আছে কিনা check, EF Core-এ SQL `IN` তে translate হয়

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// Any — custom condition
bool hasEven = numbers.Any(n => n % 2 == 0); // true

// Contains — exact value
bool has3    = numbers.Contains(3);           // true

// EF Core-এ IN query
var ids   = new[] { 1, 3, 5 };
var users = await _context.Users
    .Where(u => ids.Contains(u.Id))
    .ToListAsync();
// SQL: WHERE Id IN (1, 3, 5)

// Any — more powerful
bool hasPremium = users.Any(u => u.Plan == "Premium" && u.IsActive);
```

---

## প্রশ্ন ২২ — LINQ-এ `ToDictionary` কীসের জন্য ব্যবহার হয়?

```csharp
var students = GetStudents();

// Id → Student mapping
var byId = students.ToDictionary(s => s.Id);
// O(1) lookup: byId[5] → Student object

// Id → Name mapping
var idToName = students.ToDictionary(s => s.Id, s => s.Name);

// Grouped dictionary
var byGrade = students
    .GroupBy(s => s.Grade)
    .ToDictionary(g => g.Key, g => g.ToList());
// byGrade["A"] → List<Student>

// Duplicate key হলে Exception — safe version:
var safe = students
    .GroupBy(s => s.Email)
    .ToDictionary(g => g.Key, g => g.First());

// Config/Lookup table
var configMap = configs.ToDictionary(c => c.Key, c => c.Value);
string val = configMap["DatabaseUrl"]; // O(1) lookup
```

---

## প্রশ্ন ২৩ — LINQ Query-এর Performance কীভাবে উন্নত করবেন?

**১. Projection — শুধু দরকারী column আনুন**
```csharp
// ❌
var all = await _context.Users.ToListAsync();
// ✅
var dto = await _context.Users.Select(u => new { u.Id, u.Name }).ToListAsync();
```
**২. Filter আগে, project পরে**
```csharp
var r = _context.Products.Where(p => p.IsActive).Select(p => new { p.Name });
```
**৩. Async ব্যবহার করুন**
```csharp
var list = await _context.Users.ToListAsync();
```
**৪. Any() বনাম Count()**
```csharp
if (users.Any()) { }        // ✅ O(1)
if (users.Count() > 0) { } // ❌ O(N)
```
**৫. AsNoTracking — Read-only query-তে**
```csharp
var products = await _context.Products.AsNoTracking().ToListAsync();
```
**৬. Multiple enumeration এড়ান**
```csharp
var cached = query.ToList(); // একবার cache করুন
```

---

## প্রশ্ন ২৪ — LINQ দিয়ে Efficient Pagination কীভাবে Implement করবেন?

```csharp
// ১. Offset-based Pagination
var paged = await _context.Products
    .OrderBy(p => p.Id)          // Order ছাড়া unstable!
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// ২. Count + Items একসাথে (parallel)
var totalTask = _context.Products.CountAsync();
var itemsTask = _context.Products
    .OrderBy(p => p.Id)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
await Task.WhenAll(totalTask, itemsTask);

// ৩. Cursor-based — বড় dataset-এ দক্ষ
var afterId = 150; // শেষ দেখা item-এর Id
var next10  = await _context.Products
    .Where(p => p.Id > afterId)  // WHERE Id > 150 — index ব্যবহার করে
    .OrderBy(p => p.Id)
    .Take(10)
    .ToListAsync();
// Offset: SKIP 10000 Take 10 — ধীর (10000 row scan)
// Cursor: WHERE Id > lastId — দ্রুত O(log n)
```

---

## প্রশ্ন ২৫ — `ElementAtOrDefault` method-এর উদ্দেশ্য কী?

```csharp
var fruits = new[] { "Apple", "Banana", "Cherry" };

var b  = fruits.ElementAt(1);           // "Banana"
// fruits.ElementAt(10)                 // ❌ Exception!

var b2 = fruits.ElementAtOrDefault(1);  // "Banana"
var x  = fruits.ElementAtOrDefault(10); // null (safe)
var n  = new int[]{1,2}.ElementAtOrDefault(5); // 0

// .NET 8+ Index syntax
var last   = fruits.ElementAt(^1);      // "Cherry"
var second = fruits.ElementAt(^2);      // "Banana"

// Safe dynamic index
int idx     = GetUserSelectedIndex();
var selected = items.ElementAtOrDefault(idx) ?? defaultItem;
```

---

## প্রশ্ন ২৬ — Empty Collection-এ `First()` বনাম `FirstOrDefault()` কীভাবে আলাদা আচরণ করে?

```csharp
var empty = new List<Student>();
var nums  = new[] { 1, 2, 3 };

// First() — Exception দেয়
// empty.First();               // ❌ InvalidOperationException!
// nums.First(n => n > 10);    // ❌ No match — Exception!

// FirstOrDefault() — safe
Student? s   = empty.FirstOrDefault();              // null
int f        = nums.FirstOrDefault(n => n > 10);    // 0
int f2       = nums.FirstOrDefault(n => n > 10, -1);// -1 (.NET 6+)

// সঠিক ব্যবহার:
// First()          — নিশ্চিত যে element আছে
// FirstOrDefault() — অনিশ্চিত, null check করতে হবে
```

---

## প্রশ্ন ২৭ — LINQ দিয়ে Custom Sorting কীভাবে Implement করবেন?

```csharp
// ১. Dynamic sort field
string sortField = "Salary";
bool   ascending = true;

IOrderedEnumerable<Employee> result = sortField switch
{
    "Name"   => ascending
        ? employees.OrderBy(e => e.Name)
        : employees.OrderByDescending(e => e.Name),
    "Salary" => ascending
        ? employees.OrderBy(e => e.Salary)
        : employees.OrderByDescending(e => e.Salary),
    _        => employees.OrderBy(e => e.Id)
};

// ২. Priority-based sort
var priority = employees
    .OrderBy(e => e.Title == "Manager" ? 0 : 1)
    .ThenByDescending(e => e.Salary);

// ৩. IComparer দিয়ে
public class SalaryComparer : IComparer<Employee>
{
    public int Compare(Employee? x, Employee? y)
        => (x?.Salary ?? 0).CompareTo(y?.Salary ?? 0);
}
var sorted = employees.OrderBy(e => e, new SalaryComparer());
```

---

## প্রশ্ন ২৮ — Complex Data Types সহ Object List-এ LINQ কীভাবে ব্যবহার করবেন?

```csharp
var orders = GetOrders();

// Nested property filter
var dhaka = orders.Where(o => o.Customer.Address.City == "Dhaka");

// Nested aggregation
var bigOrders = orders
    .Where(o => o.Items.Sum(i => i.Quantity * i.Price) > 5000);

// Complex projection
var summary = orders
    .Where(o => o.Status == OrderStatus.Completed)
    .Select(o => new
    {
        Customer      = o.Customer.Name,
        ItemCount     = o.Items.Count,
        Total         = o.Items.Sum(i => i.Quantity * i.Price),
        MostExpensive = o.Items.MaxBy(i => i.Price)?.Name
    });

// Group by nested property
var byCity = orders
    .GroupBy(o => o.Customer.Address.City)
    .Select(g => new
    {
        City    = g.Key,
        Revenue = g.SelectMany(o => o.Items).Sum(i => i.Quantity * i.Price)
    });
```

---

## প্রশ্ন ২৯ — LINQ এবং Raw SQL-এর trade-offs কী?

**LINQ-এর সুবিধা**
- Type-safe, compile-time error
- SQL Injection নিরাপদ
- Readable, maintainable
- Cross-database compatible

**LINQ-এর অসুবিধা**
- Complex query-তে inefficient SQL হতে পারে
- Window function, CTE সরাসরি নেই

**Raw SQL-এর সুবিধা**
- Full SQL control, optimize করা যায়
- Window function, CTE, pivot সমর্থন

**Raw SQL-এর অসুবিধা**
- Type-unsafe, SQL Injection ঝুঁকি
- Refactoring কঠিন

```csharp
// Hybrid — প্রয়োজনে Raw SQL
var result = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Stock < 10")
    .ToListAsync();
```

---

## প্রশ্ন ৩০ — `OrderBy` এবং `OrderByDescending` কখন কোনটি ব্যবহার করবেন?

**`OrderBy`** — Ascending: সস্তা product, alphabetical list, পুরনো history  
**`OrderByDescending`** — Descending: latest news, highest score, most recent order

```csharp
var cheap   = products.OrderBy(p => p.Price);           // সস্তা আগে
var latest  = orders.OrderByDescending(o => o.CreatedAt);// নতুন আগে

// Conditional
bool isAsc  = userPreference == "asc";
var dynamic = isAsc
    ? products.OrderBy(p => p.Price)
    : products.OrderByDescending(p => p.Price);

// Leaderboard
var top = scores
    .OrderByDescending(s => s.Points)
    .ThenBy(s => s.TimeTakenSeconds)
    .ThenBy(s => s.PlayerName);
```

---

## প্রশ্ন ৩১ — LINQ to Entities-এ Inner Join কীভাবে করবেন?

```csharp
// Method Syntax
var result = await _context.Orders
    .Join(_context.Customers,
        o => o.CustomerId, c => c.Id,
        (o, c) => new { o.Id, CustomerName = c.Name, o.Amount })
    .Where(x => x.Amount > 1000)
    .ToListAsync();

// Query Syntax
var result2 = await (
    from o in _context.Orders
    join c in _context.Customers on o.CustomerId equals c.Id
    where o.Amount > 1000
    select new { o.Id, c.Name, o.Amount }
).ToListAsync();

// সর্বোত্তম — Navigation Property
var best = await _context.Orders
    .Include(o => o.Customer)
    .Where(o => o.Amount > 1000)
    .ToListAsync();
```

---

## প্রশ্ন ৩২ — `IEnumerable` এবং `IQueryable`-এর সুবিধা কী?

**`IEnumerable<T>`**
- In-memory collection-এ কাজ করে, C# লুপ চালায়
- সব C# lambda সমর্থন করে
- প্রথমে data আনে, তারপর filter

**`IQueryable<T>`**
- Database query translate করে (server-side)
- শুধু প্রয়োজনীয় SQL পাঠায়

```csharp
// IEnumerable — আগে সব আনে, তারপর filter! ❌
IEnumerable<User> users = _context.Users.ToList();
var filtered = users.Where(u => u.IsActive);

// IQueryable — server-এ filter, কম data আসে ✅
IQueryable<User> query = _context.Users.Where(u => u.IsActive);
var list = await query.ToListAsync();

// ⚠️ AsEnumerable() সব data আনে!
var risk = _context.Users.AsEnumerable().Where(u => u.IsActive); // ❌ সব আসে
```

---

## প্রশ্ন ৩৩ — LINQ দিয়ে `IN` Query কীভাবে করবেন?

```csharp
// Basic IN
var ids   = new[] { 1, 3, 5, 7 };
var users = await _context.Users
    .Where(u => ids.Contains(u.Id))
    .ToListAsync();
// SQL: WHERE Id IN (1, 3, 5, 7)

// String IN
var roles     = new[] { "Admin", "Manager" };
var privileged = await _context.Users
    .Where(u => roles.Contains(u.Role))
    .ToListAsync();

// NOT IN
var blocked = await _context.Users
    .Where(u => !ids.Contains(u.Id))
    .ToListAsync();
// SQL: WHERE Id NOT IN (1, 3, 5, 7)

// ⚠️ List খুব বড় হলে performance সমস্যা — তখন join ব্যবহার করুন
```

---

## প্রশ্ন ৩৪ — Large Dataset-এ LINQ-এর Performance সমস্যা কীভাবে সমাধান করবেন?

```csharp
// ১. শুধু দরকারী column আনুন
var minimal = await _context.Products
    .Select(p => new { p.Id, p.Name }).ToListAsync();

// ২. Database Index নিশ্চিত করুন
modelBuilder.Entity<Product>().HasIndex(p => p.CategoryId);

// ৩. Pagination ব্যবহার করুন
var page = await _context.Products
    .OrderBy(p => p.Id).Skip(offset).Take(pageSize).ToListAsync();

// ৪. AsNoTracking — Read-only-এ
var products = await _context.Products.AsNoTracking().ToListAsync();

// ৫. Chunking — বড় batch
await foreach (var chunk in _context.Products.AsAsyncEnumerable().Chunk(1000))
    ProcessChunk(chunk);

// ৬. Compiled Query
private static readonly Func<AppDbContext, int, Task<Product?>> FindById =
    EF.CompileAsyncQuery((AppDbContext db, int id) =>
        db.Products.FirstOrDefault(p => p.Id == id));
```

---

## প্রশ্ন ৩৫ — `AsEnumerable()` method LINQ-এ কীসের জন্য?

`AsEnumerable()` একটি `IQueryable<T>` কে `IEnumerable<T>`-তে রূপান্তর করে।  
এর পরে LINQ **client-side (C#)** execute হয়।

```csharp
// DB-তে translate হয় না এমন method ব্যবহার করতে
var result = _context.Users
    .Where(u => u.IsActive)         // Server-side (SQL)
    .AsEnumerable()                  // এখন client-side
    .Where(u => MyCustomMethod(u.Name)); // Custom C# method

// ✅ সঠিক — filter আগে, AsEnumerable পরে
var good = _context.Users
    .Where(u => u.IsActive)   // Server-এ filter
    .AsEnumerable()
    .Select(u => Format(u));  // Custom method

// ❌ ভুল — সব row client-এ আনে
var bad = _context.Users.AsEnumerable().Where(u => u.IsActive);
```

---

## প্রশ্ন ৩৬ — LINQ-এ Async Method ব্যবহার করা যায়?

হ্যাঁ! EF Core async LINQ extension method সমর্থন করে।

```csharp
// Basic async
var users  = await _context.Users.ToListAsync();
var first  = await _context.Users.FirstOrDefaultAsync(u => u.Id == 1);
var count  = await _context.Users.CountAsync();
var any    = await _context.Users.AnyAsync(u => u.IsActive);

// Async + Filter
var active = await _context.Users
    .Where(u => u.IsActive).OrderBy(u => u.Name).ToListAsync();

// Async stream
await foreach (var product in _context.Products.AsAsyncEnumerable())
    await ProcessAsync(product);

// Parallel async
var t1 = _context.Users.CountAsync();
var t2 = _context.Orders.CountAsync();
await Task.WhenAll(t1, t2);

// ⚠️ LINQ to Objects-এ ToListAsync() নেই!
var list = new List<int> { 1, 2, 3 };
var r    = list.Where(n => n > 1).ToList(); // sync ব্যবহার করুন
```

---

## প্রশ্ন ৩৭ — Generic Object Collection-এ LINQ কীভাবে ব্যবহার করবেন?

```csharp
// Generic filter method
public IEnumerable<T> Filter<T>(IEnumerable<T> source, Func<T, bool> predicate)
    => source.Where(predicate);

// Type constraint সহ
public IEnumerable<T> GetActive<T>(IEnumerable<T> source)
    where T : class, IActivatable
    => source.Where(x => x.IsActive);

// Expression Tree — IQueryable-এর জন্য
public IQueryable<T> ApplyFilter<T>(
    IQueryable<T> query,
    Expression<Func<T, bool>> filter)
    => query.Where(filter);

// ব্যবহার
var result = ApplyFilter(_context.Users, u => u.Age > 18);

// Reflection দিয়ে dynamic sort
public IEnumerable<T> SortBy<T>(IEnumerable<T> source, string propertyName)
{
    var prop = typeof(T).GetProperty(propertyName)
        ?? throw new ArgumentException($"Property not found: {propertyName}");
    return source.OrderBy(x => prop.GetValue(x));
}
```

---

## প্রশ্ন ৩৮ — Anonymous Methods কী এবং LINQ-এ কীভাবে ব্যবহার হয়?

**Anonymous Method** — নাম ছাড়া inline method, `delegate` বা lambda হিসেবে লেখা যায়।

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// delegate keyword (পুরনো, C# 2.0+)
var evens1 = numbers.Where(delegate(int n) { return n % 2 == 0; });

// Lambda (আধুনিক, C# 3.0+)
var evens2 = numbers.Where(n => n % 2 == 0);

// Multi-line lambda
var result = numbers.Where(n =>
{
    bool isEven  = n % 2 == 0;
    bool isGreat = n > 2;
    return isEven && isGreat;
});

// Func variable
Func<int, bool> isEven = n => n % 2 == 0;
var evens3 = numbers.Where(isEven);

// Closure — বাইরের variable ব্যবহার
int threshold = 3;
var above = numbers.Where(n => n > threshold); // deferred! threshold পরিবর্তন হলে affect হয়
```

---

## প্রশ্ন ৩৯ — LINQ Query-তে Error এবং Exception কীভাবে Handle করবেন?

```csharp
// ১. OrDefault দিয়ে null check
var user = users.FirstOrDefault(u => u.Id == 999);
if (user is null) return NotFound();

// ২. Try-Catch (EF Core)
try
{
    var data = await _context.Users.Where(u => u.IsActive).ToListAsync();
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Database error");
    throw;
}
catch (OperationCanceledException)
{
    _logger.LogWarning("Query cancelled");
    return null;
}

// ৩. Null-safe chaining
var city = user?.Address?.City ?? "অজানা";

// ৪. DefaultIfEmpty দিয়ে empty handle
var result = products.Where(p => p.Category == "X")
    .DefaultIfEmpty()
    .Select(p => p?.Name ?? "N/A");

// ৫. Input validation আগে করুন
if (ids is null || !ids.Any()) return Enumerable.Empty<User>();
```

---

## প্রশ্ন ৪০ — বাস্তব Application-এ Complex Filtering-এর জন্য LINQ কীভাবে Implement করবেন?

```csharp
// Builder Pattern দিয়ে Dynamic Filter
public class ProductFilterBuilder
{
    private IQueryable<Product> _query;

    public ProductFilterBuilder(IQueryable<Product> query)
        => _query = query;

    public ProductFilterBuilder WithSearch(string? term)
    {
        if (!string.IsNullOrWhiteSpace(term))
            _query = _query.Where(p =>
                p.Name.Contains(term) || p.Description.Contains(term));
        return this;
    }

    public ProductFilterBuilder WithPriceRange(decimal? min, decimal? max)
    {
        if (min.HasValue) _query = _query.Where(p => p.Price >= min.Value);
        if (max.HasValue) _query = _query.Where(p => p.Price <= max.Value);
        return this;
    }

    public ProductFilterBuilder WithCategory(int? id)
    {
        if (id.HasValue)
            _query = _query.Where(p => p.CategoryId == id.Value);
        return this;
    }

    public IQueryable<Product> Build() => _query;
}

// ব্যবহার
var products = await new ProductFilterBuilder(_context.Products)
    .WithSearch(request.Search)
    .WithPriceRange(request.MinPrice, request.MaxPrice)
    .WithCategory(request.CategoryId)
    .Build()
    .OrderBy(p => p.Name)
    .Skip((request.Page - 1) * request.PageSize)
    .Take(request.PageSize)
    .AsNoTracking()
    .ToListAsync();
```

---

> **সারসংক্ষেপ:**  
> Intermediate LINQ-এর মূল বিষয়: Projection, Join (Inner/Left/Full Outer), Set Operations, Custom Aggregation, Deferred Execution-এর চতুর ব্যবহার, EF Core-এ IQueryable অপ্টিমাইজেশন এবং Builder Pattern দিয়ে Complex Filtering।

---
*সর্বশেষ আপডেট: .NET 9 | C# 13 | 2026*
