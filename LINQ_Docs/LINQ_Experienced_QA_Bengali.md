# LINQ ইন্টারভিউ প্রশ্ন ও উত্তর — Experienced Level (বাংলায়)

> C# LINQ সংক্রান্ত ৪০টি Experienced/Advanced প্রশ্ন ও বিস্তারিত উত্তর | .NET 9 | EF Core | C# 13

---

## প্রশ্ন ০১ — SQL Server-এর মতো Relational Database-এ LINQ কীভাবে ব্যবহার করবেন?

LINQ with EF Core একটি **Provider Model** ব্যবহার করে — LINQ expression tree SQL-এ translate হয়।

```csharp
// ১. DbContext Setup
public class AppDbContext : DbContext
{
    public DbSet<User>    Users    { get; set; }
    public DbSet<Order>   Orders   { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
        mb.Entity<Order>().HasIndex(o => o.CreatedAt);
        mb.Entity<Product>().HasIndex(p => new { p.CategoryId, p.Price });
    }
}

// ২. Repository Pattern
public class UserRepository
{
    private readonly AppDbContext _ctx;

    public async Task<PagedList<User>> GetPagedAsync(UserFilter filter)
    {
        var query = _ctx.Users.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(u => u.Name.Contains(filter.Search));

        if (filter.IsActive.HasValue)
            query = query.Where(u => u.IsActive == filter.IsActive.Value);

        int total = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PagedList<User>(items, total, filter.Page, filter.PageSize);
    }
}
```

---

## প্রশ্ন ০২ — LINQ to SQL এবং LINQ to Entities-এ Deferred Execution কীভাবে কাজ করে?

**LINQ to SQL / EF Core-এ** deferred execution একটি **Expression Tree** তৈরি করে।  
`ToListAsync()` বা `FirstAsync()` call না করা পর্যন্ত কোনো SQL পাঠানো হয় না।

```csharp
// Expression Tree তৈরি হচ্ছে (কোনো SQL নেই)
IQueryable<Product> q = _ctx.Products;
q = q.Where(p => p.IsActive);     // WHERE IsActive = 1 যোগ হলো
q = q.Where(p => p.Price > 100);  // AND Price > 100 যোগ হলো
q = q.OrderBy(p => p.Name);       // ORDER BY Name যোগ হলো

// এখন SQL execute হয়
var result = await q.ToListAsync();
// SQL: SELECT * FROM Products WHERE IsActive=1 AND Price>100 ORDER BY Name

// Deferred + Compose — dynamic query
var query = _ctx.Users.AsQueryable();
if (onlyActive) query = query.Where(u => u.IsActive);
if (minAge > 0)  query = query.Where(u => u.Age >= minAge);
// একটিই SQL যাবে, condition অনুযায়ী WHERE clause তৈরি হবে
var users = await query.ToListAsync();
```

> **IEnumerable-এ deferred execution** C# এ হয়।  
> **IQueryable-এ deferred execution** SQL Server-এ হয়।

---

## প্রশ্ন ০৩ — Efficient LINQ Query লেখার Best Practices কী?

**১. Projection — শুধু দরকারী column**
```csharp
// ❌ সব column আনে
var users = await _ctx.Users.ToListAsync();

// ✅ শুধু দরকারী
var dtos = await _ctx.Users
    .Select(u => new UserDto { Id = u.Id, Name = u.Name })
    .ToListAsync();
```

**২. Filter আগে, Project পরে**
```csharp
// ✅
var result = _ctx.Products
    .Where(p => p.IsActive && p.Price > 100) // প্রথমে filter
    .Select(p => new { p.Name, p.Price });   // তারপর project
```

**৩. AsNoTracking — Read-only query-তে**
```csharp
var products = await _ctx.Products.AsNoTracking().ToListAsync();
```

**৪. Async সবসময়**
```csharp
var list = await _ctx.Users.Where(u => u.IsActive).ToListAsync();
```

**৫. Index নিশ্চিত করুন**
```csharp
modelBuilder.Entity<Product>().HasIndex(p => p.CategoryId);
```

**৬. Compiled Query — Hot path-এ**
```csharp
private static readonly Func<AppDbContext, int, Task<User?>> FindById =
    EF.CompileAsyncQuery((AppDbContext db, int id) =>
        db.Users.FirstOrDefault(u => u.Id == id));
```

**৭. SplitQuery — বড় Include-এ**
```csharp
var orders = await _ctx.Orders
    .Include(o => o.Items)
    .AsSplitQuery()    // একটির বদলে দুটি SQL
    .ToListAsync();
```

---

## প্রশ্ন ০৪ — LINQ দিয়ে Cross-Database Join কীভাবে করবেন?

EF Core সরাসরি cross-database join সমর্থন করে না। নিচের পদ্ধতিগুলো ব্যবহার করা হয়:

```csharp
// পদ্ধতি ১: In-Memory Join — উভয় DB থেকে আলাদা আলাদা আনো, তারপর join
var usersFromDb1  = await _ctx1.Users.ToListAsync();
var ordersFromDb2 = await _ctx2.Orders.ToListAsync();

var combined = usersFromDb1.Join(
    ordersFromDb2,
    u => u.Id,
    o => o.UserId,
    (u, o) => new { u.Name, o.Amount }
);

// পদ্ধতি ২: Linked Server (SQL Server) — Raw SQL
var result = await _ctx.Database
    .SqlQueryRaw<OrderSummary>(@"
        SELECT u.Name, o.Amount
        FROM [DB1].[dbo].[Users] u
        JOIN [DB2].[dbo].[Orders] o ON u.Id = o.UserId
    ").ToListAsync();

// পদ্ধতি ৩: View বা Stored Procedure দিয়ে abstraction
var result2 = await _ctx.Set<CrossDbView>()
    .FromSqlRaw("SELECT * FROM vw_CrossDbUserOrders")
    .ToListAsync();
```

---

## প্রশ্ন ০৫ — LINQ-এ `IEnumerable` এবং `IQueryable`-এর পার্থক্য কী?

