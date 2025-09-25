namespace Diamond.Procurement.Win.UserControls
{
    partial class OrderVendorShipmentPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderVendorShipmentPage));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            txtFilenameShipment = new DevExpress.XtraEditors.TextEdit();
            txtFilenameRemaining = new DevExpress.XtraEditors.TextEdit();
            btnExcel = new DevExpress.XtraEditors.SimpleButton();
            btnMainframeCsv = new DevExpress.XtraEditors.SimpleButton();
            gridMatrix = new DevExpress.XtraGrid.GridControl();
            viewMatrix = new DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView();
            gridBand1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            btnAddShipment = new DevExpress.XtraEditors.SimpleButton();
            lueOrderMonth = new DevExpress.XtraEditors.SearchLookUpEdit();
            searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtFilenameShipment.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtFilenameRemaining.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)viewMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lueOrderMonth.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)searchLookUpEdit1View).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem9).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(txtFilenameShipment);
            layoutControl1.Controls.Add(txtFilenameRemaining);
            layoutControl1.Controls.Add(btnExcel);
            layoutControl1.Controls.Add(btnMainframeCsv);
            layoutControl1.Controls.Add(gridMatrix);
            layoutControl1.Controls.Add(btnRefresh);
            layoutControl1.Controls.Add(btnAddShipment);
            layoutControl1.Controls.Add(lueOrderMonth);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(1263, 754);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // txtFilenameShipment
            // 
            txtFilenameShipment.Location = new Point(585, 719);
            txtFilenameShipment.Name = "txtFilenameShipment";
            txtFilenameShipment.Properties.NullValuePrompt = "For Items in Latest Shipment...";
            txtFilenameShipment.Size = new Size(174, 20);
            txtFilenameShipment.StyleController = layoutControl1;
            txtFilenameShipment.TabIndex = 8;
            // 
            // txtFilenameRemaining
            // 
            txtFilenameRemaining.Location = new Point(419, 719);
            txtFilenameRemaining.Name = "txtFilenameRemaining";
            txtFilenameRemaining.Properties.NullValuePrompt = "For Remaining Items...";
            txtFilenameRemaining.Size = new Size(154, 20);
            txtFilenameRemaining.StyleController = layoutControl1;
            txtFilenameRemaining.TabIndex = 7;
            // 
            // btnExcel
            // 
            btnExcel.AutoWidthInLayoutControl = true;
            btnExcel.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnExcel.ImageOptions.SvgImage");
            btnExcel.ImageOptions.SvgImageSize = new Size(16, 16);
            btnExcel.Location = new Point(12, 716);
            btnExcel.Name = "btnExcel";
            btnExcel.Padding = new Padding(6, 2, 6, 2);
            btnExcel.Size = new Size(114, 26);
            btnExcel.StyleController = layoutControl1;
            btnExcel.TabIndex = 6;
            btnExcel.Text = " Export to Excel";
            btnExcel.Click += btnExcel_Click;
            // 
            // btnMainframeCsv
            // 
            btnMainframeCsv.AutoWidthInLayoutControl = true;
            btnMainframeCsv.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnMainframeCsv.ImageOptions.SvgImage");
            btnMainframeCsv.ImageOptions.SvgImageSize = new Size(16, 16);
            btnMainframeCsv.Location = new Point(130, 716);
            btnMainframeCsv.Name = "btnMainframeCsv";
            btnMainframeCsv.Padding = new Padding(6, 2, 6, 2);
            btnMainframeCsv.Size = new Size(184, 26);
            btnMainframeCsv.StyleController = layoutControl1;
            btnMainframeCsv.TabIndex = 5;
            btnMainframeCsv.Text = "Generate &Mainframe PO Files";
            btnMainframeCsv.Click += btnMainframeCsv_Click;
            // 
            // gridMatrix
            // 
            gridMatrix.Location = new Point(12, 83);
            gridMatrix.MainView = viewMatrix;
            gridMatrix.Name = "gridMatrix";
            gridMatrix.Size = new Size(1239, 629);
            gridMatrix.TabIndex = 4;
            gridMatrix.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewMatrix });
            // 
            // viewMatrix
            // 
            viewMatrix.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gridBand1 });
            viewMatrix.GridControl = gridMatrix;
            viewMatrix.Name = "viewMatrix";
            viewMatrix.OptionsView.ShowFooter = true;
            // 
            // gridBand1
            // 
            gridBand1.Caption = "gridBand1";
            gridBand1.Name = "gridBand1";
            gridBand1.VisibleIndex = 0;
            // 
            // btnRefresh
            // 
            btnRefresh.AutoWidthInLayoutControl = true;
            btnRefresh.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnRefresh.ImageOptions.SvgImage");
            btnRefresh.ImageOptions.SvgImageSize = new Size(16, 16);
            btnRefresh.Location = new Point(531, 45);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Padding = new Padding(2, 0, 2, 0);
            btnRefresh.Size = new Size(97, 22);
            btnRefresh.StyleController = layoutControl1;
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = " Refresh Data";
            // 
            // btnAddShipment
            // 
            btnAddShipment.AutoWidthInLayoutControl = true;
            btnAddShipment.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddShipment.ImageOptions.SvgImage");
            btnAddShipment.ImageOptions.SvgImageSize = new Size(16, 16);
            btnAddShipment.Location = new Point(428, 45);
            btnAddShipment.Name = "btnAddShipment";
            btnAddShipment.Padding = new Padding(2, 0, 2, 0);
            btnAddShipment.Size = new Size(99, 22);
            btnAddShipment.StyleController = layoutControl1;
            btnAddShipment.TabIndex = 2;
            btnAddShipment.Text = " Add Shipment";
            // 
            // lueOrderMonth
            // 
            lueOrderMonth.Location = new Point(76, 46);
            lueOrderMonth.Name = "lueOrderMonth";
            lueOrderMonth.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            lueOrderMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            lueOrderMonth.Properties.DisplayMember = "DisplayForLookupEdit";
            lueOrderMonth.Properties.NullText = "(Select the Desired Order)";
            lueOrderMonth.Properties.PopupView = searchLookUpEdit1View;
            lueOrderMonth.Properties.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth;
            lueOrderMonth.Properties.ValueMember = "OrderVendorId";
            lueOrderMonth.Size = new Size(348, 20);
            lueOrderMonth.StyleController = layoutControl1;
            lueOrderMonth.TabIndex = 0;
            // 
            // searchLookUpEdit1View
            // 
            searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlGroup1, layoutControlItem2, layoutControlItem6, emptySpaceItem1, layoutControlItem7, layoutControlItem8, layoutControlItem9 });
            Root.Name = "Root";
            Root.Size = new Size(1263, 754);
            Root.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem3, layoutControlItem4, emptySpaceItem2 });
            layoutControlGroup1.Location = new Point(0, 0);
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(1243, 71);
            layoutControlGroup1.Text = "Options";
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            layoutControlItem1.Control = lueOrderMonth;
            layoutControlItem1.Location = new Point(0, 0);
            layoutControlItem1.MaxSize = new Size(404, 0);
            layoutControlItem1.MinSize = new Size(404, 25);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(404, 26);
            layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem1.Text = "Order For";
            layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            layoutControlItem1.TextSize = new Size(47, 13);
            layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = btnAddShipment;
            layoutControlItem3.Location = new Point(404, 0);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(103, 26);
            layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = btnRefresh;
            layoutControlItem4.Location = new Point(507, 0);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new Size(101, 26);
            layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.Location = new Point(608, 0);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(611, 26);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = gridMatrix;
            layoutControlItem2.Location = new Point(0, 71);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(1243, 633);
            layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = btnMainframeCsv;
            layoutControlItem6.Location = new Point(118, 704);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(188, 30);
            layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.Location = new Point(751, 704);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(492, 30);
            // 
            // layoutControlItem7
            // 
            layoutControlItem7.Control = btnExcel;
            layoutControlItem7.Location = new Point(0, 704);
            layoutControlItem7.Name = "layoutControlItem7";
            layoutControlItem7.Size = new Size(118, 30);
            layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            layoutControlItem8.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            layoutControlItem8.Control = txtFilenameRemaining;
            layoutControlItem8.Location = new Point(306, 704);
            layoutControlItem8.Name = "layoutControlItem8";
            layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
            layoutControlItem8.Size = new Size(259, 30);
            layoutControlItem8.Text = "Filenames for PO's";
            layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            layoutControlItem8.TextSize = new Size(88, 13);
            layoutControlItem8.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            layoutControlItem9.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            layoutControlItem9.Control = txtFilenameShipment;
            layoutControlItem9.Location = new Point(565, 704);
            layoutControlItem9.Name = "layoutControlItem9";
            layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
            layoutControlItem9.Size = new Size(186, 30);
            layoutControlItem9.Text = "Fil";
            layoutControlItem9.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            layoutControlItem9.TextToControlDistance = 0;
            layoutControlItem9.TextVisible = false;
            // 
            // OrderVendorShipmentPage
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(layoutControl1);
            Name = "OrderVendorShipmentPage";
            Size = new Size(1263, 754);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtFilenameShipment.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtFilenameRemaining.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridMatrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)viewMatrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)lueOrderMonth.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)searchLookUpEdit1View).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem8).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem9).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnAddShipment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.GridControl gridMatrix;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.SimpleButton btnMainframeCsv;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView viewMatrix;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand1;
        private DevExpress.XtraEditors.SimpleButton btnExcel;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.SearchLookUpEdit lueOrderMonth;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtFilenameRemaining;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.TextEdit txtFilenameShipment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
    }
}
