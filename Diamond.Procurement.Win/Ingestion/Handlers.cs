using Diamond.Procurement.App.Processing;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.Win;

public delegate IFileProcessor BuyerInventoryProcessorResolver(int buyerId);

public sealed class BuyerInventoryHandler : IIngestionHandler
{
    private readonly BuyerInventoryProcessorResolver _resolve;
    public BuyerInventoryHandler(BuyerInventoryProcessorResolver resolve) => _resolve = resolve;

    public FileKind Kind => FileKind.BuyerInventory;
    public bool RequiresPartyId => true;

    public async Task HandleAsync(string path, int buyerId, object? signatureMap, CancellationToken ct)
    {
        var processor = _resolve(buyerId);

        // Apply the header map if the processor supports it
        if (signatureMap is not null)
        {
            switch (signatureMap)
            {
                case ExcelSignatures.DgInventoryHeaderMap dg when processor is IUsesSignatureMap<ExcelSignatures.DgInventoryHeaderMap> p1:
                    p1.SetSignatureMap(dg); break;

                case ExcelSignatures.MeijerHdr mj when processor is IUsesSignatureMap<ExcelSignatures.MeijerHdr> p2:
                    p2.SetSignatureMap(mj); break;

                case ExcelSignatures.CnsInventoryHeaderMap cns when processor is IUsesSignatureMap<ExcelSignatures.CnsInventoryHeaderMap> p3:
                    p3.SetSignatureMap(cns); break;
            }
        }

        if (processor is IBuyerScopedProcessor scoped)
        {
            await scoped.SetBuyer(buyerId, ct);
        }

        await processor.ProcessAsync(path, ct);
    }
}

public sealed class BuyerForecastHandler : IIngestionHandler
{
    private readonly Func<LnrForecastProcessor> _proc;
    public BuyerForecastHandler(Func<LnrForecastProcessor> proc) => _proc = proc;
    public FileKind Kind => FileKind.BuyerForecast;
    public bool RequiresPartyId => true;
    public async Task HandleAsync(string path, int buyerId, object? signatureMap, CancellationToken ct)
    {
        var p = _proc();
        await p.SetBuyer(buyerId, ct);
        await p.ProcessAsync(path, ct);
    }
}

public sealed class VendorForecastHandler : IIngestionHandler
{
    private readonly Func<LibertyForecastProcessor> _proc;
    public VendorForecastHandler(Func<LibertyForecastProcessor> proc) => _proc = proc;
    public FileKind Kind => FileKind.VendorForecast;
    public bool RequiresPartyId => true;
    public async Task HandleAsync(string path, int vendorId, object? signatureMap, CancellationToken ct)
    {
        var p = _proc();
        await p.SetVendor(vendorId, ct);
        await p.ProcessAsync(path, ct);
    }
}

public sealed class MainframeInventoryHandler : IIngestionHandler
{
    private readonly Func<MainframeInventoryProcessor> _proc;
    public MainframeInventoryHandler(Func<MainframeInventoryProcessor> proc) => _proc = proc;
    public FileKind Kind => FileKind.MainframeInventory;
    public bool RequiresPartyId => false;
    public Task HandleAsync(string path, int partyId, object? signatureMap, CancellationToken ct) => _proc().ProcessAsync(path, ct);
}

public sealed class UpcCompHandler : IIngestionHandler
{
    private readonly Func<UpcCompProcessor> _proc;
    public UpcCompHandler(Func<UpcCompProcessor> proc) => _proc = proc;
    public FileKind Kind => FileKind.UpcComp;
    public bool RequiresPartyId => false;
    public Task HandleAsync(string path, int partyId, object? signatureMap, CancellationToken ct) => _proc().ProcessAsync(path, ct);
}
