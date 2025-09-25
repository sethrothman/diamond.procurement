using DevExpress.Data.Helpers;
using DevExpress.XtraEditors;

namespace Diamond.Procurement.Win.UserControls
{
    public class PlaceholderPage : XtraUserControl
    {
        public PlaceholderPage(string route)
        {
            Dock = DockStyle.Fill;

            var lbl = new Label
            {
                Text = $"{Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(route)} page not implemented yet",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Regular)
            };

            Controls.Add(lbl);
        }
    }
}
