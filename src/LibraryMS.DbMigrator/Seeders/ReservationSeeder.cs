using LibraryMS.Domain.ReservationManagement.AggregateRoots;
using LibraryMS.EntityFrameworkCore;

namespace LibraryMS.DbMigrator.Seeders;

public class ReservationSeeder : IDataSeeder
{
    public async Task SeedAsync(LibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        // 1. Pending Reservation (John waiting for B2 at Downtown Branch)
        var pendingRes = new Reservation(
            Guid.NewGuid(),
            UserAndMemberSeeder.MemberJohnId,
            BookSeeder.B2Id,
            BranchSeeder.DowntownBranchId,
            1 // Queue position
        );

        // 2. Available Reservation (Jane's reservation for B3 is ready to pick up)
        var availableRes = new Reservation(
            Guid.NewGuid(),
            UserAndMemberSeeder.MemberJaneId,
            BookSeeder.B3Id,
            BranchSeeder.MainBranchId,
            1
        );
        availableRes.NotifyAvailable(); // Marks it as Available and sets expiration

        dbContext.Reservations.AddRange(pendingRes, availableRes);
    }
}