| বিষয় | `IEnumerable<T>` | `IQueryable<T>` |
|-------|-----------------|-----------------|
| Execution | Client-side (C#) | Server-side (SQL) |
| Data loading | আগে সব আনে | Filter করে তারপর আনে |
| Lambda support | সব C# lambda | DB-translatable lambda |
| Performance | বড় data-তে ধীর | বড় data-তে দ্রুত |

```csharp
// IEnumerable — সব row আনে তারপর C#-এ filter!
IEnumerable<User> e = _ctx.Users; // SELECT * FROM Users (সব!)
var r1 = e.Where(u => u.IsActive); // C#-এ filter

// IQueryable — server-এ filter
IQueryable<User> q = _ctx.Users;
var r2 = q.Where(u => u.IsActive); // SQL: WHERE IsActive=1

// Critical difference:
var badQuery = _ctx.Users
    .AsEnumerable()              // সব row client-এ!
    .Where(u => u.IsActive);     // C#-এ filter

var goodQuery = _ctx.Users
    .Where(u => u.IsActive)      // SQL-এ filter
    .AsEnumerable();             // তারপর client-এ
```

---

## প্রশ্ন ০৬ — Query Execution-এর সময় Entity পরিবর্তন হলে LINQ to SQL কীভাবে Handle করে?

EF Core **Change Tracker** ব্যবহার করে entity-র state track করে।

```csharp
// Change Tracker States:
// Added, Modified, Deleted, Unchanged, Detached

using var ctx = new AppDbContext();

// ১. Query করলে Unchanged state-এ থাকে
var user = await ctx.Users.FindAsync(1);
// State: Unchanged

// ২. Property পরিবর্তন করলে Modified হয়
user.Name = "নতুন নাম";
// State: Modified

// ৩. SaveChanges — শুধু Modified entity-র UPDATE যায়
await ctx.SaveChangesAsync();
// SQL: UPDATE Users SET Name='নতুন নাম' WHERE Id=1

// ৪. Change Tracker কে bypass করতে AsNoTracking
var readOnly = await ctx.Users.AsNoTracking().ToListAsync();
// পরিবর্তন track হবে না — Read-only

// ৫. Explicit state setting
ctx.Entry(user).State = EntityState.Modified;
```

---

## প্রশ্ন ০৭ — Entity Framework Core-এ LINQ কীভাবে ব্যবহার করবেন?

```csharp
// ১. Basic CRUD
public class ProductService
{
    private readonly AppDbContext _ctx;

    // READ
    public async Task<List<ProductDto>> GetAllAsync()
        => await _ctx.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
            .AsNoTracking()
            .ToListAsync();

    // CREATE
    public async Task<int> CreateAsync(CreateProductDto dto)
    {
        var product = new Product { Name = dto.Name, Price = dto.Price };
        _ctx.Products.Add(product);
        await _ctx.SaveChangesAsync();
        return product.Id;
    }

    // UPDATE
    public async Task UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _ctx.Products.FindAsync(id)
            ?? throw new NotFoundException($"Product {id} not found");
        product.Name  = dto.Name;
        product.Price = dto.Price;
        await _ctx.SaveChangesAsync();
    }

    // DELETE
    public async Task DeleteAsync(int id)
    {
        var product = await _ctx.Products.FindAsync(id)
            ?? throw new NotFoundException($"Product {id} not found");
        _ctx.Products.Remove(product);
        await _ctx.SaveChangesAsync();
    }
}
```

---

## প্রশ্ন ০৮ — LINQ-এর Multi-threading ও Parallel Programming-এ গুরুত্ব কী?

```csharp
// ১. PLINQ — Parallel LINQ (CPU-bound কাজে)
var numbers  = Enumerable.Range(1, 1_000_000).ToList();

// Sequential
var sequential = numbers.Where(n => IsPrime(n)).ToList();

// Parallel (multi-core ব্যবহার করে)
var parallel = numbers.AsParallel()
    .Where(n => IsPrime(n))
    .ToList();

// ২. Ordered parallel
var orderedParallel = numbers.AsParallel()
    .AsOrdered()                     // ক্রম বজায় রাখে
    .Where(n => IsPrime(n))
    .ToList();

// ৩. Degree of parallelism নিয়ন্ত্রণ
var controlled = numbers.AsParallel()
    .WithDegreeOfParallelism(4)      // সর্বোচ্চ ৪ thread
    .Where(n => IsPrime(n))
    .ToList();

// ⚠️ সতর্কতা:
// EF Core DbContext thread-safe নয়! Parallel-এ আলাদা DbContext ব্যবহার করুন
// PLINQ শুধু CPU-bound কাজে — I/O bound-এ async/await ব্যবহার করুন
```

---

## প্রশ্ন ০৯ — LINQ-এর `Join` method Complex Object-এ কীভাবে কাজ করে?

```csharp
// Complex multi-key join
var result = orders.Join(
    products,
    o => new { o.ProductId, o.WarehouseId }, // composite key
    p => new { ProductId = p.Id, p.WarehouseId },
    (o, p) => new
    {
        OrderId     = o.Id,
        ProductName = p.Name,
        Quantity    = o.Quantity,
        Total       = o.Quantity * p.Price
    }
);

// Nested object join
var employeeDetails = employees.Join(
    departments,
    e => e.DepartmentId,
    d => d.Id,
    (e, d) => new EmployeeDetailDto
    {
        Name           = $"{e.FirstName} {e.LastName}",
        Department     = d.Name,
        DeptLocation   = d.Address.City,
        TeamLeadName   = d.Employees.FirstOrDefault(x => x.IsTeamLead)?.Name
    }
);

// Triple join
var report = from o in orders
             join c in customers on o.CustomerId equals c.Id
             join p in products  on o.ProductId  equals p.Id
             select new
             {
                 Customer = c.Name,
                 Product  = p.Name,
                 Amount   = o.Quantity * p.Price
             };
```

---

## প্রশ্ন ১০ — Complex Data Structure-এ Custom Filtering কীভাবে Implement করবেন?

```csharp
// Specification Pattern
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public Specification<T> And(Specification<T> other)
        => new AndSpecification<T>(this, other);

    public Specification<T> Or(Specification<T> other)
        => new OrSpecification<T>(this, other);
}

public class ActiveUserSpec : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
        => u => u.IsActive && !u.IsDeleted;
}

public class PremiumUserSpec : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
        => u => u.Plan == "Premium" && u.ExpiresAt > DateTime.UtcNow;
}

// Composite Specification
var spec    = new ActiveUserSpec().And(new PremiumUserSpec());
var result  = await _ctx.Users
    .Where(spec.ToExpression())
    .ToListAsync();
```

---

## প্রশ্ন ১১ — Large Dataset বা Database Table-এ LINQ Query কীভাবে Optimize করবেন?

```csharp
// ১. Index-aware query
var result = await _ctx.Orders
    .Where(o => o.CustomerId == custId)   // Indexed column
    .Where(o => o.CreatedAt >= startDate) // Indexed column
    .Select(o => new { o.Id, o.Amount })  // Projection
    .AsNoTracking()
    .ToListAsync();

// ২. Streaming — একটা একটা করে process (মেমোরি বাঁচায়)
await foreach (var order in _ctx.Orders.AsAsyncEnumerable())
    await ProcessAsync(order);

// ৩. Chunking — batch processing
const int batchSize = 1000;
var skip = 0;
while (true)
{
    var batch = await _ctx.Orders
        .OrderBy(o => o.Id)
        .Skip(skip).Take(batchSize)
        .AsNoTracking()
        .ToListAsync();

    if (!batch.Any()) break;
    await ProcessBatchAsync(batch);
    skip += batchSize;
}

// ৪. Bulk operations (EFCore.BulkExtensions)
await _ctx.BulkInsertAsync(newOrders);
await _ctx.BulkUpdateAsync(updatedOrders);

// ৫. Raw SQL — complex query-তে
var report = await _ctx.Database
    .SqlQueryRaw<SalesReport>(@"
        SELECT c.Name, SUM(o.Amount) AS Total
        FROM Orders o
        JOIN Customers c ON o.CustomerId = c.Id
        WHERE o.CreatedAt >= @p0
        GROUP BY c.Name
        ORDER BY Total DESC
    ", startDate)
    .ToListAsync();
```

---

## প্রশ্ন ১২ — LINQ to SQL-এ Transactional Support কীভাবে Implement করবেন?

```csharp
// ১. SaveChanges — implicit transaction (একটি DbContext-এর সব change একটি transaction-এ)
public async Task TransferFundsAsync(int fromId, int toId, decimal amount)
{
    var from = await _ctx.Accounts.FindAsync(fromId)
        ?? throw new NotFoundException();
    var to   = await _ctx.Accounts.FindAsync(toId)
        ?? throw new NotFoundException();

    if (from.Balance < amount)
        throw new InsufficientFundsException();

    from.Balance -= amount;
    to.Balance   += amount;

    await _ctx.SaveChangesAsync(); // atomic — উভয়ই একসাথে commit/rollback
}

// ২. Explicit transaction — cross-entity complex operation
public async Task ComplexOperationAsync()
{
    await using var transaction = await _ctx.Database.BeginTransactionAsync();
    try
    {
        var order = new Order { Amount = 1000 };
        _ctx.Orders.Add(order);
        await _ctx.SaveChangesAsync();

        var invoice = new Invoice { OrderId = order.Id, Amount = order.Amount };
        _ctx.Invoices.Add(invoice);
        await _ctx.SaveChangesAsync();

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

// ৩. Isolation level নির্ধারণ
var opts = new TransactionOptions
{
    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
};
using var scope = new TransactionScope(TransactionScopeOption.Required, opts,
    TransactionScopeAsyncFlowOption.Enabled);
await _ctx.SaveChangesAsync();
scope.Complete();
```

---

## প্রশ্ন ১৩ — LINQ to Entities-এ Change Tracking কীভাবে কাজ করে?

```csharp
// ১. Change Tracker পর্যবেক্ষণ
var user = await _ctx.Users.FindAsync(1);

Console.WriteLine(_ctx.Entry(user).State); // Unchanged

user.Name = "পরিবর্তিত নাম";
Console.WriteLine(_ctx.Entry(user).State); // Modified

// ২. Original vs Current value
var entry = _ctx.Entry(user);
var orig  = entry.OriginalValues["Name"]; // "পুরনো নাম"
var curr  = entry.CurrentValues["Name"];  // "পরিবর্তিত নাম"

// ৩. Specific property track
bool nameChanged = entry.Property(u => u.Name).IsModified; // true

// ৪. Detach করা
_ctx.Entry(user).State = EntityState.Detached;

// ৫. Global change tracking report
var changes = _ctx.ChangeTracker.Entries()
    .Where(e => e.State != EntityState.Unchanged)
    .Select(e => new
    {
        Type  = e.Entity.GetType().Name,
        State = e.State.ToString()
    });

// ৬. AsNoTracking — tracking বন্ধ (Read-only)
var readOnly = await _ctx.Users.AsNoTracking().ToListAsync();

// ৭. QueryTrackingBehavior — global default
_ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
```

---

## প্রশ্ন ১৪ — Entity Framework-এ N+1 Query Problem কীভাবে প্রতিরোধ করবেন?

```csharp
// ❌ N+1 সমস্যা — প্রতিটি Order-এর Customer আলাদা query
var orders = await _ctx.Orders.ToListAsync();
foreach (var o in orders)
    Console.WriteLine(o.Customer.Name); // প্রতিটির জন্য SELECT Customer!

// ✅ সমাধান ১ — Eager Loading (Include)
var orders1 = await _ctx.Orders
    .Include(o => o.Customer)
    .Include(o => o.Items)
    .ThenInclude(i => i.Product)
    .ToListAsync(); // একটিই JOIN SQL

// ✅ সমাধান ২ — Projection (সবচেয়ে efficient)
var orders2 = await _ctx.Orders
    .Select(o => new
    {
        OrderId      = o.Id,
        CustomerName = o.Customer.Name,
        ItemCount    = o.Items.Count,
        Total        = o.Items.Sum(i => i.Quantity * i.Price)
    })
    .ToListAsync(); // একটিই SQL, সব data

// ✅ সমাধান ৩ — AsSplitQuery (বড় Include-এ)
var orders3 = await _ctx.Orders
    .Include(o => o.Items)
    .ThenInclude(i => i.Product)
    .AsSplitQuery()  // N টির বদলে কয়েকটি query — cartesian explosion এড়ায়
    .ToListAsync();

// ✅ সমাধান ৪ — Explicit Loading (প্রয়োজনে)
var order = await _ctx.Orders.FindAsync(1);
await _ctx.Entry(order).Reference(o => o.Customer).LoadAsync();
await _ctx.Entry(order).Collection(o => o.Items).LoadAsync();
```

---

## প্রশ্ন ১৫ — Complex Multi-field Sorting LINQ-এ কীভাবে Implement করবেন?

```csharp
// ১. Static multi-level sort
var sorted = employees
    .OrderBy(e => e.Department)
    .ThenByDescending(e => e.Salary)
    .ThenBy(e => e.LastName)
    .ThenBy(e => e.FirstName);

// ২. Dynamic sort — API-র sort parameter থেকে
public IQueryable<Employee> ApplySort(
    IQueryable<Employee> query,
    string? sortBy,
    bool ascending = true)
{
    return sortBy switch
    {
        "name"       => ascending ? query.OrderBy(e => e.Name)
                                  : query.OrderByDescending(e => e.Name),
        "salary"     => ascending ? query.OrderBy(e => e.Salary)
                                  : query.OrderByDescending(e => e.Salary),
        "department" => ascending ? query.OrderBy(e => e.Department)
                                  : query.OrderByDescending(e => e.Department),
        "joinDate"   => ascending ? query.OrderBy(e => e.JoinDate)
                                  : query.OrderByDescending(e => e.JoinDate),
        _            => query.OrderBy(e => e.Id)
    };
}

// ৩. Multiple sort fields — comma-separated
// "salary desc, name asc"
public IQueryable<T> ApplyMultiSort<T>(IQueryable<T> query, string sortString)
{
    var sorts = sortString.Split(',', StringSplitOptions.TrimEntries);
    IOrderedQueryable<T>? ordered = null;
    foreach (var sort in sorts)
    {
        var parts = sort.Split(' ');
        var field  = parts[0];
        var desc   = parts.Length > 1 && parts[1] == "desc";
        ordered = ordered is null
            ? query.OrderByDynamic(field, desc)
            : ordered.ThenByDynamic(field, desc);
    }
    return ordered ?? query;
}
```

---

## প্রশ্ন ১৬ — ASP.NET Core MVC-তে Dynamic Query-এর জন্য LINQ কীভাবে ব্যবহার করবেন?

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _ctx;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetProducts(
        [FromQuery] string? search,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string?  category,
        [FromQuery] string?  sortBy     = "name",
        [FromQuery] bool     ascending  = true,
        [FromQuery] int      page       = 1,
        [FromQuery] int      pageSize   = 20)
    {
        var query = _ctx.Products.AsQueryable();

        // Dynamic filtering
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p =>
                p.Name.Contains(search) || p.Description.Contains(search));

        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice);
        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        // Dynamic sorting
        query = sortBy switch
        {
            "price" => ascending
                ? query.OrderBy(p => p.Price)
                : query.OrderByDescending(p => p.Price),
            _ => ascending
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name)
        };

        int total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price })
            .AsNoTracking()
            .ToListAsync();

        return Ok(new PagedResponse<ProductDto>
        {
            Items      = items,
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        });
    }
}
```

---

## প্রশ্ন ১৭ — LINQ-এর `SelectMany`-এর Advanced Use Cases কী?

```csharp
// ১. Hierarchical data flatten
var allSubCategories = categories
    .SelectMany(c => c.SubCategories)
    .SelectMany(sc => sc.Products);

