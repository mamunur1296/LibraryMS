using LibraryMS.Application.Contracts.Settings;
using MediatR;
using LibraryMS.Domain.SettingsManagement;

namespace LibraryMS.Application.Settings;

public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, List<SettingDto>>
{
    private readonly ISystemSettingRepository _repository;

    public GetSettingsQueryHandler(ISystemSettingRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SettingDto>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetAllAsync(cancellationToken);
        return settings.Select(s => new SettingDto(s.Key, s.Value)).ToList();
    }
}
