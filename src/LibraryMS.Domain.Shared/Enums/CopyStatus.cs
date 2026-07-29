namespace LibraryMS.Domain.Shared.Enums;

// Book copy availability status
public enum CopyStatus
{
    Available = 1,
    Borrowed = 2,
    Reserved = 3,
    Damaged = 4,
    Lost = 5
}
