# C# Master Roadmap: Detailed Breakdown

এই রোডম্যাপটি C# শেখা এবং ইন্টারভিউ প্রস্তুতির জন্য ধাপে ধাপে সাজানো হয়েছে।

## ১. C# Basics & Fundamentals (বেসিক ও আর্কিটেকচার)
*   **.NET Architecture:** C# কোড কীভাবে রান করে? (Source Code -> Compiler -> IL/MSIL -> CLR (JIT) -> Machine Code)। CTS, CLS, Assembly (.dll / .exe) কী?
*   **Data Types & Memory:** 
    *   **Value Types:** `int`, `float`, `double`, `decimal`, `char`, `bool`, `struct`, `enum` (এগুলো মেমোরির Stack-এ কিভাবে থাকে?)।
    *   **Reference Types:** `string`, `object`, `class`, `interface`, `delegate`, arrays (এগুলো মেমোরির Heap-এ কিভাবে থাকে?)।
*   **Operators (অপারেটর):** 
    *   Arithmetic, Relational, Logical, Assignment, Bitwise.
    *   **Special Operators:** Null-conditional (`?.`), Null-coalescing (`??`, `??=`), Ternary (`?:`), `typeof`, `sizeof`, `nameof`.
*   **Type Casting & Conversion:** Implicit, Explicit casting, `Parse()`, `TryParse()`, `Convert` class, `as` এবং `is` অপারেটর।
*   **Control Flow (কন্ট্রোল ফ্লো):** `if-else`, `switch` (traditional এবং C# 8/9 এর Pattern Matching switch expression)।
*   **Loops (লুপ):** `for`, `while`, `do-while`, `foreach`, `break`, `continue`.
*   **Strings & Manipulation:** String এর Immutability (কেন স্ট্রিং পরিবর্তন করা যায় না), `String` vs `StringBuilder`, String interpolation (`$""`), Verbatim strings (`@""`), Raw string literals (`""" """`)।
*   **Arrays:** 1D, 2D (`[,]`), Jagged arrays (`[][]`), Array এর বিভিন্ন মেথড (Sort, Reverse, Find)।

## ২. Object-Oriented Programming (OOP) Deep Dive (অবজেক্ট-ওরিয়েন্টেড প্রোগ্রামিং)
*   **Classes & Objects:** ক্লাস তৈরি, `new` কিওয়ার্ড দিয়ে অবজেক্ট তৈরি, Object Initializers, `this` কিওয়ার্ডের ব্যবহার।
*   **Class Members (ক্লাসের ভেতরের জিনিসপত্র):**
    *   **Fields:** ক্লাসের ভেতরের ভেরিয়েবল (State)।
    *   **Methods:** ক্লাসের ফাংশন বা কাজ (Behavior)।
    *   **Parameters:** Pass by value, Pass by reference (`ref`, `out`, `in` modifiers), Optional parameters, Named parameters।
*   **Properties:** Getters (`get`), Setters (`set`), Auto-implemented properties, Backing fields, `init` only setter (C# 9)।
*   **Constructors (কনস্ট্রাক্টর):** Default, Parameterized, Copy, Static constructor, Private constructor, Constructor overloading, Constructor chaining (`this()`, `base()`)।
*   **Encapsulation (এনক্যাপসুলেশন):**
    *   **Access Modifiers:** `public`, `private`, `protected`, `internal`, `protected internal`, `private protected` (কোনটা কোথায় অ্যাক্সেস করা যায়)।
    *   ডেটা হাইডিং (Private field কে public property দিয়ে কন্ট্রোল করা)।
*   **Inheritance (ইনহেরিটেন্স):** Base (Parent) class, Derived (Child) class, `base` কিওয়ার্ড, `sealed` ক্লাস (যাকে ইনহেরিট করা যায় না)।
*   **Polymorphism (পলিমরফিজম):**
    *   **Compile-time:** Method Overloading, Operator Overloading.
    *   **Run-time:** `virtual` মেথড, `override` মেথড, `new` কিওয়ার্ড (Method Hiding)।
*   **Abstraction (অ্যাবস্ট্রাকশন):**
    *   **Abstract Classes:** `abstract` ক্লাস এবং মেথড (যার কোনো বডি থাকে পণ্ডিত থাকে না)।
    *   **Interfaces:** ইন্টারফেস ডিফাইন করা, Multiple inheritance (একাধিক ইন্টারফেস ইমপ্লিমেন্ট করা), Default Interface Methods (C# 8)।

## ৩. Advanced Class Concepts (অ্যাডভান্সড ক্লাস কনসেপ্ট)
*   **Static Members & Classes:** `static` ভেরিয়েবল, মেথড এবং ক্লাস। কখন Static আর কখন Instance ব্যবহার করব?
*   **Extension Methods:** বিদ্যমান কোনো ক্লাস (যেমন String বা Int) এর মধ্যে কোড পরিবর্তন না করে নতুন মেথড যুক্ত করা।
*   **Partial Classes & Methods:** একটি ক্লাসকে একাধিক ফাইলে ভাগ করা।
*   **Structs (Value Types):** Struct vs Class, কখন ক্লাস না বানিয়ে Struct বানাবো?
*   **Enums (Enumerations):** কনস্ট্যান্ট ভ্যালুর গ্রুপ তৈরি করা, Flag Enums (`[Flags]`)।
*   **Records (C# 9+):** Immutable ডেটা মডেল, Value equality, `with` এক্সপ্রেশন।

## ৪. Generics (জেনেরিক্স)
*   **Generic Classes & Methods:** `List<T>`, `MyClass<T>` - Type (T) দিয়ে কীভাবে জেনেরিক কোড লেখা হয়, যা সব ডেটা টাইপে কাজ করে।
*   **Generic Constraints:** `where T : class`, `where T : struct`, `where T : new()` (T এর উপর শর্ত চাপিয়ে দেওয়া)।
*   **Covariance & Contravariance:** `out` এবং `in` কিওয়ার্ড (অ্যাডভান্সড টপিক)।

## ৫. Delegates, Events & Lambdas (ফাংশনাল প্রোগ্রামিংয়ের স্বাদ)
*   **Delegates:** ডেলিগেট কী? (Function Pointer), Single-cast, Multi-cast delegates।
*   **Built-in Delegates:** `Action` (রিটার্ন টাইপ void), `Func` (রিটার্ন টাইপ আছে), `Predicate` (রিটার্ন টাইপ bool)।
*   **Anonymous Methods & Lambdas:** `delegate()`, ল্যাম্বডা এক্সপ্রেশন `() => {}`।
*   **Events:** Publisher-Subscriber প্যাটার্ন, `event` কিওয়ার্ড, `EventHandler`, ইভেন্ট ফায়ার করা।

## ৬. Collections & Data Structures (ডেটা স্ট্রাকচার)
*   **Generic Collections:** `List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>` (ইউনিক ডেটা), `Queue<T>` (FIFO), `Stack<T>` (LIFO)। 
*   **Concurrent Collections:** `ConcurrentDictionary`, `ConcurrentQueue` (মাল্টি-থ্রেডিং এর জন্য)।
*   **Interfaces:** `IEnumerable` vs `ICollection` vs `IList` vs `IQueryable` (ইন্টারভিউয়ের জন্য সুপার ইম্পর্ট্যান্ট)।

## ৭. LINQ (Language Integrated Query)
*   **Syntax:** Query Syntax (SQL এর মতো) vs Method Syntax (Lambda দিয়ে)।
*   **Operators:** `Where`, `Select`, `OrderBy`, `GroupBy`, `Any`, `All`, `First`, `FirstOrDefault`, `SingleOrDefault` ইত্যাদি।
*   **Execution:** Deferred Execution (পরে এক্সিকিউট হওয়া) vs Immediate Execution (`.ToList()` কল করলে সাথে সাথে এক্সিকিউট হওয়া)।

## ৮. Error Handling & Exceptions (ত্রুটি ব্যবস্থাপনা)
*   **Keywords:** `try`, `catch`, `finally`, `throw`.
*   **Exception Types:** `NullReferenceException`, `ArgumentNullException` ইত্যাদি।
*   **Best Practices:** `throw ex;` বনাম `throw;` (Stack trace হারানো ঠেকানো), Custom Exception ক্লাস তৈরি করা।

## ৯. Asynchronous Programming & Multithreading (অ্যাসিঙ্ক্রোনাস)
*   **Task Parallel Library (TPL):** `Task` কী? CPU-bound vs I/O-bound কাজ।
*   **Async/Await:** `async` এবং `await` কিওয়ার্ডের ব্যবহার।
*   **Return Types:** `Task`, `Task<T>`, `ValueTask<T>`।
*   **Management:** `Task.WhenAll()`, `Task.WhenAny()`, CancellationToken (টাস্ক ক্যান্সেল করা)।
*   **Concurrency Issues:** Race conditions, Deadlocks, `lock` স্টেটমেন্ট।

## ১০. Memory Management (মেমোরি ম্যানেজমেন্ট)
*   **Garbage Collection (GC):** গার্বেজ কালেক্টর কীভাবে কাজ করে? Generation 0, 1, 2 কী?
*   **Unmanaged Resources:** ডেটাবেস কানেকশন, ফাইল স্ট্রিম ইত্যাদি হ্যান্ডেল করা।
*   **IDisposable Interface:** `Dispose()` মেথড, `using` স্টেটমেন্টের ব্যবহার।

## ১১. Advanced & Modern Features (কিছু প্রো লেভেলের জিনিস)
*   **Reflection:** রান-টাইমে ক্লাসের মেটাডেটা পড়া এবং ডাইনামিক অবজেক্ট বানানো।
*   **Attributes:** `[Obsolete]`, `[Serializable]` ইত্যাদি কাস্টম অ্যাট্রিবিউট বানানো।
*   **Indexers:** অবজেক্টকে অ্যারের মতো ইনডেক্স (`obj[0]`) দিয়ে অ্যাক্সেস করা।
