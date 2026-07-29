using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared.Enums;
using System;

namespace LibraryMS.TestBase;

/// <summary>
/// Centralized factory for creating consistent, valid domain aggregates for tests.
/// Matches actual domain constructor signatures from Phase 3/6.
/// </summary>
public static class TestDataFactory
{
    public static Branch CreateBranch(string name = "Main Library Branch")
        => new(Guid.NewGuid(), name, "123 Library St", "555-0100", "branch1@library.com");

    public static Book CreateBook(
        string title = "Clean Code", 
        string authorName = "Robert C. Martin", 
        string isbn = "9780132350884")
    {
        var categoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        return new Book(
            Guid.NewGuid(),
            title,
            isbn,
            "Software Legend",
            2008,
            categoryId,
            authorId,
            "English");
    }

    public static BookCopy CreateBookCopy(Book book, Branch branch, string copyNumber = "BC-00001")
        => new(Guid.NewGuid(), book.Id, branch.Id, copyNumber);

    public static Member CreateMember(
        string name = "John Doe", 
        string email = "john.doe@example.com", 
        MemberStatus status = MemberStatus.Active)
    {
        var member = new Member(
            Guid.NewGuid(),
            "John",
            "Doe",
            email,
            "555-0199",
            "LIB-2024-00001",
            "456 Main St");
        
        if (status == MemberStatus.Suspended)
        {
            member.Suspend(DateTime.UtcNow.AddDays(7), "Suspended for testing");
        }
        
        return member;
    }

    public static BorrowRecord CreateBorrowRecord(BookCopy copy, Member member, DateTime borrowDate, int days = 14)
    {
        var borrowRecord = new BorrowRecord(
            Guid.NewGuid(),
            member.Id,
            copy.Id,
            copy.BookId,
            copy.BranchId,
            days);
        return borrowRecord;
    }

    public static Reservation CreateReservation(Book book, Member member, int queuePosition = 1)
        => new(Guid.NewGuid(), member.Id, book.Id, Guid.NewGuid(), queuePosition);
}

