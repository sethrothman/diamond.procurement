namespace Diamond.Procurement.Win.Forms
{
    partial class frmAddShipmentWithItemsDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddShipmentWithItemsDialog));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            textEdit1 = new DevExpress.XtraEditors.TextEdit();
            btnSave = new DevExpress.XtraEditors.SimpleButton();
            gridItems = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            dateEta = new DevExpress.XtraEditors.DateEdit();
            dateShipment = new DevExpress.XtraEditors.DateEdit();
            spinTruckNum = new DevExpress.XtraEditors.SpinEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridItems).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateEta.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateEta.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateShipment.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateShipment.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)spinTruckNum.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(textEdit1);
            layoutControl1.Controls.Add(btnSave);
            layoutControl1.Controls.Add(gridItems);
            layoutControl1.Controls.Add(dateEta);
            layoutControl1.Controls.Add(dateShipment);
            layoutControl1.Controls.Add(spinTruckNum);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(981, 597);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // textEdit1
            // 
            textEdit1.Location = new Point(179, 45);
            textEdit1.Name = "textEdit1";
            textEdit1.Size = new Size(105, 20);
            textEdit1.StyleController = layoutControl1;
            textEdit1.TabIndex = 6;
            // 
            // btnSave
            // 
            btnSave.AutoWidthInLayoutControl = true;
            btnSave.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnSave.ImageOptions.SvgImage");
            btnSave.ImageOptions.SvgImageSize = new Size(16, 16);
            btnSave.Location = new Point(907, 559);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6, 2, 6, 2);
            btnSave.Size = new Size(62, 26);
            btnSave.StyleController = layoutControl1;
            btnSave.TabIndex = 5;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            // 
            // gridItems
            // 
            gridItems.Location = new Point(12, 81);
            gridItems.MainView = gridView1;
            gridItems.Name = "gridItems";
            gridItems.Size = new Size(957, 474);
            gridItems.TabIndex = 4;
            gridItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.GridControl = gridItems;
            gridView1.Name = "gridView1";
            gridView1.NewItemRowText = "Drag 'n Drop Excel File or Copy and Paste UPC & Quantity from Excel...";
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            // 
            // dateEta
            // 
            dateEta.EditValue = null;
            dateEta.Location = new Point(537, 45);
            dateEta.Name = "dateEta";
            dateEta.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateEta.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateEta.Size = new Size(144, 20);
            dateEta.StyleController = layoutControl1;
            dateEta.TabIndex = 3;
            // 
            // dateShipment
            // 
            dateShipment.EditValue = null;
            dateShipment.Location = new Point(371, 45);
            dateShipment.Name = "dateShipment";
            dateShipment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateShipment.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateShipment.Size = new Size(130, 20);
            dateShipment.StyleController = layoutControl1;
            dateShipment.TabIndex = 2;
            // 
            // spinTruckNum
            // 
            spinTruckNum.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
            spinTruckNum.Location = new Point(66, 45);
            spinTruckNum.Name = "spinTruckNum";
            spinTruckNum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            spinTruckNum.Properties.IsFloatValue = false;
            spinTruckNum.Properties.MaskSettings.Set("mask", "N00");
            spinTruckNum.Size = new Size(50, 20);
            spinTruckNum.StyleController = layoutControl1;
            spinTruckNum.TabIndex = 0;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { emptySpaceItem1, layoutControlItem4, layoutControlItem5, layoutControlGroup1 });
            Root.Name = "Root";
            Root.Size = new Size(981, 597);
            Root.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.Location = new Point(0, 547);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(895, 30);
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = gridItems;
            layoutControlItem4.Location = new Point(0, 69);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new Size(961, 478);
            layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = btnSave;
            layoutControlItem5.Location = new Point(895, 547);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new Size(66, 30);
            layoutControlItem5.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem2, layoutControlItem3, emptySpaceItem2, layoutControlItem6 });
            layoutControlGroup1.Location = new Point(0, 0);
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(961, 69);
            layoutControlGroup1.Text = "Shipment Details";
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            layoutControlItem1.Control = spinTruckNum;
            layoutControlItem1.Location = new Point(0, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(96, 24);
            layoutControlItem1.Text = "Truck #";
            layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            layoutControlItem1.TextSize = new Size(37, 13);
            layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            layoutControlItem2.Control = dateShipment;
            layoutControlItem2.Location = new Point(264, 0);
            layoutControlItem2.MaxSize = new Size(217, 24);
            layoutControlItem2.MinSize = new Size(217, 24);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
            layoutControlItem2.Size = new Size(217, 24);
            layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem2.Text = "Shipment Date";
            layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            layoutControlItem2.TextSize = new Size(70, 13);
            layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            layoutControlItem3.Control = dateEta;
            layoutControlItem3.Location = new Point(481, 0);
            layoutControlItem3.MaxSize = new Size(180, 24);
            layoutControlItem3.MinSize = new Size(180, 24);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
            layoutControlItem3.Size = new Size(180, 24);
            layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem3.Text = "ETA";
            layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            layoutControlItem3.TextSize = new Size(19, 13);
            layoutControlItem3.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.Location = new Point(661, 0);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(276, 24);
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = textEdit1;
            layoutControlItem6.Location = new Point(96, 0);
            layoutControlItem6.MaxSize = new Size(168, 24);
            layoutControlItem6.MinSize = new Size(168, 24);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
            layoutControlItem6.Size = new Size(168, 24);
            layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem6.Text = "Invoice #";
            layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            layoutControlItem6.TextSize = new Size(46, 13);
            layoutControlItem6.TextToControlDistance = 5;
            // 
            // frmAddShipmentWithItemsDialog
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(981, 597);
            Controls.Add(layoutControl1);
            IconOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("frmAddShipmentWithItemsDialog.IconOptions.SvgImage");
            Name = "frmAddShipmentWithItemsDialog";
            Text = "Add New Shipment";
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridItems).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateEta.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateEta.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateShipment.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateShipment.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)spinTruckNum.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraGrid.GridControl gridItems;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.DateEdit dateEta;
        private DevExpress.XtraEditors.DateEdit dateShipment;
        private DevExpress.XtraEditors.SpinEdit spinTruckNum;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
    }
}