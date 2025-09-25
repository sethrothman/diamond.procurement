using System;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace Diamond.Procurement.Win.Helpers;

public sealed class BusyScope : IDisposable
{
    private readonly IOverlaySplashScreenHandle? _overlay;
    private readonly Control _owner;

    private BusyScope(Control owner)
    {
        _owner = owner;
        Application.UseWaitCursor = true;
        _overlay = SplashScreenManager.ShowOverlayForm(owner); // nice dim overlay + spinner
    }

    public static BusyScope Show(Control owner) => new BusyScope(owner);

    public void Dispose()
    {
        if (_overlay != null)
            SplashScreenManager.CloseOverlayForm(_overlay);
        Application.UseWaitCursor = false;
    }
}
