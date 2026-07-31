using MediatR;

namespace LibraryMS.Application.Contracts.Settings;

public record UpdateSettingCommand(string Key, string Value) : IRequest;
