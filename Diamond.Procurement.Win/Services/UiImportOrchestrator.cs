using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.Win.Services
{
    public sealed class UiImportOrchestrator : IUiImportOrchestrator
    {
        private readonly IIngestionDispatcher _dispatcher;

        public UiImportOrchestrator(IIngestionDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task RunAllAsync(
            IReadOnlyDictionary<FileKind, StepImportSlot> steps,
            IProgress<ImportProgress>? progress,
            CancellationToken ct)
        {
            // Define the order you want to process (adjust if you have a required sequence)
            var order = new[]
            {
                FileKind.BuyerInventory,
                FileKind.BuyerForecast,
                FileKind.VendorForecast,
                FileKind.MainframeInventory,
                FileKind.UpcComp                
            };

            foreach (var kind in order)
            {
                if (!steps.TryGetValue(kind, out var slot) || string.IsNullOrWhiteSpace(slot.FilePath))
                    continue;

                try
                {
                    // Start
                    progress?.Report(new ImportProgress(
                        Kind: kind,
                        Message: $"Starting {kind} import…",
                        IsDone: false,
                        IsSuccess: true,
                        Error: null,
                        Rows: null
                    ));

                    // Dispatch into the real pipeline
                    // If DispatchAsync can return a row count, capture it and pass as Rows.
                    // e.g., var rows = await _dispatcher.DispatchAsync(...);
                    await _dispatcher.DispatchAsync(kind, slot.FilePath, slot.PartyId, slot.SignatureMap, ct);
                    int? rows = null; // set this if you can obtain a count

                    // Complete
                    progress?.Report(new ImportProgress(
                        Kind: kind,
                        Message: $"{kind} import complete.",
                        IsDone: true,
                        IsSuccess: true,
                        Error: null,
                        Rows: rows
                    ));
                }
                catch (OperationCanceledException)
                {
                    progress?.Report(new ImportProgress(
                        Kind: kind,
                        Message: $"{kind} canceled.",
                        IsDone: true,
                        IsSuccess: false,
                        Error: null,
                        Rows: null
                    ));
                    throw;
                }
                catch (Exception ex)
                {
                    progress?.Report(new ImportProgress(
                        Kind: kind,
                        Message: $"{kind} failed: {ex.Message}",
                        IsDone: true,
                        IsSuccess: false,
                        Error: ex.ToString(),
                        Rows: null
                    ));
                    throw;
                }
            }
        }
    }
}
