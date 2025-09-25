using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diamond.Procurement.Domain.Models
{
    public sealed class BuyerSoldVendorNotInMasterRow
    {
        public string Upc { get; init; } = "";
        public string Description { get; init; } = "";
        public int BuyerInventory { get; init; }
        public int SoldLastYear { get; init; }
        public int SoldYtd { get; init; }
        public int VendorQty { get; set; }
    }
}
