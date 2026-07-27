using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.BookManagement.VOs;

/// <summary>
/// ISBN Value Object — validates ISBN-10 and ISBN-13 formats.
/// </summary>
public sealed class ISBN : ValueObject
{
    public string Value { get; }

    private ISBN(string value) => Value = value;

    public static ISBN Create(string raw)
    {
        var cleaned = raw?.Replace("-", "").Replace(" ", "") ?? string.Empty;
        if (cleaned.Length != 10 && cleaned.Length != 13)
            throw new ArgumentException($"Invalid ISBN format: '{raw}'. Must be 10 or 13 digits.");
        if (!cleaned.All(char.IsDigit))
            throw new ArgumentException($"ISBN must contain only digits: '{raw}'.");
        return new ISBN(cleaned);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
