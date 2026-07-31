namespace LibraryMS.Application.Contracts.DTOs.Report;

public class LibrarianActivityDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public int BooksIssued { get; set; }
    public int BooksReturned { get; set; }
}
