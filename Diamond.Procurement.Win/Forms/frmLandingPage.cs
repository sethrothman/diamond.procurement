using DevExpress.XtraBars.Navigation;
using Diamond.Procurement.Data;
using Diamond.Procurement.Data.Repositories;
using Diamond.Procurement.Win.Helpers;
using Diamond.Procurement.Win.UserControls;
using Microsoft.Extensions.DependencyInjection;

namespace Diamond.Procurement.Win.Forms
{
    public partial class frmLandingPage : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        private readonly Dictionary<string, UserControl> _pageCache = new();
        private readonly IServiceProvider _sp;

        public frmLandingPage(IServiceProvider sp)
        {
            InitializeComponent();

            FormBoundsPersistence.Attach(this, "frmLandingPage");

            _sp = sp;

            // 2) Handle nav clicks
            accordionControl1.ElementClick += OnNavClick;
            accordionControl1.ExpandStateChanging += (s, e) => 
            {
                if (e.Element.Style == ElementStyle.Group && e.NewState == AccordionElementState.Collapsed)
                    e.ElementsToExpandCollapse.Clear();
            };

            // 3) Optional: set a default page at startup
            Shown += (_, __) => Navigate("imports"); // or "imports" if first run
        }

        private void OnNavClick(object sender, ElementClickEventArgs e)
        {
            try
            {
                Application.UseWaitCursor = true;
                Cursor.Current = Cursors.WaitCursor;

                if (e.Element.Style != ElementStyle.Item) return;

                var route = (string?)e.Element.Tag ?? e.Element.Text?.Trim().ToLowerInvariant();
                if (!string.IsNullOrEmpty(route))
                {
                    Navigate(route);
                }
            }
            finally
            {
                Application.UseWaitCursor = false;
                Cursor.Current = Cursors.Default;
            }
        }


        private void Navigate(string route)
        {
            if (!_pageCache.TryGetValue(route, out var uc))
            {
                uc = route switch
                {
                    "imports" => _sp.GetRequiredService<ImportPage>(),
                    "analysis" => CreateAndInitAnalysisPage(),  // <-- async init kicked off inside
                    "masterlist" => CreateAndInitMasterListPage(),
                    "notsellingyet" => CreateAndInitNotSellingYet(),
                    "shipments" => CreateAndInitShipments(),
                    _ => new PlaceholderPage(route),
                };

                uc.Dock = DockStyle.Fill;

                var page = new NavigationPage { Tag = route };
                page.Controls.Add(uc);
                navigationFrame1.Pages.Add(page);
                _pageCache[route] = uc;
            }

            navigationFrame1.SelectedPage = navigationFrame1.Pages
                .OfType<NavigationPage>()
                .First(p => Equals(p.Tag, route));

            SelectAccordionByRoute(route);
        }

        private void SelectAccordionByRoute(string route)
        {
            accordionControl1.BeginUpdate();
            try
            {
                var el = FindByRoute(accordionControl1.Elements, route);
                if (el == null) return;

                if (accordionControl1.SelectedElement != el)
                    accordionControl1.SelectedElement = el;
            }
            finally
            {
                accordionControl1.EndUpdate();
            }
        }

        private AccordionControlElement? FindByRoute(AccordionControlElementCollection nodes, string route)
        {
            foreach (AccordionControlElement el in nodes)
            {
                // Match by Tag first, then by Text (case-insensitive)
                var tag = el.Tag as string;
                var txt = el.Text?.Trim();

                if (string.Equals(tag, route, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(txt, route, StringComparison.OrdinalIgnoreCase))
                    return el;

                var child = FindByRoute(el.Elements, route);
                if (child != null) return child;
            }
            return null;
        }

        private UserControl CreateAndInitAnalysisPage()
        {
            var page = _sp.GetRequiredService<MasterListAnalysisPage>();        
            var repo = _sp.GetRequiredService<IOrderVendorRepository>();        
            var masterListRepo = _sp.GetRequiredService<MasterListRepository>();

            page.Configure(repo, masterListRepo);

            BeginInvoke(new Action(async () =>
            {
                try { await page.InitializeAsync(); }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to load Master List Analysis: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }));

            return page;
        }

        private UserControl CreateAndInitMasterListPage()
        {
            var page = _sp.GetRequiredService<MasterListManagePage>();
            var masterListRepo = _sp.GetRequiredService<MasterListRepository>();

            page.Configure(masterListRepo);

            BeginInvoke(new Action(async () =>
            {
                try { await page.InitializeAsync(); }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to load Master List: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }));

            return page;
        }

        private UserControl CreateAndInitNotSellingYet()
        {
            var page = _sp.GetRequiredService<BuyerSoldVendorNotInMaster>();
            var buyerRepo = _sp.GetRequiredService<BuyerInventoryRepository>();

            page.Configure(buyerRepo);

            BeginInvoke(new Action(async () =>
            {
                try { await page.InitializeAsync(); }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to load Items We're Not Selling Yet: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }));

            return page;
        }

        private UserControl CreateAndInitShipments()
        {
            var page = _sp.GetRequiredService<OrderVendorShipmentPage>();

            //page.Configure(0);

            BeginInvoke(new Action(async () =>
            {
                try { await page.InitializeAsync(); }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to load Shipments page: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }));

            return page;
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason != CloseReason.UserClosing &&
                e.CloseReason != CloseReason.FormOwnerClosing &&
                e.CloseReason != CloseReason.ApplicationExitCall)
                return;

            // If you have multiple pages, query all of them:
            var dirtyControls = _pageCache.Values.OfType<IDirtyAware>().ToList();

            foreach (var dc in dirtyControls)
            {
                var okToClose = await dc.PromptToSaveIfDirtyAsync(this);
                if (!okToClose)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}
