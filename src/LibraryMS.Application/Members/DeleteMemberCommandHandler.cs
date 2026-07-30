using LibraryMS.Application.Contracts.Members;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand>
{
    private readonly IMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMemberCommandHandler> _logger;

    public DeleteMemberCommandHandler(
        IMemberRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteMemberCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting member with ID: {Id}", request.Id);

        var member = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.Id}' was not found.");

        var dbFailed = false;
        try
        {
            await _repository.DeleteAsync(member!, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete member with ID {Id} from database.", request.Id);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while deleting the member from the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Member with ID {Id} deleted successfully.", request.Id);
    }
}
