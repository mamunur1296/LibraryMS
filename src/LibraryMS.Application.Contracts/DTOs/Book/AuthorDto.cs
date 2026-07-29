namespace LibraryMS.Application.Contracts.DTOs.Book;

public sealed class AuthorDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Biography { get; init; }
}