// ২. Permission system — role থেকে permissions
var userPerms = users.SelectMany(u => u.Roles)
    .SelectMany(r => r.Permissions)
    .Distinct();

// ৩. CSV-like flat file parse
var lines = File.ReadAllLines("data.csv");
var cells = lines.SelectMany(line => line.Split(','));

// ৪. Cartesian product (cross join)
var colors = new[] { "Red", "Blue" };
var sizes  = new[] { "S", "M", "L" };
var variants = colors.SelectMany(
    c => sizes,
    (color, size) => new { Color = color, Size = size }
);

// ৫. Graph traversal
var allPaths = nodes.SelectMany(n => n.Edges)
    .Where(e => e.Weight > 0);

// ৬. String manipulation
var sentences = new[] { "Hello World", "LINQ is powerful" };
var allWords  = sentences.SelectMany(s => s.Split(' '));
// ["Hello", "World", "LINQ", "is", "powerful"]
```

---

## প্রশ্ন ১৮ — Entity Framework-এ LINQ দিয়ে Lazy Loading কীভাবে করবেন?

```csharp
// ১. Lazy Loading setup
// Package: Microsoft.EntityFrameworkCore.Proxies
services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(connectionString)
     .UseLazyLoadingProxies()); // Lazy Loading enable

public class Order
{
    public int Id { get; set; }
    public virtual Customer Customer { get; set; } = null!; // virtual = lazy
    public virtual ICollection<OrderItem> Items { get; set; } = [];
}

