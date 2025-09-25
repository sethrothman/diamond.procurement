using Diamond.Procurement.Domain.Enums;

namespace Diamond.Procurement.Win.GridSchemas
{
    public interface IListTypeGridSchemaFactory
    {
        IListTypeGridSchema Resolve(ListTypeId listTypeId);
    }

    public sealed class ListTypeGridSchemaFactory : IListTypeGridSchemaFactory
    {
        private static readonly IListTypeGridSchema _cosmetics = new CosmeticsGridSchema();
        private static readonly IListTypeGridSchema _haircare = new HaircareGridSchema();

        public IListTypeGridSchema Resolve(ListTypeId listTypeId) => listTypeId switch
        {
            ListTypeId.Haircare => _haircare,
            _ => _cosmetics  // default/fallback
        };
    }
}
