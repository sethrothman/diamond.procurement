using DevExpress.XtraSplashScreen;

namespace Diamond.Procurement.Win.Helpers
{
    /// <summary>
    /// Runs work while showing the DevExpress overlay spinner over a host control.
    /// </summary>
    public static class OverlayHelper
    {
        /// <summary>
        /// Shows an overlay on <paramref name="host"/> while <paramref name="work"/> runs.
        /// The overlay is always closed (even if <paramref name="work"/> throws).
        /// </summary>
        public static async Task RunAsync(Control host, Func<Task> work)
        {
            if (host is null) throw new ArgumentNullException(nameof(host));
            if (work is null) throw new ArgumentNullException(nameof(work));

            IOverlaySplashScreenHandle? overlay = null;
            try
            {
                overlay = SplashScreenManager.ShowOverlayForm(host);
                await work().ConfigureAwait(true); // stay on UI ctx for typical WinForms usage
            }
            finally
            {
                if (overlay != null)
                    SplashScreenManager.CloseOverlayForm(overlay);
            }
        }

        /// <summary>
        /// Synchronous convenience overload.
        /// </summary>
        public static void Run(Control host, Action work)
        {
            if (host is null) throw new ArgumentNullException(nameof(host));
            if (work is null) throw new ArgumentNullException(nameof(work));

            IOverlaySplashScreenHandle? overlay = null;
            try
            {
                overlay = SplashScreenManager.ShowOverlayForm(host);
                work();
            }
            finally
            {
                if (overlay != null)
                    SplashScreenManager.CloseOverlayForm(overlay);
            }
        }
    }
}
