// Expose Domain internals to Application layer for DDD patterns
// This is intentional: Application layer orchestrates Domain objects
// but cannot be directly accessed by outer layers (API, Infrastructure)
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LibraryMS.Application")]
[assembly: InternalsVisibleTo("LibraryMS.Domain.Tests")]
[assembly: InternalsVisibleTo("LibraryMS.Application.Tests")]
[assembly: InternalsVisibleTo("LibraryMS.TestBase")]
[assembly: InternalsVisibleTo("LibraryMS.DbMigrator")]
