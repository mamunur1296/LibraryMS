using System;

namespace LibraryMS.Domain.Common;

public sealed class GuidGenerator : IGuidGenerator
{
    public Guid Create()
    {
        return Guid.NewGuid();
    }
}