// ব্যবহার — property access করলেই SQL যায়
var order = await _ctx.Orders.FindAsync(1);
var name  = order.Customer.Name; // এখানে SELECT Customer query যায়!

// ⚠️ Lazy Loading-এর সমস্যা — N+1
foreach (var o in orders)
    _ = o.Customer.Name; // প্রতিটির জন্য query!

// ✅ সর্বোত্তম — Explicit/Eager Loading ব্যবহার করুন
var orders = await _ctx.Orders
    .Include(o => o.Customer) // একটিই JOIN query
    .ToListAsync();

// Conditional Include
var orders2 = await _ctx.Orders
    .Include(o => o.Items.Where(i => i.IsActive)) // Filtered Include (.NET 5+)
    .ToListAsync();
```

---

## প্রশ্ন ১৯ — Stored Procedure LINQ to SQL বা Entity Framework-এ কীভাবে Execute করবেন?

```csharp
// ১. Simple stored procedure
var result = await _ctx.Database
    .SqlQueryRaw<SalesReport>("EXEC GetSalesByMonth @p0, @p1", month, year)
    .ToListAsync();

// ২. SqlParameter ব্যবহার করে (SQL Injection নিরাপদ)
var monthParam = new SqlParameter("@Month", month);
var yearParam  = new SqlParameter("@Year", year);
var result2 = await _ctx.SalesReports
    .FromSqlRaw("EXEC GetSalesByMonth @Month, @Year", monthParam, yearParam)
    .ToListAsync();

// ৩. Output parameter সহ
var countOut = new SqlParameter
{
    ParameterName = "@TotalCount",
    SqlDbType     = System.Data.SqlDbType.Int,
    Direction     = System.Data.ParameterDirection.Output
};
await _ctx.Database.ExecuteSqlRawAsync(
    "EXEC SearchProducts @SearchTerm, @TotalCount OUT",
    new SqlParameter("@SearchTerm", "Phone"), countOut
);
int total = (int)countOut.Value;

// ৪. EF Core Function Mapping
[DbFunction("fn_GetActiveUserCount", Schema = "dbo")]
public static int GetActiveUserCount(string role)
    => throw new NotImplementedException(); // EF Core-এ translate হয়

// ব্যবহার
var count = await _ctx.Users
    .Where(u => AppDbContext.GetActiveUserCount(u.Role) > 0)
    .CountAsync();
```

---

## প্রশ্ন ২০ — Multiple Entity নিয়ে LINQ Transaction Management কীভাবে করবেন?

```csharp
// ১. Unit of Work Pattern — একাধিক entity একই transaction-এ
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;

    public async Task<int> SaveAsync() => await _ctx.SaveChangesAsync();

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        var strategy = _ctx.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                await action();
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        });
    }
}

// ২. Retry strategy — transient failure handle
var strategy = _ctx.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    // connection timeout, deadlock — retry করবে
    var order = new Order { Amount = 1500 };
    _ctx.Orders.Add(order);
    await _ctx.SaveChangesAsync();
});

// ৩. Distributed transaction (multi-DbContext)
await using var tx = await ctx1.Database.BeginTransactionAsync();
ctx2.Database.UseTransaction(tx.GetDbTransaction());

_ctx1.Orders.Add(new Order { Amount = 1000 });
_ctx2.AuditLogs.Add(new AuditLog { Action = "OrderCreated" });

await ctx1.SaveChangesAsync();
await ctx2.SaveChangesAsync();
await tx.CommitAsync();
```

---

## প্রশ্ন ২১ — LINQ to SQL বনাম Entity Framework — Performance এবং Scalability পার্থক্য?

**LINQ to SQL**
- শুধু SQL Server সমর্থন করে
- Simple scenario-তে সামান্য দ্রুত (হালকা)
- Cross-DB, Migration, Seeding সমর্থন নেই
- Deprecated — নতুন project-এ ব্যবহার করবেন না

**Entity Framework Core**
- Multi-database সমর্থন (SQL Server, PostgreSQL, MySQL, SQLite)
- Compiled Query, AsNoTracking, AsSplitQuery — দক্ষ
- Query logging, Interceptor, Bulk operation সমর্থন
- Scalability-র জন্য: Sharding, Read replica সমর্থন

```csharp
// EF Core — Performance optimization
var result = await _ctx.Products
    .AsNoTracking()            // Change tracking বন্ধ
    .AsSplitQuery()            // Cartesian explosion এড়ায়
    .Where(p => p.IsActive)
    .Include(o => o.Category)
    .ToListAsync();

// Compiled query — hot path-এ
private static readonly Func<AppDbContext, int, Task<Product?>> GetById =
    EF.CompileAsyncQuery((AppDbContext ctx, int id) =>
        ctx.Products.FirstOrDefault(p => p.Id == id));
