namespace Diamond.Procurement.Domain.Models
{
    public interface IUsesSignatureMap<in TMap>
    {
        void SetSignatureMap(TMap map);
    }
}
