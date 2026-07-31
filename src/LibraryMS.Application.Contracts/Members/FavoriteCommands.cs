using MediatR;

namespace LibraryMS.Application.Contracts.Members;

public sealed record AddFavoriteCommand(Guid MemberId, Guid BookId) : IRequest;
public sealed record RemoveFavoriteCommand(Guid MemberId, Guid BookId) : IRequest;