```

---

## প্রশ্ন ২২ — Web Application-এ LINQ দিয়ে Pagination কীভাবে করবেন?

```csharp
// Generic PagedList
public class PagedList<T>
{
    public List<T> Items      { get; init; } = [];
    public int TotalCount     { get; init; }
    public int Page           { get; init; }
    public int PageSize       { get; init; }
    public int TotalPages     => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious   => Page > 1;
    public bool HasNext       => Page < TotalPages;

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> query, int page, int pageSize)
    {
        int total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>
        {
            Items      = items,
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        };
    }
}

// ব্যবহার
var paged = await PagedList<ProductDto>.CreateAsync(
    _ctx.Products
        .Where(p => p.IsActive)
        .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
        .OrderBy(p => p.Name),
    page:     2,
    pageSize: 20
);

// Response Header-এ pagination metadata
Response.Headers.Add("X-Total-Count",  paged.TotalCount.ToString());
Response.Headers.Add("X-Total-Pages",  paged.TotalPages.ToString());
Response.Headers.Add("X-Has-Next",     paged.HasNext.ToString());
```

---

## প্রশ্ন ২৩ — Complex Business Logic LINQ Query-তে কীভাবে প্রয়োগ করবেন?

```csharp
// Domain-driven approach — business rule domain-এ রাখুন
public class DiscountCalculator
{
    public static decimal GetDiscount(decimal price, string tier)
        => tier switch
        {
            "Gold"   => price * 0.20m,
            "Silver" => price * 0.10m,
            "Bronze" => price * 0.05m,
            _        => 0m
        };
}

// LINQ + Business Logic
public async Task<List<InvoiceDto>> GenerateInvoicesAsync(int customerId)
{
    var customer = await _ctx.Customers
        .Include(c => c.Orders)
        .ThenInclude(o => o.Items)
        .FirstOrDefaultAsync(c => c.Id == customerId)
        ?? throw new NotFoundException();

    return customer.Orders
        .Where(o => o.Status == OrderStatus.Completed)
        .Select(o => new InvoiceDto
        {
            OrderId    = o.Id,
            Subtotal   = o.Items.Sum(i => i.Quantity * i.Price),
            Discount   = DiscountCalculator.GetDiscount(
                o.Items.Sum(i => i.Quantity * i.Price),
                customer.MembershipTier),
            Tax        = o.Items.Sum(i => i.Quantity * i.Price) * 0.15m,
            DueDate    = o.CreatedAt.AddDays(30)
        })
        .OrderByDescending(inv => inv.DueDate)
        .ToList();
}
```

---

## প্রশ্ন ২৪ — Multiple User Data Modify করার সময় LINQ-এ Concurrency কীভাবে Manage করবেন?

```csharp
// ১. Optimistic Concurrency — RowVersion দিয়ে
public class Product
{
    public int    Id      { get; set; }
    public string Name    { get; set; } = "";
    public decimal Price  { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = []; // Concurrency token
}

public async Task UpdatePriceAsync(int id, decimal newPrice)
{
    var product = await _ctx.Products.FindAsync(id)
        ?? throw new NotFoundException();

    product.Price = newPrice;

    try
    {
        await _ctx.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        // অন্য user আগেই পরিবর্তন করেছে
        var entry    = ex.Entries.Single();
        var dbValues = await entry.GetDatabaseValuesAsync();

        if (dbValues is null)
            throw new Exception("Product deleted by another user");

        // refresh করে আবার চেষ্টা
        entry.OriginalValues.SetValues(dbValues);
        await _ctx.SaveChangesAsync(); // retry
    }
}

// ২. Pessimistic Concurrency — Database lock
await _ctx.Database.ExecuteSqlRawAsync(
    "SELECT * FROM Products WITH (UPDLOCK) WHERE Id = {0}", productId);
```

---

## প্রশ্ন ২৫ — LINQ-এ Expression Trees কী?

**Expression Tree** হলো code-এর data structure representation।  
LINQ `IQueryable`-এ lambda expression গুলো string-এ compile হয় না, Expression Tree হিসেবে রাখা হয়।  
EF Core এই Tree বিশ্লেষণ করে SQL তৈরি করে।

```csharp
// Lambda → Expression Tree
Expression<Func<User, bool>> expr = u => u.IsActive && u.Age > 18;

// Expression Tree বিশ্লেষণ
var body        = (BinaryExpression)expr.Body;
var left        = (MemberExpression)((BinaryExpression)body.Left).Left;
var right       = ((ConstantExpression)((BinaryExpression)body.Right).Right).Value;

Console.WriteLine(left.Member.Name); // "IsActive"
Console.WriteLine(right);            // 18

// Dynamic Expression Tree তৈরি
public Expression<Func<T, bool>> BuildFilter<T>(string field, object value)
{
    var param    = Expression.Parameter(typeof(T), "x");
    var property = Expression.Property(param, field);
    var constant = Expression.Constant(value);
    var equals   = Expression.Equal(property, constant);
    return Expression.Lambda<Func<T, bool>>(equals, param);
}

// ব্যবহার
var filter = BuildFilter<User>("IsActive", true);
var users  = await _ctx.Users.Where(filter).ToListAsync();
// SQL: WHERE IsActive = 1

// ⚠️ Func<T, bool> → Expression<Func<T, bool>>-এর পার্থক্য:
// Func — compiled delegate, IEnumerable-এ কাজ করে
// Expression — tree, IQueryable-এ SQL-এ translate হয়
```

---

## প্রশ্ন ২৬ — NoSQL Database (যেমন MongoDB) LINQ দিয়ে কীভাবে Handle করবেন?

```csharp
// MongoDB + LINQ (MongoDB.Driver)
var client     = new MongoClient(connectionString);
var database   = client.GetDatabase("MyApp");
var collection = database.GetCollection<Product>("products");

// LINQ query — MongoDB LINQ provider
var query = collection.AsQueryable()
    .Where(p => p.IsActive && p.Price > 100)
    .OrderBy(p => p.Name)
    .Select(p => new { p.Id, p.Name, p.Price });

var result = await query.ToListAsync();

// ২. EF Core + Cosmos DB
services.AddDbContext<AppDbContext>(o =>
    o.UseCosmos(endpoint, key, databaseName));

// Cosmos-এ LINQ (partial support)
var items = await _ctx.Products
    .Where(p => p.PartitionKey == "Electronics")
    .ToListAsync();

// ৩. Redis + LINQ (in-memory cache)
var cached = await _redisCache.GetAsync<List<Product>>("products:all");
if (cached is null)
{
    cached = await _ctx.Products.ToListAsync();
    await _redisCache.SetAsync("products:all", cached, TimeSpan.FromMinutes(5));
}
var result2 = cached.Where(p => p.Price > 100).ToList();
```

---

## প্রশ্ন ২৭ — EF Core LINQ Query-তে Data Validation এবং Integrity Constraint কীভাবে Handle করে?

```csharp
// ১. Model-level constraint (EF Core Fluent API)
modelBuilder.Entity<User>(e =>
{
    e.Property(u => u.Email).IsRequired().HasMaxLength(256);
    e.HasIndex(u => u.Email).IsUnique();
    e.Property(u => u.Age).HasDefaultValue(0);
    e.HasCheckConstraint("CK_User_Age", "Age >= 0 AND Age <= 150");
});

// ২. LINQ + Validation service
public async Task<Result> CreateUserAsync(CreateUserDto dto)
{
    // Domain validation
    if (string.IsNullOrWhiteSpace(dto.Email))
        return Result.Fail("Email প্রয়োজন");

    // Uniqueness check
    var exists = await _ctx.Users.AnyAsync(u => u.Email == dto.Email);
    if (exists)
        return Result.Fail("Email ইতিমধ্যে ব্যবহৃত");

    var user = new User { Email = dto.Email, Name = dto.Name };
    _ctx.Users.Add(user);

    try
    {
        await _ctx.SaveChangesAsync();
        return Result.Ok();
    }
    catch (DbUpdateException ex) when (ex.IsDuplicateKeyException())
    {
        return Result.Fail("Email ইতিমধ্যে ব্যবহৃত (race condition)");
    }
}

// ৩. Global query filter — soft delete
modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
// এরপর সব query-তে স্বয়ংক্রিয়ভাবে WHERE IsDeleted=0 যোগ হয়
```

---

## প্রশ্ন ২৮ — Complex LINQ Query-তে `GroupBy`-এর উদ্দেশ্য কী?

```csharp
// ১. Sales report — মাস ও category অনুযায়ী
var salesReport = await _ctx.Orders
    .Where(o => o.CreatedAt.Year == 2026)
    .GroupBy(o => new { o.CreatedAt.Month, o.Product.Category })
    .Select(g => new
    {
        Month    = g.Key.Month,
        Category = g.Key.Category,
        Count    = g.Count(),
        Revenue  = g.Sum(o => o.Amount),
        AvgOrder = g.Average(o => o.Amount),
        MaxOrder = g.Max(o => o.Amount)
    })
    .OrderBy(x => x.Month).ThenBy(x => x.Category)
    .ToListAsync();

// ২. Running total with GroupBy
var monthlyCumulative = await _ctx.Orders
    .GroupBy(o => o.CreatedAt.Month)
    .Select(g => new { Month = g.Key, Total = g.Sum(o => o.Amount) })
    .OrderBy(x => x.Month)
    .ToListAsync();

// ৩. Having clause (filter on group)
var bigGroups = await _ctx.Orders
    .GroupBy(o => o.CustomerId)
    .Where(g => g.Count() >= 5 && g.Sum(o => o.Amount) > 10000) // HAVING
    .Select(g => new { CustomerId = g.Key, Orders = g.Count() })
    .ToListAsync();

// ৪. Nested GroupBy
var nested = await _ctx.Employees
    .GroupBy(e => e.Department)
    .Select(dg => new
    {
        Department  = dg.Key,
        ByTitle     = dg.GroupBy(e => e.Title)
                        .Select(tg => new { Title = tg.Key, Count = tg.Count() })
    })
    .ToListAsync();
```

---

## প্রশ্ন ২৯ — LINQ Query async ব্যবহার করে Task-এর মাধ্যমে কীভাবে Execute করবেন?

```csharp
// ১. Basic async LINQ operators
public async Task<DashboardDto> GetDashboardAsync()
{
    // Parallel async execution
    var usersTask    = _ctx.Users.CountAsync();
    var ordersTask   = _ctx.Orders.CountAsync();
    var revenueTask  = _ctx.Orders.SumAsync(o => o.Amount);
    var pendingTask  = _ctx.Orders
        .Where(o => o.Status == OrderStatus.Pending)
        .CountAsync();

    await Task.WhenAll(usersTask, ordersTask, revenueTask, pendingTask);

    return new DashboardDto
    {
        TotalUsers    = await usersTask,
        TotalOrders   = await ordersTask,
        TotalRevenue  = await revenueTask,
        PendingOrders = await pendingTask
    };
}

// ২. Async stream — large dataset
public async IAsyncEnumerable<ReportRow> GenerateReportAsync(
    [EnumeratorCancellation] CancellationToken ct = default)
{
    await foreach (var order in _ctx.Orders.AsAsyncEnumerable().WithCancellation(ct))
    {
        yield return new ReportRow
        {
            OrderId  = order.Id,
            Amount   = order.Amount,
            Computed = await ComputeComplexValueAsync(order)
        };
    }
}

// ৩. CancellationToken — request cancel হলে query বন্ধ
public async Task<List<Product>> SearchAsync(string term, CancellationToken ct)
    => await _ctx.Products
        .Where(p => p.Name.Contains(term))
        .ToListAsync(ct); // request cancel হলে DB query বন্ধ হয়
```

---

## প্রশ্ন ৩০ — REST API বা External Service-এর সাথে LINQ কীভাবে Integrate করবেন?

```csharp
// ১. HttpClient + LINQ — API response filter করা
public class ProductApiService
{
    private readonly HttpClient _http;

