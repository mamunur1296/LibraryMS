# 👑 C# Object-Oriented Programming (OOP) Ultimate Master Checklist (100% Complete)

এই ডকুমেন্টে C# এবং .NET-এর অবজেক্ট-ওরিয়েন্টেড প্রোগ্রামিং (OOP)-এর সমস্ত টপিক অন্তর্ভুক্ত করা হয়েছে। ফাউন্ডেশন থেকে শুরু করে অ্যাডভান্সড মেটাপ্রোগ্রামিং, মেমরি ইন্টারনালস, VMT/VSD মেকানিজম এবং আধুনিক C# ১৩/১৪-এর লেটেস্ট কনসেপ্ট পর্যন্ত—**এটি C# OOP-এর চূড়ান্ত ও সম্পূর্ণ সিলেবাস**।

---

## 📌 সূচিপত্র (Table of Contents)
1. [Paradigm Foundation & Class Basics](#1-paradigm-foundation--class-basics)
2. [Encapsulation & Data Hiding (১ম স্তম্ভ)](#2-encapsulation--data-hiding-১ম-স্তম্ভ)
3. [Inheritance & Code Reusability (২য় স্তম্ভ)](#3-inheritance--code-reusability-২য়-স্তম্ভ)
4. [Polymorphism (৩য় স্তম্ভ)](#4-polymorphism-৩য়-স্তম্ভ)
5. [Abstraction & Contracts (৪র্থ স্তম্ভ)](#5-abstraction--contracts-৪র্থ-স্তম্ভ)
6. [Modern Type Paradigms (Advanced OOP Types)](#6-modern-type-paradigms-advanced-oop-types)
7. [Object Relationships & Associations](#7-object-relationships--associations)
8. [Events, Delegates & Decoupled Communication](#8-events-delegates--decoupled-communication)
9. [Memory Management & Object Internals](#9-memory-management--object-internals)
10. [OOP Architecture: SOLID Principles](#10-oop-architecture-solid-principles)
11. [Core OOP Design Patterns (GoF Patterns in C#)](#11-core-oop-design-patterns-gof-patterns-in-c)
12. [Modern C# Additions Enhancing OOP](#12-modern-c-additions-enhancing-oop)
13. [Metaprogramming, Reflection & Type Metadata](#13-metaprogramming-reflection--type-metadata)
14. [Object Equality, Cloning & Comparison Contract](#14-object-equality-cloning--comparison-contract)
15. [Advanced Polymorphism & Method Binding Mechanisms](#15-advanced-polymorphism--method-binding-mechanisms)
16. [Immutability & Modern State Control](#16-immutability--modern-state-control)
17. [Extreme Low-Level & High-Performance OOP](#17-extreme-low-level--high-performance-oop)
18. [OOP-Functional Hybrid & Latest C# 13/14 Additions](#18-oop-functional-hybrid--latest-c-1314-additions)

---

### 1. Paradigm Foundation & Class Basics
1. **Procedural vs Object-Oriented Paradigm**
2. **Class** (The Blueprint)
   1. Concrete Class
   2. Static Class
   3. Abstract Class
   4. Sealed Class
   5. Partial Class (`partial`)
   6. Nested / Inner Class
   7. Generic Class (`class Repository<T>`)
3. **Object** (The Instance)
   1. Object Instantiation (`new`, target-typed `new()`)
   2. Object Lifecycle (Allocation, Usage, De-allocation)
   3. Object Identity vs Equality (`ReferenceEquals` vs `Equals`)
   4. Anonymous Types (`new { Name = "X" }`)
4. **Reference Types vs Value Types in OOP**
   1. Class (Reference type on Heap)
   2. Struct (Value type on Stack / Heap-embedded)
   3. `readonly struct`, `ref struct`
   4. Record Class vs Record Struct (Value-based equality)

---

### 2. Encapsulation & Data Hiding (১ম স্তম্ভ)
1. **Access Modifiers**
   1. `public`
   2. `private`
   3. `protected`
   4. `internal`
   5. `protected internal`
   6. `private protected`
   7. `file` scoped type access modifier (C# 11+)
2. **Fields**
   1. Instance Fields
   2. Static Fields
   3. `readonly` Fields (Runtime constant)
   4. `const` Fields (Compile-time constant)
   5. Volatile Fields (`volatile`)
3. **Properties**
   1. Full Property (Backing Field সহ)
   2. Auto-Implemented Property
   3. Read-only / Write-only Property
   4. `init`-only Setters (Immutability pattern)
   5. `required` Modifier on Properties (C# 11+)
   6. Expression-Bodied Properties (`=>`)
4. **Constructors**
   1. Default Constructor
   2. Parameterized Constructor
   3. Copy Constructor
   4. Static Constructor (`static ClassName()`)
   5. Private Constructor (Singleton Pattern-এ ব্যবহৃত)
   6. Constructor Overloading
   7. Constructor Chaining (`this()` and `base()`)
   8. Primary Constructors (C# 12+)
5. **Destructors & Finalizers**
   1. Finalizer Syntax (`~ClassName()`)
   2. Finalize vs Dispose Pattern (`IDisposable`, `IAsyncDisposable`)
   3. `GC.SuppressFinalize`
6. **Methods**
   1. Instance vs Static Methods
   2. Method Parameters:
      - Value Parameters (`pass-by-value`)
      - Reference Parameters (`ref`)
      - Output Parameters (`out`)
      - Input-only Reference Parameters (`in`)
      - Parameter Arrays (`params`)
      - Optional & Named Parameters
   3. Method Overloading (Compile-time Polymorphism)
   4. Extension Methods
   5. Local Functions
7. **Indexers** (`this[int index]`)
   1. Single & Multi-dimensional Indexers
   2. Overloading Indexers

---

### 3. Inheritance & Code Reusability (২য় স্তম্ভ)
1. **Inheritance Mechanics**
   1. Single Class Inheritance (C# একাধিক ক্লাস ইনহেরিটেন্স সমর্থন করে না)
   2. Base Class vs Derived / Child Class
   3. `base` Keyword Usage (`base.Method()`, `base(args)`)
   4. The `System.Object` Root Hierarchy (`ToString`, `GetHashCode`, `GetType`, `Equals`)
2. **Class Modifiers in Inheritance**
   1. `sealed` Class (ইনহেরিটেন্স বন্ধ করতে)
   2. `sealed` Method (ওভাররাইডিং বন্ধ করতে)
   3. `abstract` Class (বেস ক্লাস টেমপ্লেট)
3. **Shadowing / Hiding Members**
   1. The `new` Modifier on Members
   2. Shadowing vs Overriding

---

### 4. Polymorphism (৩য় স্তম্ভ)
1. **Static / Compile-Time Polymorphism**
   1. Method Overloading
   2. Operator Overloading (`operator +`, `operator ==`, ইত্যাদি)
   3. Conversion Operators (`implicit`, `explicit`)
2. **Dynamic / Runtime Polymorphism**
   1. `virtual` Methods & Properties
   2. `override` Methods & Properties
   3. Virtual Method Table (VMT / VTable) Internals
3. **Abstract Members**
   1. Abstract Methods
   2. Abstract Properties & Indexers
4. **Casting & Type Checking in Polymorphic Hierarchies**
   1. Upcasting (Implicit & Safe)
   2. Downcasting (Explicit)
   3. Type-Testing Operators:
      - `is` Operator (Pattern matching)
      - `as` Operator (Safe casting with null)
      - `typeof()` vs `GetType()`
5. **Covariance & Contravariance**
   1. Generic Covariance (`out T`)
   2. Generic Contravariance (`in T`)
   3. Delegate & Method Group Variance

---

### 5. Abstraction & Contracts (৪র্থ স্তম্ভ)
1. **Abstract Classes vs Interfaces**
   1. কখন Abstract Class বনাম কখন Interface ব্যবহার করবেন
2. **Interfaces**
   1. Interface Declaration & Syntax
   2. Explicit Interface Implementation (`IFoo.Method()`)
   3. Implicit Interface Implementation
   4. Multiple Interface Implementation
   5. Interface Inheritance
   6. Default Interface Methods (DIM - C# 8+)
   7. Static Abstract Members in Interfaces (Generic Math - C# 11+)
3. **Loose Coupling & Separation of Concerns**

---

### 6. Modern Type Paradigms (Advanced OOP Types)
1. **Records (C# 9+)**
   1. Positional Records (`record Person(string Name)`)
   2. Non-destructive Mutation (`with` expression)
   3. Value-based Equality Semantics
   4. `record class` vs `record struct`
2. **Structs & Memory Layout in OOP**
   1. Memory Layout (Sequential, Auto, Explicit)
   2. Boxing and Unboxing Overhead
3. **Tuples vs Objects**
   1. ValueTuple
   2. Deconstruction (`Deconstruct()` method)

---

### 7. Object Relationships & Associations
1. **Inheritance Relationship ("IS-A")**
   - Dog **is-a** Animal
2. **Composition ("HAS-A" - Strong Ownership)**
   - House **has-a** Room (House ধ্বংস হলে Room ধ্বংস হয়)
3. **Aggregation ("HAS-A" - Weak Ownership)**
   - Department **has-a** Teacher (Department বন্ধ হলেও Teacher জীবিত থাকে)
4. **Association ("USES-A")**
   - Doctor **uses-a** Patient
5. **Dependency ("DEPENDS-ON")**
   - Method Parameter হিসেবে অন্য ক্লাসের অবজেক্ট রিসিভ করা

---

### 8. Events, Delegates & Decoupled Communication
1. **Delegates** (Type-safe Function Pointers)
   1. Single-cast Delegate
   2. Multicast Delegate
   3. Built-in Generic Delegates: `Action<T>`, `Func<T, TResult>`, `Predicate<T>`
2. **Events**
   1. Publisher-Subscriber Pattern
   2. `event` Keyword
   3. `EventHandler` and `EventHandler<TEventArgs>`
   4. Custom Event Accessors (`add`, `remove`)

---

### 9. Memory Management & Object Internals
1. **Object Memory Layout in CLR**
   1. Object Header (SyncBlock Index - 4/8 bytes)
   2. MethodTable Pointer (Type Handle - 4/8 bytes)
   3. Fields Payload & Alignment/Padding
2. **Garbage Collection (GC) Lifecycle for Objects**
   1. Root References
   2. Generations (Gen 0, Gen 1, Gen 2)
   3. LOH (Large Object Heap) & POH (Pinned Object Heap)
3. **Unmanaged Resources & Disposal**
   1. Dispose Pattern (`IDisposable`)
   2. Async Disposal (`IAsyncDisposable`)
   3. `using` Statement and `using` Declaration
   4. SafeHandle Wrapper

---

### 10. OOP Architecture: SOLID Principles
1. **S - Single Responsibility Principle (SRP)**
2. **O - Open/Closed Principle (OCP)**
3. **L - Liskov Substitution Principle (LSP)**
4. **I - Interface Segregation Principle (ISP)**
5. **D - Dependency Inversion Principle (DIP)**
   - Inversion of Control (IoC)
   - Dependency Injection (DI): Constructor Injection, Property Injection

---

### 11. Core OOP Design Patterns (GoF Patterns in C#)
1. **Creational Patterns**
   1. Singleton Pattern (Thread-safe, Lazy)
   2. Factory Method Pattern
   3. Abstract Factory Pattern
   4. Builder Pattern
   5. Prototype Pattern
2. **Structural Patterns**
   1. Adapter Pattern
   2. Decorator Pattern
   3. Facade Pattern
   4. Proxy Pattern
   5. Composite Pattern
3. **Behavioral Patterns**
   1. Strategy Pattern
   2. Observer Pattern
   3. Repository & Unit of Work Pattern (Enterprise C#)
   4. Command Pattern
   5. Template Method Pattern

---

### 12. Modern C# Additions Enhancing OOP
1. **Pattern Matching**
   1. Type Pattern (`if (obj is Dog d)`)
   2. Property Pattern
   3. Positional Pattern
   4. Switch Expressions
2. **Nullable Reference Types (NRT)**
   1. `string?` vs `string`
   2. Null-Forgiving Operator (`!`)
   3. Null-Coalescing (`??`, `??=`)
   4. Null-Conditional (`?.`)
3. **Generics Constraints (`where T : ...`)**
   1. `where T : class` (Reference type constraint)
   2. `where T : struct` (Value type constraint)
   3. `where T : new()` (Default constructor constraint)
   4. `where T : BaseClass`
   5. `where T : IInterface`
   6. `where T : notnull`

---

### 13. Metaprogramming, Reflection & Type Metadata
1. **Reflection & Type Inspection**
   1. `System.Type` ও `typeof()` বনাম `GetType()`
   2. Dynamic Member Invocation (রানটাইমে মেথড ও প্রোপার্টি রিড/ইনভোক করা)
   3. `Activator.CreateInstance` (রানটাইমে ডাইনামিক অবজেক্ট ক্রিয়েশন)
2. **Attributes (Custom Metadata on OOP Entities)**
   1. Applying Attributes to Classes, Methods, Properties (`[AttributeUsage]`)
   2. Writing Custom Attributes
3. **C# Source Generators** (কম্পাইল টাইমে স্বয়ংক্রিয়ভাবে ক্লাস ও মেথড তৈরি করা)

---

### 14. Object Equality, Cloning & Comparison Contract
1. **Shallow Copy vs Deep Copy**
   1. `MemberwiseClone()`
   2. `ICloneable` ইন্টারফেস ও এর সমস্যাসমূহ
   3. Serialization দিয়ে Deep Copy (বা `record`-এর `with` এক্সপ্রেশন)
2. **The Equality Contract in C#**
   1. `IEquatable<T>` (বক্সিং ছাড়া ফাস্ট ইকুয়ালিটি)
   2. `IEqualityComparer<T>`
   3. `IComparable<T>` এবং `IComparer<T>` (অবজেক্ট সর্টিং অর্ডার নির্ধারণ)
   4. `GetHashCode()` ওভাররাইড করার নিয়ম ও হ্যাশ টেবিল বাকেট মেকানিক্স

---

### 15. Advanced Polymorphism & Method Binding Mechanisms
1. **Dynamic Polymorphism Internals**
   1. **VMT (Virtual Method Table):** CLR মেমরিতে মেথড কল কীভাবে মেপ করে।
   2. **Interface Dispatch:** Virtual Stub Dispatch (VSD) — ইন্টারফেস কল কীভাবে অপ্টিমাইজ হয়।
2. **Static Abstract Members in Interfaces (C# 11+)**
   1. অপারেটরকে ইন্টারফেসে ডিফাইন করা (`static abstract T operator +(T a, T b)`)
   2. Generic Math
3. **Static Virtual Members**

---

### 16. Immutability & Modern State Control
1. **Immutable Objects**
   1. Readonly Classes & Fields
   2. `init`-only setters
   3. `ImmutableArray<T>`, `ImmutableList<T>` ইত্যাদি Immutable Collections
2. **Primary Constructors on Classes & Structs (C# 12+)**
   1. ক্লাসের প্যারামিটারে সরাসরি কনস্ট্রাক্টর ডিক্লেয়ারেশন
   2. ক্যাপচারড ফিল্ড ও স্কোপিং রুলস

---

### 17. Extreme Low-Level & High-Performance OOP
1. **`ref struct` & Stack-Only Objects**
   1. কেন এরা হিপে যেতে পারে না (No Boxing, No Interfaces)
   2. `Span<T>` ও `ReadOnlySpan<T>`
2. **`scoped ref` & Lifetime Rules** (মেথড স্কোপের বাইরে রেফারেন্স লিক রোধ)
3. **Object Pooling Pattern (`ArrayPool<T>`, `ObjectPool<T>`)**
   - ঘন ঘন অবজেক্ট তৈরি ও ধ্বংস না করে রিসাইকেল করা (GC প্রেশার দূর করতে)

---

### 18. OOP-Functional Hybrid & Latest C# 13/14 Additions
1. **Functional-OOP Hybrid**
   1. Expressions & Lambda Functions as Object Delegates
   2. Pattern Matching with Discriminated Unions স্টাইল (C# Records ও Type Patterns দিয়ে)
2. **C# 13 & 14 Latest OOP Additions**
   1. `params` Collections (Array ছাড়াও `ReadOnlySpan<T>` বা `List<T>` সাপোর্ট)
   2. Field-targeted attributes
   3. Extended `partial` properties and events
