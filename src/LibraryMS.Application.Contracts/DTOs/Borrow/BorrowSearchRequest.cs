namespace LibraryMS.Application.Contracts.DTOs.Borrow;

public sealed class BorrowSearchRequest
{
    public Guid? MemberId { get; init; }
    public Guid? BookId { get; init; }
    public string? Status { get; init; }  // "Active", "Returned", "Overdue"
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
