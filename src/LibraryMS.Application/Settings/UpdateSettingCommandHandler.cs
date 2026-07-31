using LibraryMS.Application.Contracts.Settings;
using LibraryMS.Domain.SettingsManagement;
using LibraryMS.Domain.SettingsManagement.Entities;
using LibraryMS.Domain.Shared;
using MediatR;

namespace LibraryMS.Application.Settings;

public class UpdateSettingCommandHandler : IRequestHandler<UpdateSettingCommand>
{
    private readonly ISystemSettingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSettingCommandHandler(ISystemSettingRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
    {
        var setting = await _repository.GetByKeyAsync(request.Key, cancellationToken);
        
        if (setting == null)
        {
            setting = new SystemSetting(Guid.NewGuid(), request.Key, request.Value);
            await _repository.AddAsync(setting, cancellationToken);
        }
        else
        {
            setting.UpdateValue(request.Value);
            await _repository.UpdateAsync(setting, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
