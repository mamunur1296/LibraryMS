namespace LibraryMS.Application.Contracts.DTOs.Member;

public sealed class MemberSearchRequest
{
    public string? SearchTerm { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
