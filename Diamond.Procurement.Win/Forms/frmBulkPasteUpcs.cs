using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diamond.Procurement.Win.Forms
{
    public partial class frmBulkPasteUpcs : DevExpress.XtraEditors.XtraForm
    {
        public List<string> UpcsToAdd { get; private set; } = [];
        public List<string> UpcsToRemove { get; private set; } = [];

        public frmBulkPasteUpcs()
        {
            InitializeComponent();

            Text = "Bulk UPC Actions";
            Width = 800; Height = 500;

            txtAdd.EditValueChanged += (_, __) => UpdateCounts();
            txtRemove.EditValueChanged += (_, __) => UpdateCounts();

            btnOk.Click += (_, __) =>
            {
                UpcsToAdd = Parse(txtAdd.Text);
                UpcsToRemove = Parse(txtRemove.Text);
                DialogResult = DialogResult.OK;
            };
            btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;

            UpdateCounts();
        }

        private void UpdateCounts()
        {
            var a = Parse(txtAdd.Text).Count;
            var r = Parse(txtRemove.Text).Count;
            
            layoutControlItemAdd.Text = $"Paste UPCs to <b>Add</b><br><br>({a:n0} unique UPCs)";
            layoutControlItemRemove.Text = $"Paste UPCs to <b>Remove</b><br><br>({r:n0} unique UPCs)";
        }

        // robust parse: keep digits only, but don’t lose leading zeroes (we strip non-digits)
        private static List<string> Parse(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return new();
            // split on any non-digit; filter to length 8-18 (tweak as needed)
            var parts = Regex.Split(s, @"\D+")
                             .Where(x => !string.IsNullOrEmpty(x))
                             .Select(x => x.Trim())
                             .Where(x => x.Length >= 8 && x.Length <= 18) // configurable
                             .Distinct()
                             .ToList();
            return parts;
        }
    }
}