    public async Task<List<ProductDto>> GetActiveExpensiveAsync()
    {
        var response = await _http.GetFromJsonAsync<List<ProductDto>>(
            "https://api.example.com/products");

        return response?
            .Where(p => p.IsActive && p.Price > 500)
            .OrderByDescending(p => p.Price)
            .Take(20)
            .ToList() ?? [];
    }
}

// ২. Aggregate data from multiple APIs
public async Task<CombinedReport> GetCombinedReportAsync()
{
    var usersTask    = _userApi.GetAllAsync();
    var ordersTask   = _orderApi.GetAllAsync();
    var productsTask = _productApi.GetAllAsync();

    await Task.WhenAll(usersTask, ordersTask, productsTask);

    var users    = await usersTask;
    var orders   = await ordersTask;
    var products = await productsTask;

    // LINQ join in-memory across APIs
    var report = orders
        .Join(users,    o => o.UserId,    u => u.Id, (o, u) => new { o, u })
        .Join(products, x => x.o.ProductId, p => p.Id,
            (x, p) => new ReportRow
            {
                User    = x.u.Name,
                Product = p.Name,
                Amount  = x.o.Amount
            });

    return new CombinedReport { Rows = report.ToList() };
}
```

---

## প্রশ্ন ৩১ — Highly Distributed Environment-এ `IQueryable` কীভাবে ব্যবহার করবেন?

```csharp
// ১. Repository abstraction — implementation swap করা যায়
public interface IProductRepository
{
    IQueryable<Product> Query();
    Task<Product?> FindByIdAsync(int id);
}

// SQL implementation
public class SqlProductRepository : IProductRepository
{
    private readonly AppDbContext _ctx;
    public IQueryable<Product> Query() => _ctx.Products.AsNoTracking();
    public Task<Product?> FindByIdAsync(int id) => _ctx.Products.FindAsync(id).AsTask();
}

// ২. Read/Write separation — distributed
public class DistributedProductService
{
    private readonly IProductRepository _readRepo;  // Read replica
    private readonly AppDbContext _writeCtx;        // Primary DB

    // Read → Read replica
    public async Task<List<ProductDto>> GetListAsync()
        => await _readRepo.Query()
            .Where(p => p.IsActive)
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
            .ToListAsync();

