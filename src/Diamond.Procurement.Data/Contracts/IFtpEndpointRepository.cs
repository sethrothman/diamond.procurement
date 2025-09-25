using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond.Procurement.Data.Contracts
{
    public interface IFtpEndpointRepository
    {
        Task<FtpEndpoint?> GetActiveAsync(CancellationToken ct = default);
    }
}
