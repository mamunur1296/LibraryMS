namespace LibraryMS.Application.Contracts.DTOs.Branch;

public sealed class BranchDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string Email { get; init; } = default!;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class CreateBranchRequest
{
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string Email { get; init; } = default!;
}

public sealed class UpdateBranchRequest
{
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string Email { get; init; } = default!;
}
