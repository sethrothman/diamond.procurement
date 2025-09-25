using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.Win;

public interface IIngestionHandler
{
    FileKind Kind { get; }
    bool RequiresPartyId { get; } // Buyer/Vendor id needed?
    Task HandleAsync(string path, int partyId, object? signatureMap, CancellationToken ct);
}
