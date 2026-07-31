using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.MemberManagement.Entities;

public class MemberFavorite
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public Guid BookId { get; private set; }
    public DateTime AddedAt { get; private set; }

    private MemberFavorite() { } // EF Core

    public MemberFavorite(Guid id, Guid memberId, Guid bookId)
    {
        Id = id;
        MemberId = memberId;
        BookId = bookId;
        AddedAt = DateTime.UtcNow;
    }
}
