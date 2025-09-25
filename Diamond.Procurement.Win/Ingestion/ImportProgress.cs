using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.Win;

public readonly record struct ImportProgress(
    FileKind Kind,
    string Message,
    bool IsDone = false,
    bool IsSuccess = false,
    string? Error = null,
    int? Rows = null
);