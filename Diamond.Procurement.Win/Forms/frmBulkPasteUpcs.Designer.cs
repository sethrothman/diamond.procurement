namespace Diamond.Procurement.Win.Forms
{
    partial class frmBulkPasteUpcs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBulkPasteUpcs));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnOk = new DevExpress.XtraEditors.SimpleButton();
            txtRemove = new DevExpress.XtraEditors.MemoEdit();
            txtAdd = new DevExpress.XtraEditors.MemoEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItemAdd = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItemRemove = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtRemove.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtAdd.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemAdd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemRemove).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(btnCancel);
            layoutControl1.Controls.Add(btnOk);
            layoutControl1.Controls.Add(txtRemove);
            layoutControl1.Controls.Add(txtAdd);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(403, 368);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // btnCancel
            // 
            btnCancel.AutoWidthInLayoutControl = true;
            btnCancel.Location = new Point(340, 334);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6, 0, 6, 0);
            btnCancel.Size = new Size(51, 22);
            btnCancel.StyleController = layoutControl1;
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            // 
            // btnOk
            // 
            btnOk.AutoWidthInLayoutControl = true;
            btnOk.Location = new Point(293, 334);
            btnOk.Name = "btnOk";
            btnOk.Padding = new Padding(6, 0, 6, 0);
            btnOk.Size = new Size(43, 22);
            btnOk.StyleController = layoutControl1;
            btnOk.TabIndex = 3;
            btnOk.Text = "Save";
            // 
            // txtRemove
            // 
            txtRemove.Location = new Point(142, 173);
            txtRemove.Name = "txtRemove";
            txtRemove.Size = new Size(249, 157);
            txtRemove.StyleController = layoutControl1;
            txtRemove.TabIndex = 2;
            // 
            // txtAdd
            // 
            txtAdd.Location = new Point(142, 12);
            txtAdd.Name = "txtAdd";
            txtAdd.Size = new Size(249, 157);
            txtAdd.StyleController = layoutControl1;
            txtAdd.TabIndex = 0;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItemAdd, layoutControlItemRemove, layoutControlItem1, layoutControlItem2, emptySpaceItem1 });
            Root.Name = "Root";
            Root.Size = new Size(403, 368);
            Root.TextVisible = false;
            // 
            // layoutControlItemAdd
            // 
            layoutControlItemAdd.AllowHtmlStringInCaption = true;
            layoutControlItemAdd.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItemAdd.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            layoutControlItemAdd.Control = txtAdd;
            layoutControlItemAdd.Location = new Point(0, 0);
            layoutControlItemAdd.Name = "layoutControlItemAdd";
            layoutControlItemAdd.Size = new Size(383, 161);
            layoutControlItemAdd.Text = "Paste UPCs to <b>Add</b>";
            layoutControlItemAdd.TextSize = new Size(118, 13);
            // 
            // layoutControlItemRemove
            // 
            layoutControlItemRemove.AllowHtmlStringInCaption = true;
            layoutControlItemRemove.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItemRemove.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            layoutControlItemRemove.Control = txtRemove;
            layoutControlItemRemove.Location = new Point(0, 161);
            layoutControlItemRemove.Name = "layoutControlItemRemove";
            layoutControlItemRemove.Size = new Size(383, 161);
            layoutControlItemRemove.Text = "Paste UPCs to <b>Remove</b>";
            layoutControlItemRemove.TextSize = new Size(118, 13);
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = btnOk;
            layoutControlItem1.Location = new Point(281, 322);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(47, 26);
            layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = btnCancel;
            layoutControlItem2.Location = new Point(328, 322);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(55, 26);
            layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.Location = new Point(0, 322);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(281, 26);
            // 
            // frmBulkPasteUpcs
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(403, 368);
            Controls.Add(layoutControl1);
            IconOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("frmBulkPasteUpcs.IconOptions.SvgImage");
            Name = "frmBulkPasteUpcs";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Bulk Add/Remove UPCs to Master List";
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtRemove.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtAdd.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemAdd).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemRemove).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.MemoEdit txtRemove;
        private DevExpress.XtraEditors.MemoEdit txtAdd;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemAdd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemRemove;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}