    // Write → Primary
    public async Task UpdateAsync(int id, string name)
    {
        var p = await _writeCtx.Products.FindAsync(id)!;
        p.Name = name;
        await _writeCtx.SaveChangesAsync();
    }
}

// ৩. Caching layer — IQueryable result cache করা
public async Task<List<Product>> GetCachedAsync()
{
    var cacheKey = "products:all";
    var cached   = await _cache.GetAsync<List<Product>>(cacheKey);

    if (cached is not null) return cached;

    var fresh = await _ctx.Products.AsNoTracking().ToListAsync();
    await _cache.SetAsync(cacheKey, fresh, TimeSpan.FromMinutes(5));
    return fresh;
}
```

---

## প্রশ্ন ৩২ — Database থেকে Large Amount Data Fetch করে LINQ Optimize কীভাবে করবেন?

```csharp
// ১. Streaming — AsAsyncEnumerable() — সব একসাথে load নয়
public async Task ExportToCsvAsync(Stream output)
{
    await using var writer = new StreamWriter(output);
    await writer.WriteLineAsync("Id,Name,Price");

    await foreach (var p in _ctx.Products
        .Where(p => p.IsActive)
        .Select(p => new { p.Id, p.Name, p.Price })
        .AsNoTracking()
        .AsAsyncEnumerable())
    {
        await writer.WriteLineAsync($"{p.Id},{p.Name},{p.Price}");
    }
}

// ২. Cursor-based pagination — বড় dataset export
var lastId   = 0;
var batchSize = 500;
while (true)
{
    var batch = await _ctx.Products
        .Where(p => p.Id > lastId)
        .OrderBy(p => p.Id)
        .Take(batchSize)
        .AsNoTracking()
        .ToListAsync();

    if (!batch.Any()) break;
    await ProcessBatchAsync(batch);
    lastId = batch.Last().Id;
}

// ৩. Projection — শুধু প্রয়োজনীয় column
var minimal = await _ctx.Products
    .Where(p => p.IsActive)
    .Select(p => new { p.Id, p.Name }) // ২টি column, সব নয়
    .AsNoTracking()
    .ToListAsync();

// ৪. EF Core Bulk Read
var ids = await _ctx.Products
    .Where(p => p.IsActive)
    .Select(p => p.Id)      // শুধু ID list
    .ToListAsync();
```

---

## প্রশ্ন ৩৩ — Large Dataset-এ LINQ-এর Performance Bottleneck কীভাবে সমাধান করবেন?

```csharp
// ১. Query Execution Plan দেখুন
_ctx.Database.Log = Console.WriteLine; // generated SQL log করুন

// ২. Profiling — MiniProfiler বা EF Core logging
services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(cs)
     .EnableSensitiveDataLogging()
     .LogTo(Console.WriteLine, LogLevel.Information));

// ৩. Missing index চিহ্নিত করুন
// SQL Server: sys.dm_db_missing_index_details

// ৪. COUNT চেয়ে ANY দ্রুত
if (await _ctx.Users.AnyAsync(u => u.IsActive)) { }   // ✅
// if (await _ctx.Users.CountAsync(...) > 0) { }       // ❌ slow

// ৫. Avoid SELECT N+1 — Projection বা Include
var result = await _ctx.Orders
    .Select(o => new
    {
        o.Id,
        CustomerName = o.Customer.Name // JOIN হবে, আলাদা query নয়
    }).ToListAsync();

// ৬. Database-side computation
var stats = await _ctx.Orders
    .GroupBy(o => o.CustomerId)
    .Select(g => new
    {
        CustomerId = g.Key,
        Total      = g.Sum(o => o.Amount),  // DB-তে SUM
        Count      = g.Count()               // DB-তে COUNT
    }).ToListAsync();
// সব calculation SQL Server-এ হয়, C#-এ নয়
```

---

## প্রশ্ন ৩৪ — Large Collection Query করার সময় LINQ Memory Management এবং Garbage Collection কীভাবে Handle করে?

```csharp
// ১. Streaming — মেমোরিতে সব load করে না
await foreach (var item in _ctx.Orders.AsAsyncEnumerable())
{
    Process(item); // একটা একটা করে GC পারে collect করতে
}

// ২. Chunk — batch মেমোরি ব্যবহার সীমিত
await foreach (var batch in _ctx.Orders.AsAsyncEnumerable().Chunk(100))
{
    ProcessBatch(batch); // ১০০টি করে, পুরনো batch GC eligible
}

// ৩. Projection — object কম তৈরি হয়
var names = await _ctx.Users
    .Select(u => u.Name) // string শুধু, পুরো User object নয়
    .ToListAsync();

// ৪. AsNoTracking — Change Tracker-এর reference বাদ দেয়
var products = await _ctx.Products.AsNoTracking().ToListAsync();
// Tracked object-এ internal dictionary থাকে — বেশি মেমোরি

// ৫. Dispose DbContext সময়মতো
await using var ctx = new AppDbContext(options); // using = dispose হবে
var result = await ctx.Products.ToListAsync();
// ctx dispose হলে tracked entities-ও release

// ৬. struct বা readonly record — value type projection
var items = await _ctx.Products
    .Select(p => new ProductSummary(p.Id, p.Name, p.Price))
    .ToListAsync();

public readonly record struct ProductSummary(int Id, string Name, decimal Price);
```

---

## প্রশ্ন ৩৫ — Built-in Operator ছাড়া LINQ-এ Custom Aggregation কীভাবে Implement করবেন?

```csharp
// ১. Aggregate — সবচেয়ে flexible
var numbers = Enumerable.Range(1, 10).ToList();

// Geometric mean
double geoMean = Math.Pow(
    numbers.Aggregate(1.0, (acc, n) => acc * n),
    1.0 / numbers.Count);

// Variance
double mean     = numbers.Average();
double variance = numbers.Aggregate(0.0,
    (acc, n) => acc + Math.Pow(n - mean, 2),
    total => total / numbers.Count);

// ২. Custom extension method
public static class LinqExtensions
{
    public static decimal WeightedAverage<T>(
        this IEnumerable<T> source,
        Func<T, decimal> value,
        Func<T, decimal> weight)
    {
        var items     = source.ToList();
        var totalW    = items.Sum(weight);
        if (totalW == 0) return 0;
        return items.Sum(i => value(i) * weight(i)) / totalW;
    }

    public static decimal Median(this IEnumerable<decimal> source)
    {
        var sorted = source.OrderBy(x => x).ToList();
        int mid    = sorted.Count / 2;
        return sorted.Count % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2
            : sorted[mid];
    }
}

// ব্যবহার
decimal weighted = orders.WeightedAverage(o => o.Price, o => o.Quantity);
decimal median   = scores.Median();

// ৩. .NET 9+ AggregateBy — key-based custom aggregation
var byCategory = products.AggregateBy(
    p => p.Category,
    0m,
    (acc, p) => acc + p.Price * p.Stock
);
```

---

## প্রশ্ন ৩৬ — Unit Test Environment-এ LINQ Query কীভাবে Test করবেন?

```csharp
// ১. In-Memory Database (EF Core)
public class ProductRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB প্রতি test-এ
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetActive_ReturnsOnlyActiveProducts()
    {
        // Arrange
        await using var ctx = CreateContext();
        ctx.Products.AddRange(
            new Product { Name = "A", IsActive = true,  Price = 100 },
            new Product { Name = "B", IsActive = false, Price = 200 },
            new Product { Name = "C", IsActive = true,  Price = 300 }
        );
        await ctx.SaveChangesAsync();

        var repo = new ProductRepository(ctx);

        // Act
        var result = await repo.GetActiveAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.True(p.IsActive));
    }
}

