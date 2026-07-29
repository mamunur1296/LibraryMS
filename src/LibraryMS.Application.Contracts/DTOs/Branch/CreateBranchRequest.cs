namespace LibraryMS.Application.Contracts.DTOs.Branch;

public sealed class CreateBranchRequest
{
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string Email { get; init; } = default!;
}
