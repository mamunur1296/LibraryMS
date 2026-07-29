using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Branches;

public sealed class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDto?>
{
    private readonly IBranchRepository _repository;
    private readonly ILogger<GetBranchByIdQueryHandler> _logger;

    public GetBranchByIdQueryHandler(IBranchRepository repository, ILogger<GetBranchByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<BranchDto?> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving branch with ID: {Id}", request.Id);

        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(branch, $"Branch with ID '{request.Id}' was not found.");

        _logger.LogInformation("Successfully retrieved branch with ID: {Id}", request.Id);

        return branch?.ToDto();
    }
}
