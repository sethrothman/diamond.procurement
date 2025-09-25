using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.Win.Services
{
    public interface IUiImportOrchestrator
    {
        Task RunAllAsync(
            IReadOnlyDictionary<FileKind, StepImportSlot> steps, 
            IProgress<ImportProgress> progress,
            CancellationToken ct);
    }
}
