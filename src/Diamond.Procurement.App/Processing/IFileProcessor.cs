using Diamond.Procurement.Domain.Util;
namespace Diamond.Procurement.App.Processing;

public interface IFileProcessor
{
    FileKind Kind { get; }
    Task ProcessAsync(string path, CancellationToken ct);
}