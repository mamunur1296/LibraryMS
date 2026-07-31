using MediatR;
using System;

namespace LibraryMS.Application.Contracts.Members;

public sealed record ResetMemberPasswordCommand(Guid MemberId, string NewPassword) : IRequest;
