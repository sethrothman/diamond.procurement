namespace Diamond.Procurement.Win.GridSchemas
{
    // Which built-in action a menu item triggers (page has the private handlers)
    public enum ContextAction
    {
        CopyProposedToQty,  // Cosmetics
        AddExtraCases,      // Cosmetics
        ClearExtraCases,    // Cosmetics
        CopyWeeksToQty,      // Haircare
        CopyOrderedToConfirmed //Both
    }

    public sealed class MenuItemSpec
    {
        public ContextAction Action { get; init; }
        public string Caption { get; init; } = "";
        public bool Visible { get; init; } = true;
    }

    public interface IListTypeMenuSpecProvider
    {
        // Static spec (captions/visibility) for this ListType
        IReadOnlyList<MenuItemSpec> GetMenuSpec();
        // Simple enable rule: usually selectedCount > 0
        bool ShouldEnable(ContextAction action, int selectedCount);
    }

    public sealed class CosmeticsMenuSpecProvider : IListTypeMenuSpecProvider
    {
        private static readonly MenuItemSpec[] _spec =
        {
            new() { Action = ContextAction.CopyProposedToQty, Caption = "Copy Proposed → Qty", Visible = true },
            new() { Action = ContextAction.CopyOrderedToConfirmed, Caption = "Copy Ordered → Confirmed", Visible = true },
            new() { Action = ContextAction.AddExtraCases, Caption = "Add Extra Cases",      Visible = true },
            new() { Action = ContextAction.ClearExtraCases, Caption = "Clear Extra Cases",    Visible = true },
            new() { Action = ContextAction.CopyWeeksToQty, Caption = "", Visible = false },
        };

        public IReadOnlyList<MenuItemSpec> GetMenuSpec() => _spec;
        public bool ShouldEnable(ContextAction action, int selectedCount) => selectedCount > 0;
    }

    public sealed class HaircareMenuSpecProvider : IListTypeMenuSpecProvider
    {
        private static readonly MenuItemSpec[] _spec =
        {
            new() { Action = ContextAction.CopyWeeksToQty, Caption = "Copy Proposed → Qty", Visible = true },
            new() { Action = ContextAction.CopyOrderedToConfirmed, Caption = "Copy Ordered → Confirmed", Visible = true },
            // Cosmetics-only items hidden
            new() { Action = ContextAction.CopyProposedToQty, Caption = "", Visible = false },
            new() { Action = ContextAction.AddExtraCases, Caption = "", Visible = false },
            new() { Action = ContextAction.ClearExtraCases, Caption = "", Visible = false },
        };

        public IReadOnlyList<MenuItemSpec> GetMenuSpec() => _spec;
        public bool ShouldEnable(ContextAction action, int selectedCount) => selectedCount > 0;
    }

    public sealed class MenuSpecFactory
    {
        private readonly CosmeticsMenuSpecProvider _cos = new();
        private readonly HaircareMenuSpecProvider _hair = new();

        public IListTypeMenuSpecProvider Resolve(Diamond.Procurement.Domain.Enums.ListTypeId listTypeId)
            => listTypeId == Diamond.Procurement.Domain.Enums.ListTypeId.Haircare ? _hair : _cos;
    }
}
