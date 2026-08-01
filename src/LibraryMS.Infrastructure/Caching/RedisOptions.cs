namespace LibraryMS.Infrastructure.Caching;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";
    public string Configuration { get; init; } = "localhost:6379,abortConnect=false";
}
