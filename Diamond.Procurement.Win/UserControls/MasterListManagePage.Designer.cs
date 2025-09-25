namespace Diamond.Procurement.Win.UserControls
{
    partial class MasterListManagePage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterListManagePage));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            btnSave = new DevExpress.XtraEditors.SimpleButton();
            btnRemoveSelected = new DevExpress.XtraEditors.SimpleButton();
            btnAddSingle = new DevExpress.XtraEditors.SimpleButton();
            btnBulkPaste = new DevExpress.XtraEditors.SimpleButton();
            lueMasterList = new DevExpress.XtraEditors.LookUpEdit();
            btnRefreshData = new DevExpress.XtraEditors.SimpleButton();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            barManager1 = new DevExpress.XtraBars.BarManager(components);
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            bbiDiscontinued = new DevExpress.XtraBars.BarButtonItem();
            popupMenu1 = new DevExpress.XtraBars.PopupMenu(components);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lueMasterList.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)popupMenu1).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(btnSave);
            layoutControl1.Controls.Add(btnRemoveSelected);
            layoutControl1.Controls.Add(btnAddSingle);
            layoutControl1.Controls.Add(btnBulkPaste);
            layoutControl1.Controls.Add(lueMasterList);
            layoutControl1.Controls.Add(btnRefreshData);
            layoutControl1.Controls.Add(gridControl1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(1085, 608);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            btnSave.AutoWidthInLayoutControl = true;
            btnSave.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnSave.ImageOptions.SvgImage");
            btnSave.ImageOptions.SvgImageSize = new Size(16, 16);
            btnSave.Location = new Point(1011, 570);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6, 2, 6, 2);
            btnSave.Size = new Size(62, 26);
            btnSave.StyleController = layoutControl1;
            btnSave.TabIndex = 7;
            btnSave.Text = "Save";
            // 
            // btnRemoveSelected
            // 
            btnRemoveSelected.AutoWidthInLayoutControl = true;
            btnRemoveSelected.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnRemoveSelected.ImageOptions.SvgImage");
            btnRemoveSelected.ImageOptions.SvgImageSize = new Size(16, 16);
            btnRemoveSelected.Location = new Point(619, 45);
            btnRemoveSelected.Name = "btnRemoveSelected";
            btnRemoveSelected.Padding = new Padding(6, 0, 6, 0);
            btnRemoveSelected.Size = new Size(139, 22);
            btnRemoveSelected.StyleController = layoutControl1;
            btnRemoveSelected.TabIndex = 5;
            btnRemoveSelected.Text = "Mark as Discontinued";
            // 
            // btnAddSingle
            // 
            btnAddSingle.AutoWidthInLayoutControl = true;
            btnAddSingle.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddSingle.ImageOptions.SvgImage");
            btnAddSingle.ImageOptions.SvgImageSize = new Size(16, 16);
            btnAddSingle.Location = new Point(477, 45);
            btnAddSingle.Name = "btnAddSingle";
            btnAddSingle.Padding = new Padding(6, 0, 6, 0);
            btnAddSingle.Size = new Size(120, 22);
            btnAddSingle.StyleController = layoutControl1;
            btnAddSingle.TabIndex = 4;
            btnAddSingle.Text = "Add New (Single)";
            // 
            // btnBulkPaste
            // 
            btnBulkPaste.AutoWidthInLayoutControl = true;
            btnBulkPaste.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnBulkPaste.ImageOptions.SvgImage");
            btnBulkPaste.ImageOptions.SvgImageSize = new Size(16, 16);
            btnBulkPaste.Location = new Point(348, 45);
            btnBulkPaste.Name = "btnBulkPaste";
            btnBulkPaste.Padding = new Padding(6, 0, 6, 0);
            btnBulkPaste.Size = new Size(125, 22);
            btnBulkPaste.StyleController = layoutControl1;
            btnBulkPaste.TabIndex = 3;
            btnBulkPaste.Text = " Bulk Add/Remove";
            // 
            // lueMasterList
            // 
            lueMasterList.Location = new Point(81, 46);
            lueMasterList.Name = "lueMasterList";
            lueMasterList.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
            lueMasterList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            lueMasterList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MasterListId", "Name1", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BuyerName", "Buyer"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("VendorName", "Vendor") });
            lueMasterList.Properties.DisplayMember = "Name";
            lueMasterList.Properties.NullText = "";
            lueMasterList.Properties.ValueMember = "MasterListId";
            lueMasterList.Size = new Size(140, 20);
            lueMasterList.StyleController = layoutControl1;
            lueMasterList.TabIndex = 0;
            // 
            // btnRefreshData
            // 
            btnRefreshData.AutoWidthInLayoutControl = true;
            btnRefreshData.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnRefreshData.ImageOptions.SvgImage");
            btnRefreshData.ImageOptions.SvgImageSize = new Size(16, 16);
            btnRefreshData.Location = new Point(225, 45);
            btnRefreshData.Name = "btnRefreshData";
            btnRefreshData.Padding = new Padding(2, 0, 2, 0);
            btnRefreshData.Size = new Size(101, 22);
            btnRefreshData.StyleController = layoutControl1;
            btnRefreshData.TabIndex = 2;
            btnRefreshData.Text = "&Refresh Data";
            // 
            // gridControl1
            // 
            gridControl1.Location = new Point(12, 83);
            gridControl1.MainView = gridView1;
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new Size(1061, 483);
            gridControl1.TabIndex = 6;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            gridView1.OptionsView.ShowFooter = true;
            gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem3, layoutControlGroup1, layoutControlItem4, emptySpaceItem1 });
            Root.Name = "Root";
            Root.Size = new Size(1085, 608);
            Root.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = gridControl1;
            layoutControlItem3.Location = new Point(0, 71);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(1065, 487);
            layoutControlItem3.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem6, layoutControlItem7, emptySpaceItem2, layoutControlItem1, layoutControlItem2, layoutControlItem5 });
            layoutControlGroup1.Location = new Point(0, 0);
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(1065, 71);
            layoutControlGroup1.Text = "Options";
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = btnRefreshData;
            layoutControlItem6.Location = new Point(201, 0);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(105, 26);
            layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            layoutControlItem7.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            layoutControlItem7.Control = lueMasterList;
            layoutControlItem7.Location = new Point(0, 0);
            layoutControlItem7.MaxSize = new Size(201, 24);
            layoutControlItem7.MinSize = new Size(201, 24);
            layoutControlItem7.Name = "layoutControlItem7";
            layoutControlItem7.Size = new Size(201, 26);
            layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem7.Text = "Master List";
            layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            layoutControlItem7.TextSize = new Size(52, 13);
            layoutControlItem7.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.Location = new Point(738, 0);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(303, 26);
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = btnBulkPaste;
            layoutControlItem1.Location = new Point(306, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 2, 2, 2);
            layoutControlItem1.Size = new Size(147, 26);
            layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = btnAddSingle;
            layoutControlItem2.Location = new Point(453, 0);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(124, 26);
            layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = btnRemoveSelected;
            layoutControlItem5.Location = new Point(577, 0);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 2, 2, 2);
            layoutControlItem5.Size = new Size(161, 26);
            layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = btnSave;
            layoutControlItem4.Location = new Point(999, 558);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new Size(66, 30);
            layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.Location = new Point(0, 558);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(999, 30);
            // 
            // barManager1
            // 
            barManager1.DockControls.Add(barDockControlTop);
            barManager1.DockControls.Add(barDockControlBottom);
            barManager1.DockControls.Add(barDockControlLeft);
            barManager1.DockControls.Add(barDockControlRight);
            barManager1.Form = this;
            barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { bbiDiscontinued });
            barManager1.MaxItemId = 1;
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = DockStyle.Top;
            barDockControlTop.Location = new Point(0, 0);
            barDockControlTop.Manager = barManager1;
            barDockControlTop.Size = new Size(1085, 0);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = DockStyle.Bottom;
            barDockControlBottom.Location = new Point(0, 608);
            barDockControlBottom.Manager = barManager1;
            barDockControlBottom.Size = new Size(1085, 0);
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = DockStyle.Left;
            barDockControlLeft.Location = new Point(0, 0);
            barDockControlLeft.Manager = barManager1;
            barDockControlLeft.Size = new Size(0, 608);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = DockStyle.Right;
            barDockControlRight.Location = new Point(1085, 0);
            barDockControlRight.Manager = barManager1;
            barDockControlRight.Size = new Size(0, 608);
            // 
            // bbiDiscontinued
            // 
            bbiDiscontinued.AccessibleName = "miRemoveSelected";
            bbiDiscontinued.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            bbiDiscontinued.Caption = "Mark as Discontinued";
            bbiDiscontinued.Hint = null;
            bbiDiscontinued.Id = 0;
            bbiDiscontinued.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("bbiDiscontinued.ImageOptions.SvgImage");
            bbiDiscontinued.ImageOptions.SvgImageSize = new Size(16, 16);
            bbiDiscontinued.Name = "bbiDiscontinued";
            bbiDiscontinued.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // popupMenu1
            // 
            popupMenu1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(bbiDiscontinued) });
            popupMenu1.Manager = barManager1;
            popupMenu1.Name = "popupMenu1";
            // 
            // MasterListManagePage
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(layoutControl1);
            Controls.Add(barDockControlLeft);
            Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom);
            Controls.Add(barDockControlTop);
            Name = "MasterListManagePage";
            Size = new Size(1085, 608);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)lueMasterList.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).EndInit();
            ((System.ComponentModel.ISupportInitialize)popupMenu1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private ToolStripMenuItem miCopyFiltered;
        private DevExpress.XtraEditors.SimpleButton btnRefreshData;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LookUpEdit lueMasterList;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SimpleButton btnRemoveSelected;
        private DevExpress.XtraEditors.SimpleButton btnAddSingle;
        private DevExpress.XtraEditors.SimpleButton btnBulkPaste;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem miExcel;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem miBumpUp;
        private ToolStripMenuItem miClearExtra;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbiDiscontinued;
        private DevExpress.XtraBars.PopupMenu popupMenu1;
    }
}
