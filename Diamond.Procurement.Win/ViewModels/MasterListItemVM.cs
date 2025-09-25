using Diamond.Procurement.Domain.Models;
using System.ComponentModel;

namespace Diamond.Procurement.Win.ViewModels
{
    public sealed class MasterListItemVM
    {
        // dirty flag for this specific field
        public bool IsAltBuyerDirty { get; private set; }

        public int MasterListDetailId { get; set; }   // for future inline ops
        public int MasterListId { get; set; }
        public int UpcId { get; set; }
        public string Upc { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }

        private bool _hasAlternateBuyer;
        public bool HasAlternateBuyer
        {
            get => _hasAlternateBuyer;
            set
            {
                if (_hasAlternateBuyer == value) return;
                _hasAlternateBuyer = value;
                IsAltBuyerDirty = true;     // mark dirty when user toggles
                OnPropertyChanged(nameof(HasAlternateBuyer));
            }
        }

        public MasterListItemVM(MasterListItemRow r)
        {
            MasterListDetailId = r.MasterListDetailId;
            UpcId = r.UpcId;
            Upc = r.Upc;
            Description = r.Description;
            IsActive = r.IsActive;
            DateAdded = r.DateAdded;
            DateRemoved = r.DateRemoved;
            HasAlternateBuyer = r.HasAlternateBuyer;
            IsAltBuyerDirty = false;
        }

        public void ClearDirty() => IsAltBuyerDirty = false;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
