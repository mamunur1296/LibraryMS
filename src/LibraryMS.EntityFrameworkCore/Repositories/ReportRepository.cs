using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly LibraryDbContext _dbContext;

    public ReportRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BranchComparisonDto>> GetBranchComparisonAsync(CancellationToken ct = default)
    {
        var branches = await _dbContext.Branches.Where(b => b.IsActive).ToListAsync(ct);
        var result = new List<BranchComparisonDto>();

        foreach (var branch in branches)
        {
            var branchId = branch.Id;

            // Approximating Total Books by active BookCopies in branch
            var totalBooks = await _dbContext.BookCopies.CountAsync(c => c.BranchId == branchId, ct);

            var activeBorrows = await _dbContext.Set<BorrowRecord>()
                .CountAsync(r => r.BranchId == branchId && r.Status == BorrowStatus.Active, ct);

            var overdueBorrows = await _dbContext.Set<BorrowRecord>()
                .CountAsync(r => r.BranchId == branchId && r.Status == BorrowStatus.Overdue, ct);

            var totalRevenue = await _dbContext.Set<BorrowRecord>()
                .Where(r => r.BranchId == branchId && r.IsFinePaid)
                .SumAsync(r => r.LateFine, ct);

            result.Add(new BranchComparisonDto
            {
                BranchId = branchId,
                BranchName = branch.Name,
                TotalBooks = totalBooks,
                ActiveBorrows = activeBorrows,
                OverdueBorrows = overdueBorrows,
                TotalRevenue = totalRevenue
            });
        }

        return result.OrderBy(b => b.BranchName).ToList();
    }

    public async Task<List<AnnualRevenueDto>> GetAnnualRevenueAsync(int year, CancellationToken ct = default)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddYears(1);

        var fines = await _dbContext.Set<BorrowRecord>()
            .Where(r => r.IsFinePaid && r.ReturnDate >= startDate && r.ReturnDate < endDate)
            .GroupBy(r => r.ReturnDate!.Value.Month)
            .Select(g => new { Month = g.Key, Revenue = g.Sum(x => x.LateFine) })
            .ToListAsync(ct);

        var result = new List<AnnualRevenueDto>();
        for (int i = 1; i <= 12; i++)
        {
            var monthData = fines.FirstOrDefault(f => f.Month == i);
            result.Add(new AnnualRevenueDto
            {
                Month = i,
                MonthName = new DateTime(year, i, 1).ToString("MMM"),
                Revenue = monthData?.Revenue ?? 0
            });
        }

        return result;
    }

    public async Task<List<MemberGrowthDto>> GetMemberGrowthAsync(int year, CancellationToken ct = default)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddYears(1);

        var memberCounts = await _dbContext.Members
            .Where(m => m.CreatedAt >= startDate && m.CreatedAt < endDate)
            .GroupBy(m => m.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var result = new List<MemberGrowthDto>();
        for (int i = 1; i <= 12; i++)
        {
            var monthData = memberCounts.FirstOrDefault(m => m.Month == i);
            result.Add(new MemberGrowthDto
            {
                Month = i,
                MonthName = new DateTime(year, i, 1).ToString("MMM"),
                NewMembers = monthData?.Count ?? 0
            });
        }

        return result;
    }

    public async Task<List<LibrarianActivityDto>> GetLibrarianActivityAsync(DateTime? fromDate, DateTime? toDate, CancellationToken ct = default)
    {
        var librarians = await _dbContext.Users.Where(u => u.Role == LibraryMS.Domain.Shared.Enums.UserRole.Librarian).ToListAsync(ct);
        var branches = await _dbContext.Branches.ToListAsync(ct);

        var query = _dbContext.Set<BorrowRecord>().AsQueryable();

        if (fromDate.HasValue) query = query.Where(r => r.CreatedAt >= fromDate.Value.ToUniversalTime());
        if (toDate.HasValue) query = query.Where(r => r.CreatedAt <= toDate.Value.ToUniversalTime());

        var borrows = await query.ToListAsync(ct);

        var result = new List<LibrarianActivityDto>();

        foreach (var lib in librarians)
        {
            var issued = borrows.Count(b => b.IssuedById == lib.Id);
            var returned = borrows.Count(b => b.ReturnedById == lib.Id);

            if (issued > 0 || returned > 0)
            {
                result.Add(new LibrarianActivityDto
                {
                    UserId = lib.Id,
                    Name = lib.Username,
                    BranchName = branches.FirstOrDefault(b => b.Id == lib.BranchId)?.Name ?? "Unknown",
                    BooksIssued = issued,
                    BooksReturned = returned
                });
            }
        }

        return result.OrderByDescending(r => r.BooksIssued + r.BooksReturned).ToList();
    }
}
