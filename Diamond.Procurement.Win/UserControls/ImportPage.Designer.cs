namespace Diamond.Procurement.Win.UserControls
{
    partial class ImportPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportPage));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            btnProcess = new DevExpress.XtraEditors.SimpleButton();
            txtLog = new DevExpress.XtraEditors.MemoEdit();
            stepProgressBar1 = new DevExpress.XtraEditors.StepProgressBar();
            stepBuyerInventory = new DevExpress.XtraEditors.StepProgressBarItem();
            stepBuyerForecast = new DevExpress.XtraEditors.StepProgressBarItem();
            stepVendorForecast = new DevExpress.XtraEditors.StepProgressBarItem();
            stepMainframe = new DevExpress.XtraEditors.StepProgressBarItem();
            stepUpcComp = new DevExpress.XtraEditors.StepProgressBarItem();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            svgImageCollection1 = new DevExpress.Utils.SvgImageCollection(components);
            barManager1 = new DevExpress.XtraBars.BarManager(components);
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            bbiFetch = new DevExpress.XtraBars.BarButtonItem();
            popupMenu1 = new DevExpress.XtraBars.PopupMenu(components);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtLog.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)stepProgressBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)svgImageCollection1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)popupMenu1).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(labelControl3);
            layoutControl1.Controls.Add(labelControl2);
            layoutControl1.Controls.Add(btnRefresh);
            layoutControl1.Controls.Add(labelControl1);
            layoutControl1.Controls.Add(btnProcess);
            layoutControl1.Controls.Add(txtLog);
            layoutControl1.Controls.Add(stepProgressBar1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(2744, 509, 650, 400);
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(1013, 607);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // labelControl3
            // 
            labelControl3.Appearance.Font = new Font("Tahoma", 10F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            labelControl3.Appearance.FontStyleDelta = FontStyle.Bold;
            labelControl3.Appearance.Options.UseFont = true;
            labelControl3.Location = new Point(12, 541);
            labelControl3.Name = "labelControl3";
            labelControl3.Padding = new Padding(0, 0, 0, 2);
            labelControl3.Size = new Size(39, 18);
            labelControl3.StyleController = layoutControl1;
            labelControl3.TabIndex = 6;
            labelControl3.Text = "NOTES";
            // 
            // labelControl2
            // 
            labelControl2.Appearance.FontSizeDelta = 1;
            labelControl2.Appearance.Options.UseFont = true;
            labelControl2.Location = new Point(12, 581);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new Size(950, 14);
            labelControl2.StyleController = layoutControl1;
            labelControl2.TabIndex = 5;
            labelControl2.Text = "• Right-click on 'Mainframe Inventory' and 'Comps Data' to retrieve the files directly via FTP (Mainframe Inventory is exported daily but Comps Data needs to be run manually)";
            // 
            // btnRefresh
            // 
            btnRefresh.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            btnRefresh.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnRefresh.ImageOptions.SvgImage");
            btnRefresh.ImageOptions.SvgImageSize = new Size(16, 16);
            btnRefresh.Location = new Point(903, 83);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(98, 22);
            btnRefresh.StyleController = layoutControl1;
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = " Refresh";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // labelControl1
            // 
            labelControl1.AllowHtmlString = true;
            labelControl1.Appearance.FontSizeDelta = 1;
            labelControl1.Appearance.Options.UseFont = true;
            labelControl1.Appearance.Options.UseTextOptions = true;
            labelControl1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            labelControl1.Location = new Point(12, 563);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new Size(922, 14);
            labelControl1.StyleController = layoutControl1;
            labelControl1.TabIndex = 1;
            labelControl1.Text = "• Make sure the filename of the Buyer Inventory file contains its \"effective date\" to ensure proper week number calculations (i.e. L&&R Buyer Inventory <color=red><b>20250901</b></color>.xlsx)";
            // 
            // btnProcess
            // 
            btnProcess.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            btnProcess.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            btnProcess.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnProcess.ImageOptions.SvgImage");
            btnProcess.ImageOptions.SvgImageSize = new Size(32, 32);
            btnProcess.Location = new Point(903, 22);
            btnProcess.Name = "btnProcess";
            btnProcess.Padding = new Padding(10, 2, 10, 2);
            btnProcess.Size = new Size(98, 57);
            btnProcess.StyleController = layoutControl1;
            btnProcess.TabIndex = 2;
            btnProcess.Text = "Import Data";
            btnProcess.Click += btnProcess_Click;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(12, 132);
            txtLog.Name = "txtLog";
            txtLog.Properties.Appearance.FontSizeDelta = 2;
            txtLog.Properties.Appearance.Options.UseFont = true;
            txtLog.Properties.ReadOnly = true;
            txtLog.Size = new Size(989, 405);
            txtLog.StyleController = layoutControl1;
            txtLog.TabIndex = 4;
            // 
            // stepProgressBar1
            // 
            stepProgressBar1.AllowUserInteraction = DevExpress.Utils.DefaultBoolean.False;
            stepProgressBar1.Items.Add(stepBuyerInventory);
            stepProgressBar1.Items.Add(stepBuyerForecast);
            stepProgressBar1.Items.Add(stepVendorForecast);
            stepProgressBar1.Items.Add(stepMainframe);
            stepProgressBar1.Items.Add(stepUpcComp);
            stepProgressBar1.Location = new Point(12, 12);
            stepProgressBar1.Name = "stepProgressBar1";
            stepProgressBar1.Size = new Size(887, 116);
            stepProgressBar1.StyleController = layoutControl1;
            stepProgressBar1.TabIndex = 0;
            // 
            // stepBuyerInventory
            // 
            stepBuyerInventory.ContentBlock1.Appearance.Caption.Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            stepBuyerInventory.ContentBlock1.Appearance.Caption.FontStyleDelta = FontStyle.Bold;
            stepBuyerInventory.ContentBlock1.Appearance.Caption.Options.UseFont = true;
            stepBuyerInventory.ContentBlock1.Caption = "Buyer Inventory";
            stepBuyerInventory.ContentBlock2.Caption = "Last Updated On:";
            stepBuyerInventory.Name = "stepBuyerInventory";
            // 
            // stepBuyerForecast
            // 
            stepBuyerForecast.ContentBlock1.Appearance.Caption.Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            stepBuyerForecast.ContentBlock1.Appearance.Caption.FontStyleDelta = FontStyle.Bold;
            stepBuyerForecast.ContentBlock1.Appearance.Caption.Options.UseFont = true;
            stepBuyerForecast.ContentBlock1.Caption = "Buyer Forecast";
            stepBuyerForecast.ContentBlock2.Caption = "Last Updated On:";
            stepBuyerForecast.Name = "stepBuyerForecast";
            // 
            // stepVendorForecast
            // 
            stepVendorForecast.ContentBlock1.Appearance.Caption.Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            stepVendorForecast.ContentBlock1.Appearance.Caption.FontStyleDelta = FontStyle.Bold;
            stepVendorForecast.ContentBlock1.Appearance.Caption.Options.UseFont = true;
            stepVendorForecast.ContentBlock1.Caption = "Vendor Forecast";
            stepVendorForecast.ContentBlock2.Caption = "Last Updated On:";
            stepVendorForecast.Name = "stepVendorForecast";
            // 
            // stepMainframe
            // 
            stepMainframe.ContentBlock1.Appearance.Caption.Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            stepMainframe.ContentBlock1.Appearance.Caption.FontStyleDelta = FontStyle.Bold;
            stepMainframe.ContentBlock1.Appearance.Caption.Options.UseFont = true;
            stepMainframe.ContentBlock1.Caption = "Mainframe Inventory";
            stepMainframe.ContentBlock2.Caption = "Last Updated On:";
            stepMainframe.Name = "stepMainframe";
            // 
            // stepUpcComp
            // 
            stepUpcComp.ContentBlock1.Appearance.Caption.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            stepUpcComp.ContentBlock1.Appearance.Caption.FontStyleDelta = FontStyle.Bold;
            stepUpcComp.ContentBlock1.Appearance.Caption.Options.UseFont = true;
            stepUpcComp.ContentBlock1.Caption = "Comps Data";
            stepUpcComp.ContentBlock2.Caption = "Last Updated On:";
            stepUpcComp.Name = "stepUpcComp";
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem2, layoutControlItem3, layoutControlItem4, layoutControlItem5, emptySpaceItem1, emptySpaceItem2, layoutControlItem6, layoutControlItem7 });
            Root.Name = "Root";
            Root.Size = new Size(1013, 607);
            Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = stepProgressBar1;
            layoutControlItem1.Location = new Point(0, 0);
            layoutControlItem1.MaxSize = new Size(0, 120);
            layoutControlItem1.MinSize = new Size(54, 120);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(891, 120);
            layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = txtLog;
            layoutControlItem2.Location = new Point(0, 120);
            layoutControlItem2.MinSize = new Size(14, 20);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(993, 409);
            layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.AllowHtmlStringInCaption = true;
            layoutControlItem3.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            layoutControlItem3.Control = btnProcess;
            layoutControlItem3.Location = new Point(891, 10);
            layoutControlItem3.MaxSize = new Size(102, 61);
            layoutControlItem3.MinSize = new Size(102, 61);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(102, 61);
            layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = labelControl1;
            layoutControlItem4.Location = new Point(0, 551);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new Size(993, 18);
            layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = btnRefresh;
            layoutControlItem5.Location = new Point(891, 71);
            layoutControlItem5.MaxSize = new Size(102, 26);
            layoutControlItem5.MinSize = new Size(102, 26);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new Size(102, 26);
            layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.Location = new Point(891, 0);
            emptySpaceItem1.MaxSize = new Size(102, 10);
            emptySpaceItem1.MinSize = new Size(102, 10);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(102, 10);
            emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.Location = new Point(891, 97);
            emptySpaceItem2.MaxSize = new Size(102, 0);
            emptySpaceItem2.MinSize = new Size(102, 10);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(102, 23);
            emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = labelControl2;
            layoutControlItem6.Location = new Point(0, 569);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(993, 18);
            layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            layoutControlItem7.Control = labelControl3;
            layoutControlItem7.Location = new Point(0, 529);
            layoutControlItem7.Name = "layoutControlItem7";
            layoutControlItem7.Size = new Size(993, 22);
            layoutControlItem7.TextVisible = false;
            // 
            // svgImageCollection1
            // 
            svgImageCollection1.Add("actions_checkcircled", "image://svgimages/icon builder/actions_checkcircled.svg");
            svgImageCollection1.Add("singlepageview", "image://svgimages/pdf viewer/singlepageview.svg");
            svgImageCollection1.Add("bo_attention", "image://svgimages/business objects/bo_attention.svg");
            svgImageCollection1.Add("actions_forbid", "image://svgimages/icon builder/actions_forbid.svg");
            svgImageCollection1.Add("notstarted", "image://svgimages/outlook inspired/notstarted.svg");
            svgImageCollection1.Add("bo_statemachine", "image://fluentimages/business objects/bo_statemachine.svg");
            // 
            // barManager1
            // 
            barManager1.DockControls.Add(barDockControlTop);
            barManager1.DockControls.Add(barDockControlBottom);
            barManager1.DockControls.Add(barDockControlLeft);
            barManager1.DockControls.Add(barDockControlRight);
            barManager1.Form = this;
            barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { bbiFetch });
            barManager1.MaxItemId = 1;
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = DockStyle.Top;
            barDockControlTop.Location = new Point(0, 0);
            barDockControlTop.Manager = barManager1;
            barDockControlTop.Size = new Size(1013, 0);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = DockStyle.Bottom;
            barDockControlBottom.Location = new Point(0, 607);
            barDockControlBottom.Manager = barManager1;
            barDockControlBottom.Size = new Size(1013, 0);
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = DockStyle.Left;
            barDockControlLeft.Location = new Point(0, 0);
            barDockControlLeft.Manager = barManager1;
            barDockControlLeft.Size = new Size(0, 607);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = DockStyle.Right;
            barDockControlRight.Location = new Point(1013, 0);
            barDockControlRight.Manager = barManager1;
            barDockControlRight.Size = new Size(0, 607);
            // 
            // bbiFetch
            // 
            bbiFetch.AccessibleName = "miFetchFromMainframe";
            bbiFetch.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            bbiFetch.Caption = "Fetch from Mainframe via FTP";
            bbiFetch.Hint = null;
            bbiFetch.Id = 0;
            bbiFetch.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("bbiFetch.ImageOptions.SvgImage");
            bbiFetch.ImageOptions.SvgImageSize = new Size(16, 16);
            bbiFetch.Name = "bbiFetch";
            bbiFetch.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // popupMenu1
            // 
            popupMenu1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(bbiFetch) });
            popupMenu1.Manager = barManager1;
            popupMenu1.Name = "popupMenu1";
            // 
            // ImportPage
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(layoutControl1);
            Controls.Add(barDockControlLeft);
            Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom);
            Controls.Add(barDockControlTop);
            Name = "ImportPage";
            Size = new Size(1013, 607);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtLog.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)stepProgressBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).EndInit();
            ((System.ComponentModel.ISupportInitialize)svgImageCollection1).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).EndInit();
            ((System.ComponentModel.ISupportInitialize)popupMenu1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.StepProgressBar stepProgressBar1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.StepProgressBarItem stepBuyerInventory;
        private DevExpress.XtraEditors.StepProgressBarItem stepBuyerForecast;
        private DevExpress.XtraEditors.StepProgressBarItem stepVendorForecast;
        private DevExpress.XtraEditors.StepProgressBarItem stepMainframe;
        private DevExpress.XtraEditors.MemoEdit txtLog;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton btnProcess;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.Utils.SvgImageCollection svgImageCollection1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbiFetch;
        private DevExpress.XtraBars.PopupMenu popupMenu1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.StepProgressBarItem stepUpcComp;
    }
}
