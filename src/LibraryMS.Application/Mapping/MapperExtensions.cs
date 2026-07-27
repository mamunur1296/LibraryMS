using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.ReservationManagement;

namespace LibraryMS.Application.Mapping;

public static class MapperExtensions
{
    public static BranchDto ToDto(this Branch branch)
    {
        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Address = branch.Address,
            Phone = branch.Phone,
            Email = branch.Email,
            IsActive = branch.IsActive,
            CreatedAt = branch.CreatedAt
        };
    }

    public static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN.Value,
            Description = book.Description,
            PublicationYear = book.PublicationYear,
            Language = book.Language,
            CoverImageUrl = book.CoverImageUrl,
            CategoryId = book.CategoryId,
            AuthorId = book.AuthorId,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            CreatedAt = book.CreatedAt,
            // Navigation properties to be filled by Queries when joining
            CategoryName = string.Empty, 
            AuthorName = string.Empty
        };
    }

    public static BookCopyDto ToDto(this BookCopy copy)
    {
        return new BookCopyDto
        {
            Id = copy.Id,
            CopyNumber = copy.CopyNumber,
            Status = copy.Status.ToString(),
            BranchId = copy.BranchId,
            BranchName = string.Empty
        };
    }

    public static AuthorDto ToDto(this Author author)
    {
        return new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            Biography = author.Biography
        };
    }

    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public static MemberDto ToDto(this Member member)
    {
        return new MemberDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            FullName = member.FullName,
            Email = member.Email,
            Phone = member.Phone,
            MembershipNumber = member.MembershipNumber,
            Address = member.Address,
            Status = member.Status.ToString(),
            JoinDate = member.JoinDate,
            SuspendedUntil = member.SuspendedUntil,
            ActiveBorrows = 0
        };
    }

    public static BorrowDto ToDto(this BorrowRecord borrow)
    {
        return new BorrowDto
        {
            Id = borrow.Id,
            MemberId = borrow.MemberId,
            BookId = borrow.BookId,
            BranchId = borrow.BranchId,
            BorrowDate = borrow.BorrowDate,
            DueDate = borrow.DueDate,
            ReturnDate = borrow.ReturnDate,
            Status = borrow.Status.ToString(),
            LateFine = borrow.LateFine,
            IsFinePaid = borrow.IsFinePaid,
            IsOverdue = borrow.IsOverdue,
            DaysUntilDue = borrow.DaysUntilDue,
            // Navigational
            MemberName = string.Empty,
            MembershipNumber = string.Empty,
            BookTitle = string.Empty,
            BookISBN = string.Empty,
            CopyNumber = string.Empty,
            BranchName = string.Empty
        };
    }

    public static ReservationDto ToDto(this Reservation reservation)
    {
        return new ReservationDto
        {
            Id = reservation.Id,
            MemberId = reservation.MemberId,
            BookId = reservation.BookId,
            BranchId = reservation.BranchId,
            QueuePosition = reservation.QueuePosition,
            Status = reservation.Status.ToString(),
            CreatedAt = reservation.CreatedAt,
            NotifiedAt = reservation.NotifiedAt,
            ExpiresAt = reservation.ExpiresAt,
            MemberName = string.Empty,
            MembershipNumber = string.Empty,
            BookTitle = string.Empty,
            BookISBN = string.Empty,
            BranchName = string.Empty
        };
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            MemberId = user.MemberId
        };
    }
}
