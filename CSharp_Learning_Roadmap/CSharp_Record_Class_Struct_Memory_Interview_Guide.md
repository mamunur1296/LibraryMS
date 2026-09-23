# C# .NET Interview Preparation: Types, Memory Management & OOP Master Guide

> এই ডকুমেন্টে আজকের সেশনের সকল বিষয়বস্তু—**Record বনাম Class**, **Mutability বনাম Immutability**, **Boxing & Unboxing-এর মেমোরি ইন্টারনালস ও ৭টি প্র্যাকটিক্যাল কোড সিনারিও**, **Struct-এর স্বরূপ**, **Value Object-এর আর্কিটেকচারাল ব্যাখ্যা**, এবং C#-এর সম্পূর্ণ **Type System (Class, Struct, Record, Tuple, Delegate, Enum)-এর পুঙ্খানুপুঙ্খ শ্রেণীবিন্যাস** বিস্তারিত সংকলিত করা হলো।

---

## সূচিপত্র
1. [Record বনাম Class](#১-record-বনাম-class)
2. [Mutable বনাম Immutable](#২-mutable-বনাম-immutable)
3. [Boxing ও Unboxing: মেমোরি ইন্টারনালস এবং কেন সতর্ক হবেন](#৩-boxing-ও-unboxing-মেমোরি-ইন্টারনালস-এবং-কেন-সতর্ক-হবেন)
4. [Boxing ও Unboxing-এর ৭টি প্র্যাকটিক্যাল কোড সিনারিও](#৪-boxing-ও-unboxing-এর-৭টি-প্র্যাকটিক্যাল-কোড-সিনারিও)
5. [ArrayList বনাম List<T> মেমোরি ইন্টারনালস (Generics-এর ভূমিকা)](#৫-arraylist-বনাম-listt-মেমোরি-ইন্টারনালস)
6. [Struct কী এবং Class বনাম Struct-এর মেমোরি আচরণ](#৬-struct-কী-এবং-class-বনাম-struct-এর-মেমোরি-আচরণ)
7. [Struct, Class বনাম Value Object (ডিজাইন প্যাটার্ন বনাম ল্যাঙ্গুয়েজ টাইপ)](#৭-struct-class-বনাম-value-object)
8. [C# Type System ও পূর্ণাঙ্গ শ্রেণীবিন্যাস (Classifications)](#৮-c-type-system-ও-পূর্ণাঙ্গ-শ্রেণীবিন্যাস)
   - [Class-এর শ্রেণীবিন্যাস](#ক-class-এর-শ্রেণীবিন্যাস)
   - [Struct-এর প্রকারভেদ](#খ-struct-এর-প্রকারভেদ)
   - [Record-এর প্রকারভেদ](#গ-record-এর-প্রকারভেদ)
   - [Tuple-এর প্রকারভেদ (Tuple বনাম ValueTuple)](#ঘ-tuple-এর-প্রকারভেদ)
   - [Delegate-এর প্রকারভেদ](#ঙ-delegate-এর-প্রকারভেদ)
   - [Enum-এর প্রকারভেদ](#চ-enum-এর-প্রকারভেদ)
   - [Anonymous Type](#ছ-anonymous-type)

---

## ১. Record বনাম Class

### ভাইবাতে এক লাইনে বলার মতো উত্তর
> **Class** হলো Reference Type যা মূলত **Object-oriented Behavior & State Management**-এর জন্য ব্যবহৃত হয় এবং বাই-ডিফল্ট এটি **Reference-based Equality** (মেমোরি রেফারেন্স) চেক করে।
>
> **Record** (C# 9.0+) মূলত **Data-centric & Immutable Models**-এর জন্য ডিজাইন করা হয়েছে, যা বাই-ডিফল্ট **Value-based Equality** (ভেতরের ভ্যালুগুলো সমান কি না) চেক করে এবং স্বয়ংক্রিয়ভাবে `ToString()`, `Equals()`, `GetHashCode()` ইত্যাদি মেথড জেনারেট করে দেয়।

### মূল পার্থক্যসমূহ

| বিষয় | `Class` | `Record` |
| :--- | :--- | :--- |
| **মূল উদ্দেশ্য** | আচরণ (Behavior / Logic) ও মিউটেবল স্টেট হ্যান্ডেল করা। | ডেটা হোল্ড করা (Data-centric modeling)। |
| **Equality Check** | **Reference Equality** (দুটি অবজেক্ট একই মেমোরি লোকেশনে আছে কি না তা দেখে)। | **Value Equality** (প্রোপার্টির মানগুলো হুবহু সমান কি না তা দেখে)। |
| **Mutability** | বাই-ডিফল্ট **Mutable** (মান পরিবর্তনযোগ্য)। | সাধারণত **Immutable** (`init-only` প্রোপার্টি)। |
| **Boilerplate Code** | `Equals()`, `ToString()`, `GetHashCode()` নিজে ওভাররাইড করতে হয়। | কম্পাইলার স্বয়ংক্রিয়ভাবে এগুলো তৈরি করে দেয়। |
| **Cloning / Copying** | ম্যানুয়ালি কপি কনস্ট্রাক্টর বা ক্লোন লজিক লিখতে হয়। | Built-in `with` এক্সপ্রেশন দিয়ে নন-ডিস্ট্রাক্টিভ মিউটেশন করা যায়। |
| **ডিক্লারেশন** | `public class Person { ... }` | `public record Person(string Name, int Age);` |

### কখন `Record` ব্যবহার করবেন?
1. **DTO (Data Transfer Object):** ক্লায়েন্ট বা ডেটাবেস থেকে ডেটা আনা-নেওয়ার সময়।
2. **API Request / Response Models:** কন্ট্রোলার থেকে রেসপন্স বা রিকোয়েস্ট বডি রিসিভ করতে।
3. **CQRS Pattern & Messaging:** Command, Query বা Event অবজেক্ট তৈরি করতে (ইভেন্ট সাধারণত একবার তৈরির পর অপরিবর্তনীয় বা Immutable রাখা উচিত)।
4. **Domain-Driven Design (DDD) Value Objects:** যেখানে অবজেক্টের পরিচয় (Identity/ID) নয়, বরং তার ভেতরের মানগুলোর সাম্যতাই মূখ্য।
5. **Shallow Copyিং-এর প্রয়োজন হলে:** `with` এক্সপ্রেশন দিয়ে অবজেক্টের কিছু প্রোপার্টি পরিবর্তন করে নতুন অবজেক্ট তৈরির ক্ষেত্রে:
   ```csharp
   var p1 = new Person("Rahim", 25);
   var p2 = p1 with { Age = 26 }; // Rahim, 26
   ```

---

## ২. Mutable বনাম Immutable

### সহজ সংজ্ঞা
* **Mutable (মিউটেবল):** কোনো অবজেক্ট মেমোরিতে তৈরি (Initialize) হওয়ার পর যদি তার ভেতরের ভ্যালু বা স্টেট **সরাসরি পরিবর্তন করা যায়**, তবে তাকে Mutable অবজেক্ট বলে।
* **Immutable (ইমিউটেবল):** কোনো অবজেক্ট মেমোরিতে একবার তৈরি হয়ে গেলে তার ভেতরের ভ্যালু **আর কখনো সরাসরি পরিবর্তন করা যায় না**। কোনো পরিবর্তন করতে হলে সবসময় পুরনো অবজেক্টের আদলে **নতুন একটি অবজেক্ট তৈরি করতে হয়**।

### C#-এ কোড উদাহরণ

```csharp
// Mutable (Class)
public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var user = new User { Name = "Rahim", Age = 25 };
user.Age = 26; // মেমোরিতে একই অবজেক্টের ডেটা সরাসরি পরিবর্তন হচ্ছে

// Immutable (Record / init-only)
public record ImmutableUser(string Name, int Age);

var immUser = new ImmutableUser("Rahim", 25);
// immUser.Age = 26; // ❌ Compile-time ERROR!
var updatedUser = immUser with { Age = 26 }; // ✅ নতুন অবজেক্ট তৈরি হলো
```

### .NET-এ পরিচিত Immutable উদাহরণ: `string`
C#-এর `string` হলো বিল্ট-ইন ইমিউটেবল টাইপ:
```csharp
string text = "Hello";
text = text + " World";
```
> মেমোরিতে `"Hello"` ঠিক আগের মতোই থেকে যায়। নতুন জায়গায় `"Hello World"` তৈরি হয়ে `text` ভ্যারিয়েবলে তার নতুন রেফারেন্স বসে। মূল মেমোরি অবজেক্ট অপরিবর্তিত থাকে।

### কেন Immutability জরুরি?
1. **Thread Safety:** একাধিক থ্রেড একসাথে ডেটা রিড করলেও রেস কন্ডিশন বা ডেটা করাপশন হয় না।
2. **Side-Effect Free:** কোনো মেথডে অবজেক্ট পাস করলে মেথডটি গোপনে মূল ডেটা বদলে ফেলতে পারে না।
3. **Predictability:** সিস্টেমের স্টেট অনুমানযোগ্য ও নির্ভরযোগ্য থাকে।

---

## ৩. Boxing ও Unboxing: মেমোরি ইন্টারনালস এবং কেন সতর্ক হবেন

### সংজ্ঞা
* **Boxing:** একটি Value Type-কে Reference Type (যেমন `object` বা কোনো `interface`)-এ রূপান্তর করা। এর ফলে Stack-এ থাকা ডেটা Heap-এ নতুন অবজেক্ট হিসেবে মেমোরি দখল করে কপি হয়।
* **Unboxing:** Heap-এ বক্স হয়ে থাকা অবজেক্ট থেকে মূল Value Type মানটিকে পুনরায় বের করে Stack-এ ফিরিয়ে আনা।

### মেমোরির ভেতরে আসলে কী ঘটে?
```csharp
int age = 25;        // Value Type (Stack)
object obj = age;    // Boxing
```
```
    STACK                               MANAGED HEAP
+-----------+                      +-----------------------+
|  age: 25  |                      | SyncBlockIndex (8B)   |
+-----------+                      +-----------------------+
|  obj: *---+--------------------->| TypeHandle/MethodTable(8B)
+-----------+ (Pointer to Heap)    +-----------------------+
                                   | Value: 25 (4B)        |
                                   +-----------------------+
```
1. **Heap Allocation:** Managed Heap-এ অবজেক্ট হেডার (SyncBlockIndex + TypeHandle) সহ ১৬ থেকে ২৪ বাইট জায়গা বরাদ্দ হয়।
2. **Value Copy:** Stack-এর `25` মানটি বিট-বাই-বিট Heap-এর মেমোরিতে কপি হয়।
3. **Reference Return:** Heap-এর ওই মেমোরি অ্যাড্রেসটি Stack-এর `obj` ভ্যারিয়েবলে অ্যাসাইন হয়।

### Why Should You Care?
1. **GC Pressure & CPU Overhead:** ঘনঘন বক্সিং হলে Gen 0 Garbage Collection ঘনঘন ট্রিগার হয়, অ্যাপ্লিকেশনে স্ট্যাটার বা ল্যাগ দেখা দেয়।
2. **Performance Degradation:** Heap Allocation ও মেমোরি কপি Stack অপারেশনের চেয়ে ১০-২০ গুণ বেশি সময় নেয়।
3. **Type Safety Loss:** Unboxing-এর সময় ভুল টাইপ দিলে রানটাইমে `InvalidCastException` ঘটে।
4. **Struct Mutation Trap:** Boxed struct-এর মান পরিবর্তন করলে মূল স্ট্যাকের struct পরিবর্তিত হয় না।

---

## ৪. Boxing ও Unboxing-এর ৭টি প্র্যাকটিক্যাল কোড সিনারিও

### দৃশ্যপট ১: বেসিক Boxing এবং Unboxing
```csharp
int num = 100;              // Stack
object boxedNum = num;      // Boxing (Heap-এ নতুন অবজেক্ট)
int unboxedNum = (int)boxedNum; // Unboxing (Stack-এ কপি)

Console.WriteLine($"Original: {num}, Boxed: {boxedNum}, Unboxed: {unboxedNum}");
```
**আউটপুট:** `Original: 100, Boxed: 100, Unboxed: 100`

---

### দৃশ্যপট ২: টাইপ অমিল হলে ক্র্যাশ (`InvalidCastException`)
```csharp
int original = 50;
object boxed = original; // Boxed as int

try
{
    // ভুল: int-কে সরাসরি double-এ Unbox করার চেষ্টা
    double wrong = (double)boxed; 
}
catch (InvalidCastException ex)
{
    Console.WriteLine($"Error: {ex.Message}"); // Specified cast is not valid.
}

// সঠিক উপায়: আগে int-এ আনবক্স করে তারপর double-এ কাস্ট করা
double correct = (double)(int)boxed;
```

---

### দৃশ্যপট ৩: কালেকশনে Boxing-এর ফাঁদ (`ArrayList` বনাম `List<T>`)
```csharp
// ❌ ArrayList (Non-generic, Boxing & Unboxing ঘটে)
ArrayList arrList = new ArrayList();
arrList.Add(10); // Boxing!
int val1 = (int)arrList[0]; // Unboxing!

// ✅ List<T> (Generic, কোনো Boxing নেই)
List<int> genericList = new List<int>();
genericList.Add(10); // No Boxing!
int val2 = genericList[0]; // No Unboxing!
```

---

### দৃশ্যপট ৪: Struct মিউটেশনের অপ্রত্যাশিত বাগ
```csharp
struct Counter { public int Count { get; set; } }

Counter c = new Counter { Count = 5 };
object boxedC = c; // হিপে আলাদা কপি তৈরি হলো

c.Count = 10; // মূল c পরিবর্তন হলো

Console.WriteLine(c.Count); // 10
Console.WriteLine(((Counter)boxedC).Count); // 5 (বক্সড কপি অপরিবর্তিত রয়ে গেছে!)
```

---

### দৃশ্যপট ৫: Method Parameters ও String Formatting
```csharp
int score = 95;
bool isActive = true;

// ❌ Boxing ঘটে (params object[] বা object প্যারামিটার নেওয়ার কারণে)
string message = string.Format("Score: {0}, Active: {1}", score, isActive);

// ✅ সমাধান: .ToString() কল করা বা টাইপ-সেফ ইন্টারপোলেশন
string fastMessage = $"Score: {score.ToString()}, Active: {isActive.ToString()}";
```

---

### দৃশ্যপট ৬: Struct-কে Interface-এ কাস্ট করলে Boxing
```csharp
interface IDescribable { void Describe(); }
struct Product : IDescribable 
{ 
    public string Name;
    public void Describe() => Console.WriteLine(Name); 
}

// ❌ Boxing ঘটে (Interface একটি Reference Type)
void ProcessBoxing(IDescribable item) => item.Describe();

// ✅ No Boxing (Generic Constraint কম্পাইলারকে টাইপ ডিরেক্ট চিনিয়ে দেয়)
void ProcessNoBoxing<T>(T item) where T : IDescribable => item.Describe();
```

---

### দৃশ্যপট ৭: Nullable Value Type (`int?`)-এর অদ্ভুত আচরণ
```csharp
int? hasValue = 42;
int? isNull = null;

object boxed1 = hasValue; // পিওর int হিসেবে বক্স হয় (Type: System.Int32)
object boxed2 = isNull;   // কোনো হিপ অবজেক্ট তৈরি হয় না, সরাসরি মেমোরি পয়েন্টার 'null' রিটার্ন করে!

Console.WriteLine(boxed2 == null); // True
```

---

## ৫. ArrayList বনাম List<T> মেমোরি ইন্টারনালস

* **`ArrayList` এবং `List<T>` উভয়ই Heap মেমোরিতে থাকে এবং উভয়ই ডাইনামিক।**
* **কেন `ArrayList`-এ Boxing হয়?**
  `.NET 1.0`-এ জেনেরিকস ছিল না। `ArrayList`-এর ভেতরে থাকে একটি `object[]` অ্যারে। আপনি যখন `10` দেন, তখন সে কাঁচা সংখ্যা রাখতে পারে না, হিপে একটি বক্স অবজেক্ট বানিয়ে তার পয়েন্টার রাখে। ১০টি উপাদানের জন্য ১০টি আলাদা বক্স অবজেক্ট তৈরি হয়।
* **কেন `List<int>`-এ Boxing হয় না?**
  `C# 2.0`-এ Generics আসার পর যখন আপনি `List<int>` লেখেন, JIT কম্পাইলার হিপের ভেতরে সরাসরি একটি কাঁচা `int[]` অ্যারে বরাদ্দ করে। সেখানে সরাসরি ৪-বাইটের পূর্ণসংখ্যা পরপর মেমোরিতে বসে—কোনো বক্স অবজেক্ট বা রেফারেন্স পয়েন্টার তৈরি হয় না।

---

## ৬. Struct কী এবং Class বনাম Struct-এর মেমোরি আচরণ

### সংজ্ঞা
> **`struct` হলো C#-এর একটি Value Type।** `class` মেমোরির **Heap**-এ তৈরি হয়, আর `struct` সরাসরি **Stack** মেমোরিতে (অথবা প্যারেন্ট অবজেক্টের ভেতরে ইনলাইন হয়ে) তৈরি হয়।

C#-এর `int`, `double`, `bool`, `DateTime`, `Guid`, `TimeSpan`—এগুলো সবই মূলত এক একটি `struct`!

### Class বনাম Struct-এর কোড ও মেমোরি আচরণ
```csharp
public class PersonClass { public int Age; }
public struct PersonStruct { public int Age; }

// Class: রেফারেন্স কপি হয়
PersonClass c1 = new PersonClass { Age = 25 };
PersonClass c2 = c1;
c2.Age = 30;
Console.WriteLine(c1.Age); // 30 (উভয়েই একই অবজেক্টকে নির্দেশ করছে)

// Struct: সম্পূর্ণ স্বাধীন ভ্যালু কপি হয়
PersonStruct s1 = new PersonStruct { Age = 25 };
PersonStruct s2 = s1;
s2.Age = 30;
Console.WriteLine(s1.Age); // 25 (s1 অপরিবর্তিত)
```

### কখন Struct ব্যবহার করবেন?
1. অবজেক্টের সাইজ ছোট হলে (সাধারণত **১৬ বাইট বা তার কম**)।
2. স্বল্পস্থায়ী (Short-lived) কাজের জন্য।
3. অবজেক্টটি ইমিউটেবল হলে।
4. কোনো ইনহেরিটেন্সের প্রয়োজন না থাকলে।
*(যেমন: জিপিএস কো-অর্ডিনেট `Point(X, Y)`, `Color(R, G, B)`)*।

---

## ৭. Struct, Class বনাম Value Object

| বিষয় | `class` | `struct` | `Value Object` |
| :--- | :--- | :--- | :--- |
| **ক্যাটাগরি** | C# Language Type | C# Language Type | **Design Pattern / DDD Concept** |
| **মেমোরি** | Heap (Reference Type) | Stack / Inline (Value Type) | ইমপ্লিমেন্টেশনের ওপর নির্ভর করে |
| **আইডেন্টিটি** | মেমোরি রেফারেন্স | সরাসরি ভ্যালু | **কোনো নিজস্ব ID নেই; ভেতরের মানগুলোই তার পরিচয়** |

> **মূল শিক্ষা:** `class` ও `struct` হলো ভাষা বা কোডিংয়ের **কাঁচামাল (Building Blocks)**। আর `Value Object` হলো একটি **আর্কিটেকচারাল কনসেপ্ট**। C#-এ একটি Value Object বানাতে আপনি `record`, `class`, বা `readonly struct` যেকোনোটি ব্যবহার করতে পারেন।

---

## ৮. C# Type System ও পূর্ণাঙ্গ শ্রেণীবিন্যাস

সি#-এর মূল টাইপ সিস্টেম দুটি প্রধান ক্যাটাগরিতে বিভক্ত:
1. **Reference Types:** `class`, `record class`, `interface`, `delegate`
2. **Value Types:** `struct`, `record struct`, `enum`, `ValueTuple`

---

### ক. Class-এর শ্রেণীবিন্যাস (৮টি প্রধান প্রকার)

#### ১. ইনহেরিটেন্স ও অবজেক্ট তৈরির ক্ষমতা অনুযায়ী:
1. **Concrete / Standard Class:** সাধারণ ক্লাস, সরাসরি `new` দিয়ে অবজেক্ট তৈরি হয় এবং ইনহেরিট করা যায়।
2. **Abstract Class (`abstract`):** সরাসরি `new` করা যায় না। এটি বেইজ ক্লাস হিসেবে কাজ করে; চাইল্ড ক্লাসকে অ্যাবস্ট্রাক্ট মেম্বার ওভাররাইড করতে বাধ্য করে।
3. **Sealed Class (`sealed`):** একে অন্য কেউ ইনহেরিট করতে পারে না। সিকিউরিটির জন্য এবং পারফরম্যান্স বাড়াতে ব্যবহার হয় (যেমন .NET-এর `string`)।
4. **Static Class (`static`):** কোনো অবজেক্ট তৈরি করা যায় না, ইনহেরিটেন্স নেই। এর ভেতরের সব মেম্বার অবশ্যই `static` হতে হয় (যেমন `Math`, `Convert`)।

#### ২. কোড স্ট্রাকচার ও স্কোপ অনুযায়ী:
5. **Partial Class (`partial`):** একটি ক্লাসের কোডকে একাধিক আলাদা আলাদা ফাইলে ভাগ করে রাখা যায়। কম্পাইল টাইমে তারা একটি ক্লাসে যুক্ত হয়।
6. **Nested Class:** একটি ক্লাসের বডির ভেতরে আরেকটি ক্লাস ডিফাইন করা। সাধারণত ক্লাসের অভ্যন্তরীণ প্রাইভেট হেল্পার হিসেবে ব্যবহৃত হয়।

#### ৩. টাইপ ও ডেটা প্যাটার্ন অনুযায়ী:
7. **Generic Class (`class<T>`):** নির্দিষ্ট টাইপ ছাড়াই ক্লাস ডিফাইন করা যায় (যেমন `Repository<T>`)। টাইপ-সেফ ও নো-বক্সিং।
8. **Record Class (`record class`):** রেফারেন্স টাইপ হওয়া সত্ত্বেও ভ্যালু-বেসড সমতা এবং ইমিউটেবল স্টেট সাপোর্ট করে।

---

### খ. Struct-এর প্রকারভেদ (৪ প্রকার)

1. **Standard Struct:** সাধারণ ভ্যালু টাইপ, মিউটেবল হতে পারে।
2. **`readonly struct`:** সম্পূর্ণ ইমিউটেবল। কম্পাইলার বাড়তি ডিফেন্সিভ কপি তৈরি করা বন্ধ করে গতি বাড়ায়।
3. **`ref struct`:** এটি **শুধুমাত্র Stack-এ থাকতে পারে**। কখনো Heap-এ যেতে পারে না বা Box হতে পারে না (যেমন: হাই-পারফরম্যান্স `Span<T>`)।
4. **`readonly ref struct`:** একই সাথে রিড-অনলি এবং রেফ স্ট্রাক্ট (যেমন: `ReadOnlySpan<T>`)।

---

### গ. Record-এর প্রকারভেদ (২ প্রকার)

1. **`record class` (বা শুধু `record`):** Reference Type (Heap-এ থাকে)। C# 9-এর ফিচার।
2. **`record struct`:** Value Type (Stack-এ থাকে)। C# 10-এর ফিচার। রেকর্ডের ভ্যালু-বেসড সমতা এবং স্ট্রাক্টের গতি একসাথে পাওয়া যায়।

---

### ঘ. Tuple-এর প্রকারভেদ (Tuple বনাম ValueTuple)

| বিষয় | `System.Tuple` (পুরাতন - C# 4) | `ValueTuple` (আধুনিক - C# 7+) |
| :--- | :--- | :--- |
| **টাইপ** | **Reference Type (Class)** — Heap | **Value Type (Struct)** — Stack |
| **মিউটেবিলিটি** | **Immutable** | **Mutable** |
| **ফিল্ড নাম** | `Item1`, `Item2` (কোনো নাম দেওয়া যায় না) | `(int Id, string Name)` নাম দেওয়া যায় |
| **সিনট্যাক্স** | `Tuple.Create(1, "A")` | `(1, "A")` |

---

### ঙ. Delegate-এর প্রকারভেদ

1. **Custom Delegate:** ইউজার নিজের তৈরি ডেলিগেট (`delegate void MyHandler()`)।
2. **Built-in Generic Delegates:**
   - **`Action<T>`:** কোনো রিটার্ন টাইপ নেই (`void`) এমন মেথডকে পয়েন্ট করে।
   - **`Func<T, TResult>`:** কোনো মান রিটার্ন করে এমন মেথডকে পয়েন্ট করে।
   - **`Predicate<T>`:** শুধুমাত্র `bool` রিটার্ন করে এমন মেথডকে পয়েন্ট করে।

---

### চ. Enum-এর প্রকারভেদ

1. **Simple Enum:** যেকোনো একটি অপশন সিলেক্ট করা যায় (`enum Status { Active, Inactive }`)।
2. **Flags Enum (`[Flags]`):** বিটওয়াইজ অপারেশন দিয়ে একাধিক অপশন এক সাথে নির্বাচন করা যায় (`Permissions.Read | Permissions.Write`)।

---

### ছ. Anonymous Type
* **এর কোনো প্রকারভেদ বা সাব-টাইপ নেই।**
* `new { Name = "X", Roll = 101 }` লিখলে C# কম্পাইলার ব্যাকগ্রাউন্ডে স্বয়ংক্রিয়ভাবে একটি প্রাইভেট জেনেরিক রিড-অনলি ক্লাস জেনারেট করে দেয়।

---

### ভাইবা রিভিশন টেবিল (Quick Recap)

| কনসেপ্ট | স্ট্যাক নাকি হিপ? | ভ্যালু না রেফারেন্স সমতা? | মূল ব্যবহার |
| :--- | :---: | :---: | :--- |
| **Class** | Heap | Reference Equality | অবজেক্ট ওরিয়েন্টেড বড় লজিক |
| **Record Class** | Heap | Value Equality | DTO, API Models, CQRS |
| **Struct** | Stack | Value Equality | হালকা ডেটা, Coordinates, Math |
| **Record Struct** | Stack | Value Equality | ভ্যালু টাইপ DTO |
| **ValueTuple** | Stack | Value Equality | মেথড থেকে মাল্টিপল মান রিটার্ন |
| **Interface** | Heap Reference | N/A | আর্কিটেকচারাল কন্ট্রাক্ট / DI |
