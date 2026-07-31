using System.Collections.Generic;
using MediatR;

namespace LibraryMS.Application.Contracts.Settings;

public record GetSettingsQuery() : IRequest<List<SettingDto>>;
