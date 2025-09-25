using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DevExpress.Export;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraPrinting;

namespace Diamond.Procurement.Win.Helpers;

public static class GridExportHelpers
{
    /// <summary>
    /// Exports the given AdvBandedGridView to an .xlsx file using WYSIWYG (preserves colors, fonts, band headers, etc.).
    /// The file is written to the current user's "My Documents" folder and the user is prompted to open it.
    /// </summary>
    public static void ExportAdvBandedViewToExcel(AdvBandedGridView view, string? baseFileName = null)
    {
        if (view == null) throw new ArgumentNullException(nameof(view));
        GridControl grid = view.GridControl ?? throw new InvalidOperationException("The view is not attached to a GridControl.");

        try
        {
            // Ensure the export uses the same visual styles used for printing/export
            view.OptionsPrint.UsePrintStyles = false;
            view.OptionsPrint.AutoWidth = false;

            // Build output path in "My Documents"
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string safeName = MakeSafeFileName(baseFileName ?? view.Name ?? "GridExport");
            string fileName = $"{safeName}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            string fullPath = Path.Combine(docs, fileName);

            // Export options: WYSIWYG preserves on-screen formatting & colors
            var xlsx = new XlsxExportOptionsEx
            {
                ExportType = ExportType.WYSIWYG,   // key to preserve appearance/colors
                TextExportMode = TextExportMode.Value                
            };

            // Perform export
            grid.ExportToXlsx(fullPath, xlsx);

            // Prompt to open the file
            var dr = MessageBox.Show(
                $"Exported to:\n{fullPath}\n\nOpen it now?",
                "Export Complete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Export failed:\n\n" + ex.Message,
                "Export Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static string MakeSafeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return string.IsNullOrWhiteSpace(name) ? "Export" : name;
    }
}
