using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms; // for Control, Keys, Timer
using System.Xml;
using DevExpress.Data.Filtering;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Diamond.Procurement.Win.Helpers
{
    public interface IGridSettingsPersistence
    {
        void RestoreSettings(GridControl gridControl, string uniqueId);
        void SaveSettings(GridControl gridControl, string uniqueId);
        bool SettingsExist(GridControl gridControl, string uniqueId);
        void ClearSettings(GridControl gridControl, string uniqueId);
    }

    /// <summary>
    /// Persists specific DevExpress GridControl settings (filters, sort order, find text) between sessions.
    /// Settings file name now includes the host (Form/UserControl) and GridControl.Name, plus a hash of uniqueId.
    /// Files are stored in %APPDATA%\[CompanyName]\GridSettings.
    /// </summary>
    public sealed class GridSettingsPersistence : IGridSettingsPersistence
    {
        private readonly string _baseDir;
        private const string DEFAULT_COMPANY = "DiamondProcurement";
        private const string SETTINGS_FOLDER = "GridSettings";

        public GridSettingsPersistence(string companyName = DEFAULT_COMPANY)
        {
            var appDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _baseDir = Path.Combine(appDataRoot, companyName, SETTINGS_FOLDER);
            Directory.CreateDirectory(_baseDir);
        }

        public void RestoreSettings(GridControl gridControl, string uniqueId)
        {
            if (gridControl?.MainView == null) return;

            // Skip restoring if SHIFT held down
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                ClearGridSettings(gridControl);
                ClearSettings(gridControl, uniqueId);
                return;
            }

            var settingsPath = GetSettingsPath(gridControl, uniqueId);
            if (!File.Exists(settingsPath)) return;

            try
            {
                var settings = LoadSettings(settingsPath);
                ApplySettingsToGrid(gridControl, settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error restoring grid settings: {ex.Message}");
            }
        }

        public void SaveSettings(GridControl gridControl, string uniqueId)
        {
            if (gridControl?.MainView == null) return;

            try
            {
                var settings = ExtractSettingsFromGrid(gridControl);
                var settingsPath = GetSettingsPath(gridControl, uniqueId);
                SaveSettings(settings, settingsPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving grid settings: {ex.Message}");
            }
        }

        public bool SettingsExist(GridControl gridControl, string uniqueId)
        {
            return File.Exists(GetSettingsPath(gridControl, uniqueId));
        }

        public void ClearSettings(GridControl gridControl, string uniqueId)
        {
            var settingsPath = GetSettingsPath(gridControl, uniqueId);
            try
            {
                if (File.Exists(settingsPath))
                {
                    File.Delete(settingsPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing grid settings: {ex.Message}");
            }
        }

        private void ClearGridSettings(GridControl gridControl)
        {
            var view = gridControl.MainView;
            if (view is GridView gridView)
            {
                gridView.BeginUpdate();
                try
                {
                    gridView.ClearSorting();
                    gridView.ActiveFilterCriteria = null;
                    gridView.ClearFindFilter();
                    gridView.FindFilterText = string.Empty;
                }
                finally
                {
                    gridView.EndUpdate();
                }
            }
        }

        private GridSettings ExtractSettingsFromGrid(GridControl gridControl)
        {
            var settings = new GridSettings();
            var view = gridControl.MainView;

            if (view is GridView gridView)
            {
                if (gridView.ActiveFilterCriteria != null)
                {
                    settings.FilterCriteria = CriteriaOperator.ToString(gridView.ActiveFilterCriteria);
                }

                if (gridView.SortInfo.Count > 0)
                {
                    var sortInfos = new SortInfo[gridView.SortInfo.Count];
                    for (int i = 0; i < gridView.SortInfo.Count; i++)
                    {
                        var sortInfo = gridView.SortInfo[i];
                        sortInfos[i] = new SortInfo
                        {
                            FieldName = sortInfo.Column?.FieldName ?? string.Empty,
                            SortOrder = sortInfo.SortOrder.ToString()
                        };
                    }
                    settings.SortInfo = sortInfos;
                }

                if (!string.IsNullOrEmpty(gridView.FindFilterText))
                {
                    settings.FindText = gridView.FindFilterText;
                }
            }

            return settings;
        }

        private void ApplySettingsToGrid(GridControl gridControl, GridSettings settings)
        {
            var view = gridControl.MainView;

            if (view is GridView gridView)
            {
                gridView.BeginUpdate();
                try
                {
                    gridView.ClearSorting();
                    gridView.ActiveFilterCriteria = null;
                    gridView.FindFilterText = string.Empty;

                    if (!string.IsNullOrEmpty(settings.FilterCriteria))
                    {
                        try
                        {
                            var criteria = CriteriaOperator.Parse(settings.FilterCriteria);
                            gridView.ActiveFilterCriteria = criteria;
                        }
                        catch
                        {
                            // ignore invalid filters
                        }
                    }

                    if (settings.SortInfo != null && settings.SortInfo.Length > 0)
                    {
                        foreach (var sortInfo in settings.SortInfo)
                        {
                            var column = gridView.Columns[sortInfo.FieldName];
                            if (column != null &&
                                Enum.TryParse(sortInfo.SortOrder, out DevExpress.Data.ColumnSortOrder sortOrder))
                            {
                                column.SortOrder = sortOrder;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(settings.FindText))
                    {
                        var findText = settings.FindText;
                        var timer = new System.Windows.Forms.Timer { Interval = 50 };
                        timer.Tick += (s, e) =>
                        {
                            timer.Stop();
                            timer.Dispose();
                            try
                            {
                                gridView.ApplyFindFilter(findText);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error applying find filter: {ex.Message}");
                                gridView.FindFilterText = findText;
                            }
                        };
                        timer.Start();
                    }
                }
                finally
                {
                    gridView.EndUpdate();
                }
            }
        }

        private GridSettings LoadSettings(string filePath)
        {
            var settings = new GridSettings();
            var doc = new XmlDocument();
            doc.Load(filePath);

            var root = doc.DocumentElement;
            if (root?.Name != "GridSettings") return settings;

            var filterNode = root.SelectSingleNode("FilterCriteria");
            if (filterNode != null) settings.FilterCriteria = filterNode.InnerText;

            var sortNode = root.SelectSingleNode("SortInfo");
            if (sortNode?.HasChildNodes == true)
            {
                var list = new System.Collections.Generic.List<SortInfo>();
                foreach (XmlNode n in sortNode.ChildNodes)
                {
                    if (n.Name == "SortItem")
                    {
                        var fieldName = n.Attributes?["FieldName"]?.Value ?? string.Empty;
                        var sortOrder = n.Attributes?["SortOrder"]?.Value ?? string.Empty;
                        if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(sortOrder))
                            list.Add(new SortInfo { FieldName = fieldName, SortOrder = sortOrder });
                    }
                }
                settings.SortInfo = list.ToArray();
            }

            var findNode = root.SelectSingleNode("FindText");
            if (findNode != null) settings.FindText = findNode.InnerText;

            return settings;
        }

        private void SaveSettings(GridSettings settings, string filePath)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("GridSettings");
            doc.AppendChild(root);

            if (!string.IsNullOrEmpty(settings.FilterCriteria))
            {
                var filterNode = doc.CreateElement("FilterCriteria");
                filterNode.InnerText = settings.FilterCriteria;
                root.AppendChild(filterNode);
            }

            if (settings.SortInfo != null && settings.SortInfo.Length > 0)
            {
                var sortNode = doc.CreateElement("SortInfo");
                foreach (var si in settings.SortInfo)
                {
                    var item = doc.CreateElement("SortItem");
                    item.SetAttribute("FieldName", si.FieldName);
                    item.SetAttribute("SortOrder", si.SortOrder);
                    sortNode.AppendChild(item);
                }
                root.AppendChild(sortNode);
            }

            if (!string.IsNullOrEmpty(settings.FindText))
            {
                var findNode = doc.CreateElement("FindText");
                findNode.InnerText = settings.FindText;
                root.AppendChild(findNode);
            }

            doc.Save(filePath);
        }

        private string GetSettingsPath(GridControl gridControl, string uniqueId)
        {
            // Host detection (prefer Form; else nearest UserControl; else top parent)
            var host = ResolveHostControl(gridControl);
            var hostName = SanitizeForFileName(host?.Name) ?? "Host";
            var hostType = SanitizeForFileName(host?.GetType().Name) ?? "Control";

            var gridName = SanitizeForFileName(gridControl.Name);
            if (string.IsNullOrWhiteSpace(gridName)) gridName = "GridControl";

            // Short, stable hash across host+grid+uniqueId to avoid overlong filenames and ensure uniqueness
            var hash = GetHashSuffix($"{hostType}.{hostName}|{gridName}|{uniqueId}");

            var fileName = $"{hostType}__{hostName}__{gridName}__{hash}.xml";
            return Path.Combine(_baseDir, fileName);
        }

        private static Control? ResolveHostControl(Control? c)
        {
            if (c == null) return null;

            // Walk up to the Form if available
            var form = c.FindForm();
            if (form != null) return form;

            // Otherwise, try to find nearest UserControl up the chain
            var p = c.Parent;
            while (p != null)
            {
                if (p is UserControl) return p;
                p = p.Parent;
            }

            // Fallback to topmost parent
            p = c;
            while (p?.Parent != null) p = p.Parent;
            return p;
        }

        private static string? SanitizeForFileName(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            var invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(s.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            // Keep it reasonable
            if (cleaned.Length > 80) cleaned = cleaned.Substring(0, 80);
            return cleaned.Trim();
        }

        private static string GetHashSuffix(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            // 6 bytes = 12 hex chars; short but low collision risk for this scope
            var shortBytes = bytes.Take(6).ToArray();
            return BitConverter.ToString(shortBytes).Replace("-", "").ToLowerInvariant();
        }

        #region Helper classes

        private class GridSettings
        {
            public string FilterCriteria { get; set; } = string.Empty;
            public SortInfo[] SortInfo { get; set; } = Array.Empty<SortInfo>();
            public string FindText { get; set; } = string.Empty;
        }

        private class SortInfo
        {
            public string FieldName { get; set; } = string.Empty;
            public string SortOrder { get; set; } = string.Empty;
        }

        #endregion
    }

    /// <summary>
    /// Extension helpers. New string-based uniqueId overloads + int-based overloads for back-compat.
    /// </summary>
    public static class GridControlExtensions
    {
        private static readonly IGridSettingsPersistence _persistence = new GridSettingsPersistence();

        // Preferred (generic) API
        public static void RestoreSettings(this GridControl gridControl, string uniqueId)
            => _persistence.RestoreSettings(gridControl, uniqueId ?? string.Empty);

        public static void SaveSettings(this GridControl gridControl, string uniqueId)
            => _persistence.SaveSettings(gridControl, uniqueId ?? string.Empty);

        public static bool HasSavedSettings(this GridControl gridControl, string uniqueId)
            => _persistence.SettingsExist(gridControl, uniqueId ?? string.Empty);

        public static void ClearSavedSettings(this GridControl gridControl, string uniqueId)
            => _persistence.ClearSettings(gridControl, uniqueId ?? string.Empty);

        // Back-compat overloads (ints map to string)
        public static void RestoreSettings(this GridControl gridControl, int listTypeId)
            => _persistence.RestoreSettings(gridControl, listTypeId.ToString());

        public static void SaveSettings(this GridControl gridControl, int listTypeId)
            => _persistence.SaveSettings(gridControl, listTypeId.ToString());

        public static bool HasSavedSettings(this GridControl gridControl, int listTypeId)
            => _persistence.SettingsExist(gridControl, listTypeId.ToString());

        public static void ClearSavedSettings(this GridControl gridControl, int listTypeId)
            => _persistence.ClearSettings(gridControl, listTypeId.ToString());
    }
}
