using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.Win;

public interface IIngestionDispatcher
{
    Task<int?> DispatchAsync(FileKind kind, string path, int? partyId, object? signatureMap, CancellationToken ct);
}

public sealed class IngestionDispatcher : IIngestionDispatcher
{
    private readonly Dictionary<FileKind, IIngestionHandler> _map;

    public IngestionDispatcher(IEnumerable<IIngestionHandler> handlers)
        => _map = handlers.ToDictionary(h => h.Kind);

    public async Task<int?> DispatchAsync(FileKind kind, string path, int? partyId, object? signatureMap, CancellationToken ct)
    {
        if (!_map.TryGetValue(kind, out var h))
            throw new InvalidOperationException($"No handler for {kind}");

        if (h.RequiresPartyId)
        {
            if (partyId <= 0)
                throw new ArgumentException("PartyId (Buyer or Vendor) is required.");

            await h.HandleAsync(path, partyId.Value, signatureMap, ct);
            return null;
        }
        else
        { 
            await h.HandleAsync(path, 0, signatureMap, ct);
            return null;
        } 
    }
}
