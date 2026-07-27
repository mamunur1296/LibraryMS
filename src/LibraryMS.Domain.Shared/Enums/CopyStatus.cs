namespace LibraryMS.Domain.Shared.Enums;

/// <summary>Book copy availability status</summary>
public enum CopyStatus
{
    Available = 1,
    Borrowed = 2,
    Reserved = 3,
    Damaged = 4,
    Lost = 5
}
