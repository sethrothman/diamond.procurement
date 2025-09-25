public interface IBuyerScopedProcessor
{
    Task SetBuyer(int buyerId, CancellationToken ct);
}
