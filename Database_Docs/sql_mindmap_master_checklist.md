# 🧠 SQL & Database Mind Map Master Checklist (Complete Self-Test Q&A)

আপনার দেওয়া সম্পূর্ণ SQL মাইন্ডম্যাপের (Mind Map) ১৩টি মূল শাখা ও তার শত শত সাব-টপিকগুলোকে রিভিশন ও সেলফ-টেস্টের জন্য পয়েন্ট-বাই-পয়েন্ট সহজ বাংলায় এখানে সাজিয়ে দেওয়া হলো।

---

## 📌 সূচিপত্র (Table of Contents)

1. [১. SQL and Database Fundamentals](#১-sql-and-database-fundamentals)
2. [২. SQL Syntax Fundamentals](#২-sql-syntax-fundamentals)
3. [৩. Databases and Schemas](#৩-databases-and-schemas)
4. [৪. SQL Data Types](#৪-sql-data-types)
5. [৫. Keys and Constraints](#৫-keys-and-constraints)
6. [৬. Table Creation & Data Manipulation](#৬-table-creation--data-manipulation)
7. [৭. SELECT Fundamentals](#৭-select-fundamentals)
8. [৮. Operators and Expressions](#৮-operators-and-expressions)
9. [৯. Aggregate Functions & Joins](#৯-aggregate-functions--joins)
10. [১০. Built-in Functions](#১০-built-in-functions)
11. [১১. Subqueries & Set Operations](#১১-subqueries--set-operations)
12. [১২. Views, Indexes & Transactions](#১২-views-indexes--transactions)
13. [১৩. Advanced SQL (Only for Data/SQL Engineers)](#১৩-advanced-sql-only-for-datasql-engineers)

---

# ১. SQL and Database Fundamentals

### ১.১ Database Fundamentals
- [ ] **Schema (স্কিমা):** পুরো ডেটাবেসের লজিক্যাল ডিজাইন, কাঠামো বা ব্লুপ্রিন্ট (টেবিল, কলাম, ভিউ ও নিয়মের নকশা)।
- [ ] **Table (টেবিল):** ডেটাবেসে ডেটা সংরক্ষণের মূল কাঠামো, যা অনুভূমিক সারি (Rows) এবং উলম্ব কলাম (Columns) নিয়ে গঠিত।
- [ ] **Row (রো / সারি):** টেবিলের একটি একক অনুভূমিক লাইন।
- [ ] **Record (রেকর্ড):** একজন নির্দিষ্ট ব্যক্তি বা বস্তুর পূর্ণাঙ্গ তথ্যের সেট (একটি Row-ই হলো একটি Record)।
- [ ] **Column (কলাম):** টেবিলের একটি একক উলম্ব দাগ বা বিভাগ, যাতে নির্দিষ্ট ধরনের ডেটা থাকে।
- [ ] **Field (ফিল্ড):** একটি নির্দিষ্ট Row এবং Column-এর মিলনস্থল বা একক সেল (যেমন: রহিমের বয়সের সেল `25`)।
- [ ] **Attribute (অ্যাট্রিবিউট):** একটি এনটিটির নির্দিষ্ট কোনো বৈশিষ্ট্য (যেমন: `Name`, `Email`, `Price`)।
- [ ] **Entity (এনটিটি):** বাস্তব জগতের যেকোনো ব্যক্তি, বস্তু বা ঘটনা যার ডেটা আমরা সেভ করতে চাই (যেমন: `Student`, `Book`)।
- [ ] **Relationship (রিলেশনশিপ):** দুটি বা তার বেশি টেবিলের মধ্যকার লজিক্যাল সংযোগ বা সম্পর্ক (Primary ও Foreign Key দিয়ে তৈরি)।
- [ ] **Metadata (মেটাডেটা):** Data about Data — অর্থাৎ ডেটা সম্পর্কিত তথ্য (যেমন: কলামের ডেটাটাইপ, সাইজ, তৈরির সময়)।
- [ ] **Query (কোয়েরি):** ডেটাবেস থেকে ডেটা খোঁজা, ফিল্টার বা পরিবর্তন করার জন্য দেওয়া SQL নির্দেশ।
- [ ] **RDBMS (রিলেশনাল ডিবিএমএস):** রিলেশনাল মডেলভিত্তিক সফটওয়্যার যা সম্পর্কিত টেবিল, কিজ (Keys) এবং SQL দিয়ে পরিচালিত হয় (যেমন: SQL Server, MySQL)।

### ১.২ Basic Terminology
- [ ] **Degree of a table (ডিগ্রি):** একটি টেবিলে মোট কয়টি কলাম (Attributes) আছে তার মোট সংখ্যা।
- [ ] **Cardinality of a table (কার্ডিনালিটি):** একটি টেবিলে মোট কয়টি রো (Tuples / Records) আছে তার মোট সংখ্যা।
- [ ] **Empty table (খালি টেবিল):** যে টেবিলে কলাম বা স্কিমা তৈরি আছে কিন্তু কোনো ডেটা বা রো নেই (Cardinality = 0)।
- [ ] **Duplicate rows (ডুপ্লিকেট রো):** টেবিলে থাকা একাধিক সারি যাদের ভেতরের সমস্ত কলামের মান হুবহু এক।
- [ ] **NULL values (নাল মান):** ডেটাবেসে কোনো ঘরে মান অনুপস্থিত (Missing), অজানা (Unknown) বা অপ্রযোজ্য হওয়া (`0` বা স্পেস নয়)।

### ১.৩ Types of Relationships
- [ ] **One-to-One Relationship (1 : 1):** ১ম টেবিলের ১টি রেকর্ড ২য় টেবিলের মাত্র ১টি রেকর্ডের সাথেই যুক্ত থাকে (যেমন: User $\leftrightarrow$ UserPassport)।
- [ ] **One-to-Many Relationship (1 : N):** ১ম টেবিলের ১টি রেকর্ড ২য় টেবিলের একাধিক রেকর্ডের সাথে যুক্ত থাকে (যেমন: Department $\rightarrow$ Employees)।
- [ ] **Many-to-One Relationship (N : 1):** বহু রেকর্ড ১ম টেবিলের ১টি মাত্র প্যারেন্ট রেকর্ডের সাথে যুক্ত থাকে (One-to-Many এর বিপরীত দিক)।
- [ ] **Many-to-Many Relationship (M : N):** ১ম টেবিলের বহু রেকর্ড ২য় টেবিলের বহু রেকর্ডের সাথে যুক্ত থাকে (জাংশন টেবিলের মাধ্যমে হ্যান্ডেল করা হয়, যেমন: Students $\leftrightarrow$ Courses)।
- [ ] **Self-Referencing Relationship:** যখন একটি টেবিল নিজের ভেতরেই নিজের প্রাইমারি কী-কে Foreign Key হিসেবে রেফারেন্স করে (যেমন: Employees টেবিলে `ManagerID` কলামটি একই টেবিলের `EmpID`-কে নির্দেশ করে)।

### ১.৪ Types of SQL Commands
- [ ] **DQL (Data Query Language):** `SELECT` — ডেটাবেস থেকে কোনো পরিবর্তন না করে শুধু তথ্য পড়া ও খুঁজে বের করা।
- [ ] **DML (Data Manipulation Language):** `INSERT`, `UPDATE`, `DELETE` — টেবিলে ডেটা তৈরি, পরিবর্তন বা মুছে ফেলা।
- [ ] **DDL (Data Definition Language):** `CREATE`, `ALTER`, `DROP`, `TRUNCATE`, `RENAME` — টেবিল বা স্কিমার কাঠামো নিয়ে কাজ করা।
- [ ] **TCL (Transaction Control Language):** `START TRANSACTION`, `COMMIT`, `ROLLBACK`, `SAVEPOINT`, `RELEASE SAVEPOINT` — লেনদেন নিরাপত্তা নিয়ন্ত্রণ।
- [ ] **DCL (Data Control Language):** `GRANT`, `REVOKE` — ইউজার পারমিশন ও সিকিউরিটি দেওয়া বা কেড়ে নেওয়া।

---

# ২. SQL Syntax Fundamentals

### ২.১ SQL Statement Structure
- [ ] **SQL Keywords:** SQL-এর আগে থেকে সংরক্ষিত নির্দিষ্ট নির্দেশনামূলক শব্দ (যেমন: `SELECT`, `FROM`, `WHERE`)।
- [ ] **SQL Clauses:** একটি সম্পূর্ণ স্টেটমেন্টের এক একটি অংশ যা নির্দিষ্ট কাজ করে (যেমন: `WHERE Age > 20`, `ORDER BY Name DESC`)।
- [ ] **SQL Statements:** এক বা একাধিক ক্লজ মিলিয়ে তৈরি হওয়া একটি সম্পূর্ণ নির্দেশ (`SELECT * FROM Users WHERE ID = 1;`)।
- [ ] **Statement Terminator:** একটি নির্দেশ শেষ বোঝাতে ব্যবহৃত সেমিকোলন (`;`) চিহ্ন।
- [ ] **Whitespace:** কোডের ভেতরে থাকা স্পেস, ট্যাব বা নতুন লাইন যা কোডের রিডিবিলিটি বাড়ায়।

### ২.২ Comments
- [ ] **Single-line comments:** দুটি ড্যাশ (`--`) দিয়ে শুরু হওয়া এক লাইনের কমেন্ট।
- [ ] **Multi-line comments:** `/* ... */` দিয়ে শুরু ও শেষ হওয়া একাধিক লাইনের কমেন্ট।
- [ ] **Inline comments:** একই লাইনে কোডের মাঝখানে মন্তব্য লেখার পদ্ধতি (`SELECT Name, /* comment */ Age FROM Users;`)।

### ২.৩ Identifiers
- [ ] **Database Identifiers:** ডেটাবেসকে চিহ্নিত করার ইউনিক নাম (`LibraryDB`)।
- [ ] **Table Identifiers:** টেবিলের নাম (`Students`, `Orders`)।
- [ ] **Column Identifiers:** কলামের নাম (`FirstName`, `Salary`)।
- [ ] **Alias Identifiers:** কোয়েরির রেজাল্ট সুন্দর দেখানোর সাময়িক নাম (`AS TotalCost`)।
- [ ] **Reserved Keywords as Identifiers:** সংরক্ষিত শব্দ ব্যবহারে ব্র্যাকেট বা ব্যাকটিক দেওয়া (`[Order]` বা `` `User` ``)।

### ২.৪ SQL Literals
- [ ] **Numeric Literals:** সাধারণ সংখ্যাগত মান (`42`, `100.50`)।
- [ ] **Integer Literals:** পূর্ণসংখ্যা (`10`, `-25`)।
- [ ] **Decimal Literals:** নির্দিষ্ট দশমিক সম্বলিত টাকার মান (`99.99`)।
- [ ] **Floating-point Literals:** বৈজ্ঞানিক ভগ্নাংশ সংখ্যা (`3.14159`)।
- [ ] **String Literals:** কোটেশনের ভেতরে থাকা টেক্সট (`'Rahim Ahmed'`)।
- [ ] **Date Literals:** তারিখের মান (`'2026-08-29'`)।
- [ ] **Time Literals:** সময়ের মান (`'14:30:00'`)।
- [ ] **Date-time Literals:** তারিখ ও সময় একসাথে (`'2026-08-29 14:30:00'`)।
- [ ] **Boolean Values:** সত্য বা মিথ্যা মান (`TRUE`/`FALSE` বা `1`/`0`)।
- [ ] **Binary Literals:** বাইনারি ডেটা যা `0x` দিয়ে শুরু হয় (`0x4A6F686E`)।
- [ ] **Hexadecimal Literals:** হেক্সাডেসিমেল ফরম্যাটের মান (`0xFF`)।
- [ ] **NULL Literal:** কোনো মান নেই বোঝাতে ব্যবহৃত স্পেশাল কি-ওয়ার্ড (`NULL`)।

### ২.৫ Quotes & Escaping
- [ ] **Single quotes (`' '`):** সমস্ত টেক্সট, স্ট্রিং বা ডেট লিটারাল লেখার জন্য ব্যবহৃত হয়।
- [ ] **Double quotes (`" "`):** কলাম বা টেবিলের নামে স্পেস থাকলে কোটেড আইডেন্টিফায়ার হিসেবে ব্যবহৃত হয়।
- [ ] **Backticks (`` ` ` ``):** MySQL ডেটাবেসে টেবিল বা কলামের নাম র‍্যাপ করার জন্য ব্যবহৃত হয়।
- [ ] **Escaping quotes:** টেক্সটের ভেতরে অ্যাপোস্ট্রফি থাকলে পরপর দুটি সিঙ্গেল কোটেশন দেওয়া (`'O''Reilly'`)।
- [ ] **Escape characters:** স্পেশাল ক্যারেক্টার যেমন ব্যাকস্ল্যাশ (`\n`, `\t`)।
- [ ] **Quoted Identifiers:** স্কয়ার ব্র্যাকেট বা কোটেশন দিয়ে অবজেক্ট নাম লেখা (`[First Name]`)।
- [ ] **Quoted string values:** টেক্সট ডেটাকে কোটেশনের মধ্যে আবদ্ধ করা (`'Hello World'`)।

---

# ৩. Databases and Schemas

### ৩.১ Database Commands
- [ ] **`SHOW DATABASES`:** সার্ভারে থাকা সমস্ত ডেটাবেসের তালিকা দেখতে।
- [ ] **`CREATE DATABASE`:** নতুন ডেটাবেস তৈরি করতে।
- [ ] **`CREATE DATABASE IF NOT EXISTS`:** ডেটাবেস না থাকলে তৈরি করবে (এরর ছাড়া)।
- [ ] **`USE`:** নির্দিষ্ট ডেটাবেস সিলেক্ট বা সক্রিয় করতে।
- [ ] **`SELECT DATABASE()`:** বর্তমান সক্রিয় ডেটাবেসের নাম দেখতে।
- [ ] **`ALTER DATABASE`:** ডেটাবেসের কনফিগারেশন বা কোলেশন পরিবর্তন করতে।
- [ ] **`DROP DATABASE`:** ডেটাবেস সমস্ত ডেটাসহ চিরতরে মুছে ফেলতে।
- [ ] **`DROP DATABASE IF EXISTS`:** ডেটাবেস থাকলে ডিলিট করবে, না থাকলে এরর দেবে না।

### ৩.২ Table Commands
- [ ] **`CREATE TABLE`:** নতুন টেবিল তৈরি করতে।
- [ ] **`ALTER TABLE`:** টেবিলের কাঠামো পরিবর্তন করতে।
- [ ] **`RENAME`:** টেবিলের নাম পরিবর্তন করতে।
- [ ] **`TRUNCATE`:** টেবিল স্ট্রাকচার রেখে সব ডেটা এক পলকে খালি ও রিসেট করতে।
- [ ] **`DROP`:** টেবিল ডেটাসহ পুরোপুরি মুছে ফেলতে।

### ৩.৩ Database Properties
- [ ] **Default Character Set:** টেক্সট এনকোডিং ফরম্যাট (যেমন: `utf8mb4`)।
- [ ] **Default Collation:** টেক্সট তুলনা ও সাজানোর অ্যালগরিদম (যেমন: `utf8mb4_general_ci`)।
- [ ] **Database Existence:** সার্ভারে কোনো ডেটাবেস আগে থেকেই তৈরি আছে কি না তা যাচাই করার মেকানিজম (`IF EXISTS`)।

### ৩.৪ Inspecting Database Objects
- [ ] **`SHOW TABLES`:** ডেটাবেসের সমস্ত টেবিলের তালিকা দেখতে।
- [ ] **`SHOW FULL TABLES`:** টেবিলের পাশাপাশি ভিউ (View) কি না তাও দেখতে।
- [ ] **`DESCRIBE` / `DESC`:** টেবিলের কলাম, ডেটাটাইপ ও কিজ-এর বিবরণ দেখতে (`DESC Students;`)।
- [ ] **`SHOW COLUMNS`:** কলামগুলোর লিস্ট ও প্রোপার্টি দেখতে।
- [ ] **`SHOW FULL COLUMNS`:** পারমিশন ও কমেন্টসসহ কলামের বিবরণ দেখতে।
- [ ] **`SHOW CREATE DATABASE`:** ডেটাবেস তৈরির মূল SQL কোড দেখতে।
- [ ] **`SHOW CREATE TABLE`:** টেবিল তৈরির মূল `CREATE TABLE` কোড দেখতে।

### ৩.৫ Modifying Table Structure
- [ ] **Add Column:** নতুন কলাম যোগ করা (`ALTER TABLE Students ADD Age INT;`)।
- [ ] **Modify Column:** বিদ্যমান কলামের ডেটাটাইপ পরিবর্তন করা (`ALTER TABLE Students MODIFY Age TINYINT;`)।
- [ ] **Rename Column:** কলামের নাম পরিবর্তন করা (`ALTER TABLE Students RENAME COLUMN OldName TO NewName;`)।
- [ ] **Drop Column:** টেবিল থেকে কলাম মুছে ফেলা (`ALTER TABLE Students DROP COLUMN Age;`)।
- [ ] **Reorder Column:** কলামের পজিশন আগে-পিছে করা (`AFTER Column1` বা `FIRST`)।

---

# ৪. SQL Data Types

### ৪.১ Integer Data Types
- [ ] **`TINYINT`:** ১ বাইট (`-128` থেকে `127`, Unsigned: `0` থেকে `255`)।
- [ ] **`SMALLINT`:** ২ বাইট (`-32,768` থেকে `32,767`)।
- [ ] **`MEDIUMINT`:** ৩ বাইট (প্রায় `±৮.৩ মিলিয়ন`, MySQL স্পেশাল)।
- [ ] **`INT` / `INTEGER`:** ৪ বাইট (সবচেয়ে বেশি ব্যবহৃত, প্রায় `±২.১৪ বিলিয়ন`)।
- [ ] **`BIGINT`:** ৮ বাইট (বিশাল স্কেল, প্রায় `±৯ কুইন্টিলিয়ন`)।
- [ ] **Signed & Unsigned:** Signed মানে নেগেটিভ+পজিটিভ; Unsigned মানে শুধু পজিটিভ (রেঞ্জ দ্বিগুণ)।

### ৪.২ Decimal & Floating-point Types
- [ ] **`DECIMAL` / `NUMERIC`:** ফিক্সড প্রিসিশন দশমিক সংখ্যা। **টাকা বা মূল্যের জন্য সেরা**।
- [ ] **`FLOAT`:** আনুমানিক ভগ্নাংশ সংখ্যা (Approximate scientific value)।
- [ ] **`DOUBLE`:** দ্বিগুণ প্রিসিশন সম্বলিত বড় বৈজ্ঞানিক দশমিক সংখ্যা (৮ বাইট)।
- [ ] **Precision and Scale:** `DECIMAL(10, 2)` $\rightarrow$ মোট ১০টি সংখ্যার মধ্যে দশমিকের পর ২টি থাকবে।

### ৪.৩ Character Data Types
- [ ] **`CHAR`:** ফিক্সড দৈর্ঘ্যের স্ট্রিং (`CHAR(2)` = 'BD')।
- [ ] **`VARCHAR`:** পরিবর্তনশীল দৈর্ঘ্যের স্ট্রিং (`VARCHAR(100)` $\rightarrow$ যতটুকু টেক্সট ততটুকু মেমোরি)।
- [ ] **`TEXT`:** বিশাল আকারের আর্টিকেল বা ডেসক্রিপশন (64 KB থেকে 4 GB পর্যন্ত)।

### ৪.৪ Binary Data Types
- [ ] **`BINARY`:** ফিক্সড সাইজের কাঁচা বাইনারি বাইট।
- [ ] **`VARBINARY`:** পরিবর্তনশীল সাইজের বাইনারি ডেটা (পাসওয়ার্ড হ্যাশ বা সিক্রেট কি)।
- [ ] **`BLOB`:** ছবি, অডিও, ভিডিও বা পিডিএফ ফাইল সংরক্ষণের টাইপ।

### ৪.৫ Bit & Boolean Types
- [ ] **`BIT`:** সিঙ্গেল বিট ডেটাটাইপ (`1` বা `0`)।
- [ ] **`BOOLEAN` / `BOOL`:** সত্য বা মিথ্যা ফ্ল্যাগ (`TRUE = 1`, `FALSE = 0`)।

### ৪.৬ Date & Time Data Types
- [ ] **`DATE`:** শুধুমাত্র তারিখ (`YYYY-MM-DD`, যেমন: `2026-08-29`)।
- [ ] **`TIME`:** শুধুমাত্র সময় (`HH:MM:SS`)।
- [ ] **`DATETIME`:** তারিখ ও সময় একসাথে (`YYYY-MM-DD HH:MM:SS`)।
- [ ] **`TIMESTAMP`:** টাইমজোন ভিত্তিক স্বয়ংক্রিয় সময় (UTC কনভার্ট হয়ে সেভ হয়)।
- [ ] **`YEAR`:** শুধুমাত্র ৪ ডিজিটের বছর (`2026`)।

### ৪.৭ Special String Types
- [ ] **`ENUM`:** নির্দিষ্ট অপশন থেকে একটি বেছে নেওয়ার টাইপ (`ENUM('Male', 'Female')`)।
- [ ] **`SET`:** অপশন থেকে একাধিক মান বেছে নেওয়ার টাইপ (`SET('Sports', 'Coding')`)।
- [ ] **`JSON`:** সরাসরি ভ্যালিডেটেড JSON অবজেক্ট সেভ করার টাইপ।
- [ ] **Spatial Data Types:** ম্যাপ লোকেশন ও কোঅর্ডিনেট (`GEOMETRY`, `POINT`)।

### ৪.৮ Character Sets & Collations
- [ ] **Character Set:** এনকোডিং ফরম্যাট (`latin1`, `utf8`)।
- [ ] **Collation:** টেক্সট সাজানো ও তুলনা করার রুলস (`utf8mb4_general_ci`)।
- [ ] **`utf8mb4`:** বাংলা, ইংরেজি এবং ইমোজি (😀) সহ সমস্ত ভাষা শতভাগ সাপোর্ট করে।

### ৪.৯ Data Type Conversion
- [ ] **Implicit Conversion:** ইঞ্জিন নিজে থেকেই স্বয়ংক্রিয়ভাবে টাইপ কনভার্ট করে নিলে।
- [ ] **Explicit Conversion:** ডেভেলপার নিজে ফাংশন দিয়ে কনভার্ট করলে।
- [ ] **`CAST()`:** ANSI স্ট্যান্ডার্ড কনভার্সন (`CAST('100' AS INT)`)।
- [ ] **`CONVERT()`:** কনভার্ট করার সাথে ডেটের স্টাইল ফরম্যাট করার ফাংশন (`CONVERT(VARCHAR, GETDATE(), 103)`).

---

# ৫. Keys and Constraints

### ৫.১ Keys
- [ ] **Super Key:** এক বা একাধিক কলামের কম্বিনেশন যা প্রতিটি রো-কে ইউনিকভাবে চেনে।
- [ ] **Candidate Key:** মিনিমাল সুপার কী যার প্রাইমারি কী হওয়ার যোগ্যতা আছে।
- [ ] **Primary Key:** টেবিলের প্রধান ইউনিক আইডেন্টিফায়ার (কখনো `NULL` বা ডুপ্লিকেট হবে না)।
- [ ] **Alternate Key:** প্রাইমারি কী হিসেবে সিলেক্ট না হওয়া বাকি ক্যান্ডিডেট কী-গুলো।
- [ ] **Foreign Key:** প্যারেন্ট টেবিলের প্রাইমারি কী-কে রেফারেন্স করে সম্পর্ক তৈরি করে।
- [ ] **Composite Key:** একাধিক কলাম একসাথে মিলে তৈরি হওয়া প্রাইমারি কী।
- [ ] **Unique Key:** কলামের সব মান ইউনিক হবে, তবে ১টি মাত্র `NULL` থাকতে পারে।
- [ ] **Natural Key:** বাস্তব জগতে আগে থেকেই বিদ্যমান ইউনিক বৈশিষ্ট্য (`NID`, `Passport`)।
- [ ] **Surrogate Key:** ডেটাবেসের কৃত্রিম অটো-জেনারেটেড আইডি (`Auto_Increment` / `Identity: 1, 2, 3...`)।

### ৫.২ Constraints
- [ ] **`UNIQUE`:** ডুপ্লিকেট মান প্রবেশ বন্ধ করে।
- [ ] **`NOT NULL`:** কলাম ফাঁকা রাখা নিষিদ্ধ করে।
- [ ] **`DEFAULT`:** মান না দিলে স্বয়ংক্রিয় ডিফল্ট মান বসায়।
- [ ] **`CHECK`:** নির্দিষ্ট শর্ত সত্য হলেই ডেটা সেভ হতে দেয় (`CHECK (Age >= 18)`)।

### ৫.৩ Referential Actions
- [ ] **`CASCADE`:** প্যারেন্ট ডিলিট/আপডেট হলে চাইল্ডের ডেটাও অটো ডিলিট/আপডেট হবে।
- [ ] **`SET NULL`:** প্যারেন্ট মুছে ফেললে চাইল্ডের ফরেন কী `NULL` হয়ে যাবে।
- [ ] **`RESTRICT`:** চাইল্ডে ডেটা থাকলে প্যারেন্ট ডিলিট করতে কঠোরভাবে বাধা দেবে।
- [ ] **`NO ACTION`:** ডিফল্ট আচরণ—অপারেশন আটকে দিয়ে এরর ছুড়ে মারবে।
- [ ] **`ON DELETE`:** প্যারেন্ট ডিলিট করার সময়ের অ্যাকশন নির্ধারণ করে।
- [ ] **`ON UPDATE`:** প্যারেন্ট প্রাইমারি কী আপডেট করার সময়ের অ্যাকশন নির্ধারণ করে।

---

# ৬. Table Creation & Data Manipulation

### ৬.১ Creating Tables & Cloning
- [ ] **`CREATE TABLE`:** নতুন টেবিল তৈরি।
- [ ] **`CREATE TABLE IF NOT EXISTS`:** এরর ছাড়া টেবিল তৈরি।
- [ ] **Empty Tables:** শুধু স্কিমা তৈরি করে রাখা।
- [ ] **`TEMPORARY TABLE`:** সেশনভিত্তিক সাময়িক টেবিল।
- [ ] **Column Properties:** `NULL`, `NOT NULL`, `DEFAULT`, `AUTO_INCREMENT`, `UNSIGNED`, এবং `Generated/Computed Columns`।
- [ ] **`CREATE TABLE ... AS SELECT` (CTAS):** অন্য টেবিলের ডেটাসহ নতুন টেবিল তৈরি।
- [ ] **`CREATE TABLE ... LIKE`:** অন্য টেবিলের হুবহু স্কিমা কপি করা (ডেটা ছাড়া)।
- [ ] **Copying Structure vs Copying Data:** শুধু কলাম কপি করা (`WHERE 1=0`) বনাম ডেটা কপি করা (`INSERT INTO ... SELECT`)।

### ৬.২ Inserting Data
- [ ] **Basic `INSERT`:** সাধারণ ডেটা ইনসার্ট।
- [ ] **`INSERT ... VALUES`:** নির্দিষ্ট মান দিয়ে একক বা একাধিক রো ইনসার্ট করা।
- [ ] **`INSERT ... SET`:** অবজেক্ট স্টাইলে কলাম ধরে ডেটা সেট করা (MySQL)।
- [ ] **`INSERT ... SELECT`:** অন্য টেবিল থেকে কুয়েরি করে ডেটা ইনসার্ট করা।
- [ ] **Default and NULL Handling:** মান না দিলে ডিফল্ট বা নাল বসা।
- [ ] **Duplicate-Key Handling:** ডুপ্লিকেট এরর এড়াতে `ON DUPLICATE KEY UPDATE` বা `INSERT IGNORE`।
- [ ] **Auto-Increment Handling:** অটো আইডি স্বয়ংক্রিয়ভাবে জেনারেট হওয়া ও রিট্রিভ করা।

### ৬.৩ UPDATE
- [ ] **`UPDATE`:** বিদ্যমান রো-এর মান পরিবর্তন করা।
- [ ] **`SET`:** কোন কোন কলামের নতুন মান কী হবে তা নির্ধারণ করা (`SET Salary = 60000 WHERE ID = 1`)।

### ৬.৪ DELETE vs TRUNCATE vs DROP
- [ ] **`DELETE`:** শর্ত দিয়ে নির্দিষ্ট কিছু রো মুছে ফেলা (DML, রো-বাই-রো লগ রাখে)।
- [ ] **`TRUNCATE`:** টেবিল রেখে সব ডেটা এক নিমেষে খালি করা ও আইডি ১-এ রিসেট করা (DDL)।
- [ ] **`DROP`:** ডেটাসহ পুরো টেবিল ধ্বংস করে ফেলা (DDL)।

### ৬.৫ Modifying Table Structure
- [ ] **`ALTER TABLE` Basics:** কলাম যোগ (`ADD`), বাদ (`DROP`), ডেটাটাইপ পরিবর্তন (`MODIFY`), নাম পরিবর্তন (`RENAME`), এবং সিরিয়াল পরিবর্তন (`REORDER`)।
- [ ] **Constraint Modification:** নতুন কনস্ট্রেইন্ট যোগ (`ADD CONSTRAINT`) বা মুছে ফেলা (`DROP CONSTRAINT`)।
- [ ] **Table Modification:** টেবিলের নাম, Character Set বা Collation পরিবর্তন করা।

---

# ৭. SELECT Fundamentals

### ৭.১ - ৭.৭ Basic SELECT Clauses
- [ ] **`SELECT`:** কলাম রিড বা কুয়েরি করার মূল কমান্ড।
- [ ] **`FROM`:** কোন টেবিল থেকে ডেটা আসবে তা নির্ধারণ করে।
- [ ] **`WHERE`:** রো লেভেলে শর্ত দিয়ে ডেটা ফিল্টার করে।
- [ ] **`GROUP BY`:** কলামের মান অনুযায়ী ডেটাকে গ্রুপ করে সামারি বের করে।
- [ ] **`HAVING`:** গ্রুপের ওপর শর্ত বা ফিল্টার বসায়।
- [ ] **`ORDER BY`:** ফলাফলকে ছোট থেকে বড় (`ASC`) বা বড় থেকে ছোট (`DESC`) সাজায়।
- [ ] **`LIMIT` and `OFFSET`:** কয়টি রো দেখাবে (`LIMIT`) এবং শুরুর কয়টি বাদ দেবে (`OFFSET`)।

### ৭.৮ Query Processing Order (লজিক্যাল এক্সিকিউশন ক্রম)
```text
  ১. FROM ──► ২. ON ──► ৩. JOIN ──► ৪. WHERE ──► ৫. GROUP BY ──► ৬. HAVING 
      └──► ৭. Window Functions ──► ৮. SELECT ──► ৯. DISTINCT ──► ১০. ORDER BY ──► ১১. LIMIT
```

### ৭.৯ - ৭.১১ Aliases, DISTINCT & Pagination
- [ ] **Column Aliases:** `AS` দিয়ে বা `AS` ছাড়া কলামের ছদ্মনাম দেওয়া।
- [ ] **`DISTINCT`:** আউটপুট থেকে ডুপ্লিকেট সারি বাদ দিয়ে শুধু ইউনিক মান দেখানো।
- [ ] **Basic Pagination Formula:**
  * পেজ ১ $\rightarrow$ `LIMIT 10 OFFSET 0`
  * পেজ ২ $\rightarrow$ `LIMIT 10 OFFSET 10`
  * পেজ ৩ $\rightarrow$ `LIMIT 10 OFFSET 20`

---

# ৮. Operators and Expressions

### ৮.১ Arithmetic Operators
- [ ] **Addition (`+`):** যোগফল (`Salary + Bonus`)।
- [ ] **Subtraction (`-`):** বিয়োগফল (`Total - Discount`)।
- [ ] **Multiplication (`*`):** গুণফল (`Price * Quantity`)।
- [ ] **Division (`/`):** ভাগফল (`Salary / 30`)।
- [ ] **Integer Division (`DIV`):** পূর্ণসংখ্যার ভাগফল (`10 DIV 3 = 3`)।
- [ ] **Modulo (`%` বা `MOD`):** ভাগশেষ বের করা (`10 % 3 = 1`)।

### ৮.২ Comparison Operators
- [ ] **Equal (`=`):** সমান কি না।
- [ ] **Not Equal (`!=` বা `<>`):** অসমান কি না।
- [ ] **Greater / Less Than (`>`, `<`):** বড় বা ছোট কি না।
- [ ] **Greater / Less Than or Equal (`>=`, `<=`)**
- [ ] **NULL-Safe Equality (`<=>`):** নাল ভ্যালুসহ দুটি মান সমান কি না চেক করা।

### ৮.৩ Logical Operators
- [ ] **`AND`:** সব শর্ত সত্য হতে হবে।
- [ ] **`OR`:** যেকোনো একটি সত্য হলেই হবে।
- [ ] **`NOT`:** শর্তের বিপরীত করা।
- [ ] **`XOR`:** দুটি শর্তের মধ্যে মাত্র একটি সত্য হতে হবে।
- [ ] **Operator Precedence:** অগ্রাধিকার ক্রম (`NOT` $\rightarrow$ `AND` $\rightarrow$ `OR`)।
- [ ] **Parentheses (`( )`):** অগ্রাধিকার নির্ধারণের ব্র্যাকেট।

### ৮.৪ Range & List Operators
- [ ] **`BETWEEN` / `NOT BETWEEN`:** নির্দিষ্ট রেঞ্জের মধ্যে বা বাইরে খোঁজা (`Age BETWEEN 20 AND 30`)।
- [ ] **`IN` / `NOT IN`:** তালিকার মধ্যে বা তালিকার বাইরে খোঁজা (`City IN ('Dhaka', 'Sylhet')`)।

### ৮.৫ Pattern Matching & Wildcards
- [ ] **`LIKE` / `NOT LIKE`:** প্যাটার্ন মিলানো (`Name LIKE 'R%'`)।
- [ ] **`EXISTS` / `NOT EXISTS`:** সাবকোয়েরির রেজাল্ট আছে কি না চেক করা।
- [ ] **`%` Wildcard:** শূন্য বা একাধিক অক্ষরের কম্বিনেশন (`'A%'`).
- [ ] **`_` Wildcard:** মাত্র একটি একক অক্ষর (`'_ahim'`).
- [ ] **`REGEXP` / `RLIKE`:** রেগুলার এক্সপ্রেশন দিয়ে জটিল প্যাটার্ন খোঁজা।

### ৮.৬ NULL Operators
- [ ] **`IS NULL` / `IS NOT NULL`:** মান খালি নাকি ভরা তা চেক করা।
- [ ] **`IS TRUE` / `IS FALSE`:** বুলিয়ান মান পরীক্ষা।

### ৮.৭ Bitwise & Assignment
- [ ] **Bitwise AND (`&`), OR (`|`), XOR (`^`), Shifting (`<<`, `>>`)**
- [ ] **Assignment Operators (`=` এবং `:=`)**

### ৮.৮ Conditional Expressions & Functions
- [ ] **`CASE` Expression:** `CASE WHEN ... THEN ... ELSE ... END` (SQL IF-ELSE)।
- [ ] **`IF()`:** সহজ কন্ডিশনাল ফাংশন (`IF(Age>=18, 'Adult', 'Minor')`)।
- [ ] **`IFNULL()`:** নাল হলে বিকল্প মান বসানো।
- [ ] **`NULLIF()`:** দুটি মান সমান হলে `NULL` রিটার্ন করে।
- [ ] **`COALESCE()`:** তালিকার প্রথম নন-নাল মানটি তুলে আনে।

### ৮.৯ Types of Filtering
- [ ] **Numeric, String, Date, Boolean এবং NULL ফিল্টারিং।**

---

# ৯. Aggregate Functions & Joins

### ৯.১ Aggregate Functions
- [ ] **`COUNT()`:** মোট কয়টি রো আছে তা গোনা।
- [ ] **`SUM()`:** কলামের সমস্ত সংখ্যার যোগফল বের করা।
- [ ] **`AVG()`:** গড় মান বের করা।
- [ ] **`MIN()` / `MAX()`:** সর্বনিম্ন ও সর্বোচ্চ মান খুঁজে বের করা।
- [ ] **`GROUP_CONCAT()` / `STRING_AGG()`:** গ্রুপের টেক্সট কমা দিয়ে এক লাইনে জোড়া লাগানো।

### ৯.২ Aggregate Variations (`COUNT`)
- [ ] **`COUNT(*)`:** সমস্ত রো কাউন্ট করে (এমনকি সব কলামে `NULL` থাকলেও)।
- [ ] **`COUNT(column)`:** শুধুমাত্র নির্দিষ্ট কলামের নন-নাল মানগুলো গোনে।
- [ ] **`COUNT(DISTINCT column)`:** ডুপ্লিকেট বাদ দিয়ে ইউনিক নন-নাল মানগুলো গোনে।

### ৯.৩ SQL Joins & Clauses
- [ ] **`INNER JOIN`:** শুধুমাত্র উভয় টেবিলের কমন/ম্যাচিং রেকর্ড আনে।
- [ ] **`LEFT JOIN`:** বাম টেবিলের সব রেকর্ড + ডান টেবিলের ম্যাচিং রেকর্ড আনে।
- [ ] **`RIGHT JOIN`:** ডান টেবিলের সব রেকর্ড + বাম টেবিলের ম্যাচিং রেকর্ড আনে।
- [ ] **`CROSS JOIN`:** দুই টেবিলের কার্তেসীয় গুণফল (Cartesian product)।
- [ ] **`SELF JOIN`:** একটি টেবিল যখন নিজের সাথেই JOIN করে।
- [ ] **`ON` Clause:** টেবিলগুলোর জয়েনিং শর্ত (`ON Orders.CustID = Customers.CustID`)।
- [ ] **`USING` Clause:** দুটি টেবিলে কলামের নাম এক হলে শর্টকাট জয়েন (`USING (CustID)`).

---

# ১০. Built-in Functions

### ১০.১ String Functions
- [ ] **`CONCAT()`:** টেক্সট জোড়া লাগানো (`CONCAT(First, ' ', Last)`).
- [ ] **`LOWER()` / `UPPER()`:** ছোট বা বড় হাতের অক্ষরে রূপান্তর।
- [ ] **`LENGTH()` / `LEN()`:** ক্যারেক্টার সংখ্যা গণনা।
- [ ] **`TRIM()`:** অপ্রয়োজনীয় স্পেস মোছা।
- [ ] **`SUBSTRING()`:** টেক্সটের নির্দিষ্ট অংশ কেটে আনা।
- [ ] **`LOCATE()` / `CHARINDEX()`:** শব্দের পজিশন নম্বর বের করা।
- [ ] **`REPLACE()`:** নির্দিষ্ট শব্দ বদলে অন্য শব্দ বসানো।

### ১০.২ Numeric Functions
- [ ] **`ABS()`:** ঋণাত্মককে ধনাত্মক করা (`ABS(-50) = 50`)।
- [ ] **`ROUND()`:** দশমিক সংখ্যাকে নির্দিষ্ট ঘর পর্যন্ত আসন্ন মান করা।
- [ ] **`CEIL()` / `CEILING()`:** নিকটবর্তী বড় পূর্ণসংখ্যায় তোলা (`CEIL(4.1) = 5`)।
- [ ] **`FLOOR()`:** নিকটবর্তী ছোট পূর্ণসংখ্যায় নামানো (`FLOOR(4.9) = 4`)।
- [ ] **`TRUNCATE()`:** দশমিকের পরের অংশ সরাসরি কেটে ফেলা।
- [ ] **`MOD()`, `POWER()`, `SQRT()`:** ভাগশেষ, পাওয়ার ও বর্গমূল।

### ১০.৩ Date & Time Functions
- [ ] **`CURDATE()` / `GETDATE()`:** আজকের তারিখ।
- [ ] **`NOW()` / `CURRENT_TIMESTAMP`:** বর্তমান তারিখ ও সময় একসাথে।
- [ ] **`YEAR()`, `MONTH()`, `DAY()`:** তারিখ থেকে সাল, মাস বা দিন আলাদা করা।
- [ ] **`DATEDIFF()`:** দুটি তারিখের মাঝখানের ব্যবধান।
- [ ] **`DATE_FORMAT()` / `FORMAT()`:** তারিখ সুন্দর ফরম্যাটে দেখানো।
- [ ] **`STR_TO_DATE()`:** স্ট্রিংকে ডেট ফরম্যাটে রূপান্তর।

### ১০.৪ Conversion Functions
- [ ] **`CAST()`:** ANSI স্ট্যান্ডার্ড ডেটাটাইপ রূপান্তর।
- [ ] **`CONVERT()`:** ডেটাটাইপ রূপান্তরের সাথে স্টাইল ফরম্যাটিং।

---

# ১১. Subqueries & Set Operations

### ১১.১ Subquery Fundamentals
- [ ] **Inner and Outer Queries:** ব্র্যাকেটের ভেতরের কোয়েরি (Inner) আগে চলে এবং বাইরের কোয়েরি (Outer) তার রেজাল্ট ব্যবহার করে।
- [ ] **Nested Queries:** সাবকোয়েরির ভেতরে আরও সাবকোয়েরি।

### ১১.২ Multiple-row Subqueries
- [ ] **`IN` / `NOT IN`:** সাবকোয়েরির লিস্টের মানের সাথে মিলানো।
- [ ] **`ANY` / `SOME`:** লিস্টের যেকোনো একটি মানের চেয়ে বড়/ছোট কি না।
- [ ] **`ALL`:** লিস্টের সবগুলো মানের চেয়ে বড়/ছোট হতে হবে।

### ১১.৩ `EXISTS` & `NOT EXISTS`
- [ ] **`EXISTS`:** সাবকোয়েরিতে অন্তত একটি রেকর্ড মিললে সত্য রিটার্ন করে (সুপার ফাস্ট)।
- [ ] **`NOT EXISTS`:** সাবকোয়েরিতে কোনো রেকর্ড না থাকলে সত্য রিটার্ন করে।

### ১১.৪ Subquery Locations
- [ ] **Subquery in `SELECT`:** কলাম হিসেবে বসা (Scalar Subquery)।
- [ ] **Subquery in `FROM`:** ভার্চুয়াল টেবিল হিসেবে বসা (Derived Table / Inline View)।
- [ ] **Subquery in `WHERE` / `HAVING`:** শর্ত বা ফিল্টার হিসেবে বসা।

### ১১.৫ Set Operations
- [ ] **`UNION` / `UNION ALL`:** কুয়েরির রেজাল্ট যোগ করা (ইউনিক মান বনাম সব মান)।
- [ ] **`INTERSECT` / `INTERSECT ALL`:** দুই কুয়েরির কমন রেকর্ড আনা।
- [ ] **`EXCEPT` / `EXCEPT ALL` (`MINUS`):** ১ম কুয়েরি থেকে ২য় কুয়েরির রেজাল্ট বাদ দেওয়া।

---

# ১২. Views, Indexes & Transactions

### ১২.১ Views
- [ ] **`CREATE VIEW` / `CREATE OR REPLACE VIEW`:** ভার্চুয়াল সিকিউর টেবিল তৈরি ও আপডেট।
- [ ] **`ALTER VIEW` / `DROP VIEW`:** ভিউ পরিবর্তন বা মুছে ফেলা।
- [ ] **`WITH CHECK OPTION`:** ভিউয়ের মাধ্যমে ডেটা ঢোকানোর সময় শর্ত নিশ্চিত করা।

### ১২.২ Indexes
- [ ] **Primary Index:** প্রাইমারি কী-এর ওপর তৈরি হওয়া ক্লাস্টার্ড ইনডেক্স।
- [ ] **Unique Index:** ডুপ্লিকেট রোধকারী ও দ্রুত সার্চ ইনডেক্স।
- [ ] **Single-Column vs Composite Index:** এক কলামের ইনডেক্স বনাম একাধিক কলামের যৌথ ইনডেক্স।

### ১২.৩ Transactions & ACID Properties
- [ ] **Commands:** `START TRANSACTION` / `BEGIN`, `COMMIT` (স্থায়ী সেভ), `ROLLBACK` (বাতিল)।
- [ ] **ACID Properties:**
  * **Atomicity:** All or nothing (সব হবে অথবা কিছুই হবে না)।
  * **Consistency:** লেনদেনের আগে ও পরে ডেটাবেস ভ্যালিড থাকবে।
  * **Isolation:** একাধিক লেনদেন একে অপরকে প্রভাবিত করবে না।
  * **Durability:** একবার কমিট হলে বিদ্যুৎ গেলেও ডেটা নষ্ট হবে না।
- [ ] **Isolation Levels:** `Read Uncommitted`, `Read Committed`, `Repeatable Read`, `Serializable`।

### ১২.৪ Concurrency Problems, Locking & Deadlocks
- [ ] **Dirty Read:** আনকমিটেড ডেটা পড়ে ফেলা।
- [ ] **Non-Repeatable Read:** একই কুয়েরিতে আপডেটের কারণে মান বদলে যাওয়া।
- [ ] **Phantom Read:** নতুন রো ইনসার্টের কারণে অতিরিক্ত সারি দেখা।
- [ ] **Lost Update:** দুজন একসাথে এডিট করায় একজনের তথ্য হারিয়ে যাওয়া।
- [ ] **Locking & Deadlocks:** রিসোর্সে তালা দেওয়া এবং দুই সেশন আটকে গেলে ভিকটিম কিল করে ডেডলক ছুটানো।

---

# ১৩. Advanced SQL (Only for Data/SQL Engineers)

### ১৩.১ Common Table Expressions (CTE)
- [ ] **`WITH` (Standard CTE):** কোয়েরিকে সহজে পড়ার উপযোগী সাময়িক টেবিল।
- [ ] **`WITH RECURSIVE`:** লুপ চালিয়ে ট্রি বা হায়ারার্কিকাল ডেটা ট্রাভার্স করা।

### ১৩.২ Window Functions & Window Frames
- [ ] **Window Syntax:** `OVER (PARTITION BY ... ORDER BY ...)`
- [ ] **Ranking:** `ROW_NUMBER()`, `RANK()`, `DENSE_RANK()`, `NTILE()`
- [ ] **Value:** `LAG()`, `LEAD()`, `FIRST_VALUE()`, `LAST_VALUE()`, `NTH_VALUE()`
- [ ] **Distribution:** `CUME_DIST()`, `PERCENT_RANK()`
- [ ] **Running Total:** `SUM(Salary) OVER (PARTITION BY Dept ORDER BY Date)`
- [ ] **Window Frames:** `ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING`

### ১৩.৩ Stored Procedures & Procedural Logic
- [ ] **Commands:** `CREATE PROCEDURE`, `CALL`, `ALTER PROCEDURE`, `DROP PROCEDURE`
- [ ] **Parameters:** `IN` (ইনপুট), `OUT` (আউটপুট), `INOUT` (ইনপুট ও আউটপুট উভয়ই)।
- [ ] **Blocks & Variables:** `BEGIN...END`, `DECLARE`, `SET`, `SELECT ... INTO`
- [ ] **Conditions & Loops:** `IF...ELSE`, `CASE`, `WHILE`, `LOOP`, `REPEAT`, `LEAVE`, `ITERATE`
- [ ] **Cursors:** `DECLARE CURSOR` $\rightarrow$ `OPEN` $\rightarrow$ `FETCH` $\rightarrow$ `CLOSE`
- [ ] **Error Handling:** `CONTINUE/EXIT HANDLER`, `SIGNAL`, `RESIGNAL`, `GET DIAGNOSTICS`
- [ ] **Stored Functions:** প্রসিডিউরাল ফাংশন যা মান রিটার্ন করে।

### ১৩.৪ Triggers
- [ ] **Commands:** `CREATE TRIGGER`, `DROP TRIGGER`, `SHOW TRIGGERS`
- [ ] **Timings:** `BEFORE`, `AFTER`, `INSTEAD OF`
- [ ] **Events:** `INSERT`, `UPDATE`, `DELETE`

### ১৩.৫ Scheduled Events (ক্রন জব / শিডিউলার)
- [ ] **Commands:** `CREATE EVENT`, `ALTER EVENT`, `DROP EVENT`
- [ ] **Schedules:** `AT 'YYYY-MM-DD'`, `EVERY 1 DAY`, `STARTS`, `ENDS`

### ১৩.৬ Users, Roles & Privileges
- [ ] **User Management:** `CREATE USER`, `ALTER USER`, `RENAME USER`, `DROP USER`, `ACCOUNT LOCK`
- [ ] **Privilege Management:** `GRANT`, `REVOKE`, `SHOW GRANTS`, `WITH GRANT OPTION`
- [ ] **Role Management:** `CREATE ROLE`, `DROP ROLE`, `GRANT role TO user`, `SET ROLE`

### ১৩.৭ Table Partitioning (বিশাল টেবিল বিভাজন)
- [ ] **Partition Types:** `RANGE`, `RANGE COLUMNS`, `LIST`, `LIST COLUMNS`, `HASH`, `KEY`
- [ ] **Partition Management:** `ADD PARTITION`, `DROP PARTITION`, `TRUNCATE PARTITION`, `REORGANIZE`, `EXCHANGE`

### ১৩.৮ Import and Export
- [ ] **Import:** `LOAD DATA INFILE`, `LOAD DATA LOCAL INFILE`
- [ ] **Export:** `SELECT ... INTO OUTFILE`
- [ ] **Formatting:** `FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'`

### ১৩.৯ Query Optimization
- [ ] **`EXPLAIN` / `EXPLAIN ANALYZE`:** এক্সিকিউশন প্ল্যান ও কস্ট দেখা।
- [ ] **Index Seek vs Scan:** ফুল টেবিল স্ক্যান এড়িয়ে সরাসরি ইনডেক্স থেকে ডেটা আনা।
- [ ] **Covering Index:** ইনডেক্সেই সমস্ত প্রয়োজনীয় কলাম রেখে কুয়েরিকে সুপারফাস্ট করা।
- [ ] **Query Refactoring:** স্লো সাবকোয়েরির বদলে অপ্টিমাইজড `JOIN` বা `CTE` ব্যবহার করা।
