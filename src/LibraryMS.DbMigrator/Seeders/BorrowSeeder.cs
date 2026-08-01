using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.EntityFrameworkCore;
using System.Reflection;

namespace LibraryMS.DbMigrator.Seeders;

public class BorrowSeeder : IDataSeeder
{
    public async Task SeedAsync(LibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        // 1. Active Borrow (John borrowed B1 today)
        var activeBorrow = new BorrowRecord(
            Guid.NewGuid(), 
            UserAndMemberSeeder.MemberJohnId, 
            BookSeeder.B1Copy1Id, 
            BookSeeder.B1Id, 
            BranchSeeder.MainBranchId
        );

        // 2. Overdue Borrow (Jane borrowed B2 34 days ago, due 20 days ago)
        var overdueBorrow = new BorrowRecord(
            Guid.NewGuid(), 
            UserAndMemberSeeder.MemberJaneId, 
            BookSeeder.B2Copy1Id, 
            BookSeeder.B2Id, 
            BranchSeeder.MainBranchId
        );
        SetPastDates(overdueBorrow, 34, 20);
        overdueBorrow.MarkAsOverdue();

        // 3. Returned Borrow with Paid Fine (Bob borrowed B3 40 days ago, due 26 days ago, returned 10 days ago)
        var returnedBorrow = new BorrowRecord(
            Guid.NewGuid(), 
            UserAndMemberSeeder.MemberBobId, 
            BookSeeder.B3Copy1Id, 
            BookSeeder.B3Id, 
            BranchSeeder.MainBranchId
        );
        SetPastDates(returnedBorrow, 40, 26);
        returnedBorrow.MarkAsOverdue(); // First mark it overdue
        
        SetProperty(returnedBorrow, "ReturnDate", DateTime.UtcNow.AddDays(-10));
        SetProperty(returnedBorrow, "Status", BorrowStatus.Returned);
        returnedBorrow.AccumulateFine(); // Calculate fine
        returnedBorrow.PayFine();

        dbContext.BorrowRecords.AddRange(activeBorrow, overdueBorrow, returnedBorrow);
    }

    private void SetPastDates(BorrowRecord record, int borrowDaysAgo, int dueDaysAgo)
    {
        SetProperty(record, "BorrowDate", DateTime.UtcNow.AddDays(-borrowDaysAgo));
        SetProperty(record, "DueDate", DateTime.UtcNow.AddDays(-dueDaysAgo));
    }

    private void SetProperty(object obj, string propertyName, object value)
    {
        var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(obj, value);
        }
        else
        {
            // Try field if property fails
            var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