// ২. Mock IQueryable — Moq + TestAsyncEnumerable
var data = new List<Product> { ... }.AsQueryable();
var mockSet = new Mock<DbSet<Product>>();
mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(data.Provider);
mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(data.Expression);

// ৩. LINQ to Objects test (domain logic পরীক্ষা)
[Fact]
public void Filter_ActiveAndExpensive_Works()
{
    var products = new List<Product>
    {
        new() { IsActive = true,  Price = 600 },
        new() { IsActive = false, Price = 700 },
        new() { IsActive = true,  Price = 300 }
    };

    var result = products.Where(p => p.IsActive && p.Price > 500).ToList();

    Assert.Single(result);
    Assert.Equal(600, result[0].Price);
}
```

---

## প্রশ্ন ৩৭ — Large Dataset-এ Pagination এবং Filtering কীভাবে Dynamically Handle করবেন?

```csharp
// Generic Dynamic Query Builder
public class DynamicQueryBuilder<T> where T : class
{
    private IQueryable<T> _query;

    public DynamicQueryBuilder(IQueryable<T> query) => _query = query;

    public DynamicQueryBuilder<T> ApplyFilter(
        Expression<Func<T, bool>>? filter)
    {
        if (filter is not null) _query = _query.Where(filter);
        return this;
    }

    public DynamicQueryBuilder<T> ApplySort(
        Expression<Func<T, object>>? keySelector,
        bool descending = false)
    {
        if (keySelector is null) return this;
        _query = descending
            ? _query.OrderByDescending(keySelector)
            : _query.OrderBy(keySelector);
        return this;
    }

    public async Task<PagedResult<T>> ToPagedAsync(int page, int pageSize)
    {
        int total = await _query.CountAsync();
        var items = await _query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PagedResult<T>
        {
            Items      = items,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }
}

// ব্যবহার
var result = await new DynamicQueryBuilder<Product>(_ctx.Products)
    .ApplyFilter(p => p.IsActive && p.Price > 100)
    .ApplySort(p => p.Price, descending: true)
    .ToPagedAsync(page: 2, pageSize: 20);
```

---

## প্রশ্ন ৩৮ — LINQ to SQL Database Schema পরিবর্তন কীভাবে Handle করে?

**LINQ to SQL (Deprecated)**
- `.dbml` ফাইল manually আপডেট করতে হতো
- Schema change আপডেট না করলে runtime error

**Entity Framework Core (আধুনিক)**
- **Migration** দিয়ে schema পরিবর্তন track করা হয়

```csharp
// Schema পরিবর্তনের workflow:

// ১. Model পরিবর্তন
public class Product
{
    public int     Id          { get; set; }
    public string  Name        { get; set; } = "";
    public decimal Price       { get; set; }
    public string? Description { get; set; } // নতুন column যোগ
}

// ২. Migration তৈরি
// dotnet ef migrations add AddProductDescription

// ৩. Migration apply
// dotnet ef database update

// ৪. Migration rollback
// dotnet ef database update PreviousMigration

// ৫. Auto-migration (production-এ সতর্কতার সাথে)
await _ctx.Database.MigrateAsync();

// ৬. Backward compatibility — nullable/default value
modelBuilder.Entity<Product>()
    .Property(p => p.Description)
    .HasDefaultValue(""); // পুরনো row-এ empty string
```

---

## প্রশ্ন ৩৯ — LINQ to SQL-এ Custom Database Schema Data Model-এ কীভাবে Map করবেন?

```csharp
// EF Core Fluent API — custom schema mapping

public class AppDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ১. Custom table name ও schema
        mb.Entity<User>().ToTable("tbl_users", schema: "auth");

        // ২. Custom column name ও type
        mb.Entity<User>(e =>
        {
            e.Property(u => u.Name)
             .HasColumnName("user_name")
             .HasMaxLength(100)
             .IsRequired();

            e.Property(u => u.CreatedAt)
             .HasColumnName("created_at")
             .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");
        });

        // ৩. Composite primary key
        mb.Entity<OrderItem>()
            .HasKey(i => new { i.OrderId, i.ProductId });

        // ৪. Relationship mapping
        mb.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ৫. Value conversion (Enum → string)
        mb.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>();

        // ৬. Shadow property — model-এ field নেই, DB-তে আছে
        mb.Entity<User>().Property<DateTime>("LastModified");
    }
}
```

---

## প্রশ্ন ৪০ — কোন Real-world Scenario-তে LINQ Data Access Logic নাটকীয়ভাবে সরল করে?

```csharp
// ১. E-commerce — Dynamic Product Search
var products = await new ProductFilterBuilder(_ctx.Products)
    .WithSearch(request.Search)
    .WithPriceRange(request.MinPrice, request.MaxPrice)
    .WithCategory(request.CategoryId)
    .WithInStock()
    .Build()
    .OrderByDescending(p => p.Rating)
    .ToPagedAsync(request.Page, request.PageSize);

// ২. Reporting — Sales Dashboard
var dashboard = new
{
    TopProducts  = await _ctx.OrderItems
        .GroupBy(i => i.Product.Name)
        .Select(g => new { Product = g.Key, Revenue = g.Sum(i => i.Quantity * i.Price) })
        .OrderByDescending(x => x.Revenue).Take(10).ToListAsync(),

    MonthlySales = await _ctx.Orders
        .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
        .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(o => o.Amount) })
        .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync(),

    ActiveUsers  = await _ctx.Users
        .Where(u => u.LastLogin > DateTime.UtcNow.AddDays(-30))
        .CountAsync()
};

// ৩. Notification system — complex rule
var toNotify = await _ctx.Users
    .Where(u => u.IsActive)
    .Where(u => u.Subscriptions.Any(s =>
        s.ExpiresAt <= DateTime.UtcNow.AddDays(7) &&
        s.AutoRenew == false))
    .Where(u => !u.Notifications.Any(n =>
        n.Type == "ExpiryWarning" &&
        n.SentAt >= DateTime.UtcNow.AddDays(-3)))
    .Select(u => new { u.Id, u.Email, u.Name })
    .ToListAsync();

// ৪. Multi-tenant — Global Query Filter
modelBuilder.Entity<Order>()
    .HasQueryFilter(o => o.TenantId == _tenantContext.CurrentTenantId);
// সব query-তে স্বয়ংক্রিয়ভাবে tenant filter যোগ হয়!
```

---

> **সারসংক্ষেপ:**
> Experienced LINQ-এর মূল দক্ষতা: Expression Tree, Specification Pattern, EF Core Change Tracking, Concurrency Management, Distributed Query, N+1 Prevention, Custom Aggregation, Unit Testing, এবং Real-world Complex Filtering।

---
*সর্বশেষ আপডেট: .NET 9 | EF Core 9 | C# 13 | 2026*
