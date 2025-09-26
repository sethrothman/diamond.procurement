namespace Diamond.Procurement.Win.Forms
{
    partial class frmLandingPage
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
            if (disposing)
            {
                _updateCheckTimer?.Dispose();
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLandingPage));
            fluentDesignFormContainer1 = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer();
            navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
            navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
            accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
            accordionControlElementUpdateAvailable = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement1 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement3 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement9 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement4 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement6 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement5 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement8 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            accordionControlElement7 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            fluentDesignFormControl1 = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl();
            fluentFormDefaultManager1 = new DevExpress.XtraBars.FluentDesignSystem.FluentFormDefaultManager(components);
            fluentDesignFormContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)navigationFrame1).BeginInit();
            navigationFrame1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)accordionControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)fluentDesignFormControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)fluentFormDefaultManager1).BeginInit();
            SuspendLayout();
            // 
            // fluentDesignFormContainer1
            // 
            fluentDesignFormContainer1.Controls.Add(navigationFrame1);
            fluentDesignFormContainer1.Dock = DockStyle.Fill;
            fluentDesignFormContainer1.Location = new Point(208, 31);
            fluentDesignFormContainer1.Name = "fluentDesignFormContainer1";
            fluentDesignFormContainer1.Size = new Size(1154, 728);
            fluentDesignFormContainer1.TabIndex = 0;
            // 
            // navigationFrame1
            // 
            navigationFrame1.Controls.Add(navigationPage1);
            navigationFrame1.Dock = DockStyle.Fill;
            navigationFrame1.Location = new Point(0, 0);
            navigationFrame1.Name = "navigationFrame1";
            navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] { navigationPage1 });
            navigationFrame1.SelectedPage = navigationPage1;
            navigationFrame1.Size = new Size(1154, 728);
            navigationFrame1.TabIndex = 0;
            navigationFrame1.Text = "navigationFrame1";
            navigationFrame1.TransitionAnimationProperties.FrameInterval = 5000;
            // 
            // navigationPage1
            // 
            navigationPage1.Caption = "navigationPage1";
            navigationPage1.Name = "navigationPage1";
            navigationPage1.Size = new Size(1154, 728);
            // 
            // accordionControl1
            // 
            accordionControl1.AllowItemSelection = true;
            accordionControl1.Appearance.Item.Default.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            accordionControl1.Appearance.Item.Default.Options.UseFont = true;
            accordionControl1.Dock = DockStyle.Left;
            accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] { accordionControlElementUpdateAvailable, accordionControlElement1, accordionControlElement4, accordionControlElement8 });
            accordionControl1.Location = new Point(0, 31);
            accordionControl1.Name = "accordionControl1";
            accordionControl1.OptionsMinimizing.AllowMinimizeMode = DevExpress.Utils.DefaultBoolean.True;
            accordionControl1.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Touch;
            accordionControl1.Size = new Size(208, 728);
            accordionControl1.TabIndex = 1;
            accordionControl1.ViewType = DevExpress.XtraBars.Navigation.AccordionControlViewType.HamburgerMenu;
            // accordionControlElementUpdateAvailable
            //
            accordionControlElementUpdateAvailable.Appearance.Normal.ForeColor = Color.FromArgb(230, 126, 34);
            accordionControlElementUpdateAvailable.Appearance.Normal.Options.UseForeColor = true;
            accordionControlElementUpdateAvailable.ImageOptions.SvgImageSize = new Size(16, 16);
            accordionControlElementUpdateAvailable.Name = "accordionControlElementUpdateAvailable";
            accordionControlElementUpdateAvailable.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            accordionControlElementUpdateAvailable.Tag = "update";
            accordionControlElementUpdateAvailable.Text = "Update available - click to restart";
            accordionControlElementUpdateAvailable.Visible = false;
            //
            // accordionControlElement1
            //
            accordionControlElement1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] { accordionControlElement2, accordionControlElement3, accordionControlElement9 });
            accordionControlElement1.Expanded = true;
            accordionControlElement1.Name = "accordionControlElement1";
            accordionControlElement1.Text = "Main";
            // 
            // accordionControlElement2
            // 
            accordionControlElement2.Expanded = true;
            accordionControlElement2.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("accordionControlElement2.ImageOptions.SvgImage");
            accordionControlElement2.ImageOptions.SvgImageSize = new Size(16, 16);
            accordionControlElement2.Name = "accordionControlElement2";
            accordionControlElement2.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            accordionControlElement2.Tag = "imports";
            accordionControlElement2.Text = "Data Imports";
            // 
            // accordionControlElement3
            // 
            accordionControlElement3.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("accordionControlElement3.ImageOptions.SvgImage");
            accordionControlElement3.ImageOptions.SvgImageSize = new Size(16, 16);
            accordionControlElement3.Name = "accordionControlElement3";
            accordionControlElement3.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            accordionControlElement3.Tag = "analysis";
            accordionControlElement3.Text = "Monthly Orders";
            // 
            // accordionControlElement9
            // 
            accordionControlElement9.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("accordionControlElement9.ImageOptions.SvgImage");
            accordionControlElement9.ImageOptions.SvgImageSize = new Size(16, 16);
            accordionControlElement9.Name = "accordionControlElement9";
            accordionControlElement9.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            accordionControlElement9.Tag = "shipments";
            accordionControlElement9.Text = "Shipments";
            // 
            // accordionControlElement4
            // 
            accordionControlElement4.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] { accordionControlElement6, accordionControlElement5 });
            accordionControlElement4.Expanded = true;
            accordionControlElement4.Name = "accordionControlElement4";
            accordionControlElement4.Text = "Maintenance";
            // 
            // accordionControlElement6
            // 
            accordionControlElement6.Name = "accordionControlElement6";
            accordionControlElement6.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            accordionControlElement6.Tag = "masterlist";
            accordionControlElement6.Text = "Master List";
            // 
            // accordionControlElement5
            // 
            accordionControlElement5.Name = "accordionControlElement5";
            accordionControlElement5.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            accordionControlElement5.Text = "UPC";
            accordionControlElement5.Visible = false;
            // 
            // accordionControlElement8
            // 
            accordionControlElement8.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] { accordionControlElement7 });
            accordionControlElement8.Expanded = true;
            accordionControlElement8.Name = "accordionControlElement8";
            accordionControlElement8.Text = "Reports";
            accordionControlElement8.Visible = false;
            // 
            // accordionControlElement7
            // 
            accordionControlElement7.Name = "accordionControlElement7";
            accordionControlElement7.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            accordionControlElement7.Tag = "notsellingyet";
            accordionControlElement7.Text = "Buyer Wants - Not on Master List";
            // 
            // fluentDesignFormControl1
            // 
            fluentDesignFormControl1.FluentDesignForm = this;
            fluentDesignFormControl1.Location = new Point(0, 0);
            fluentDesignFormControl1.Manager = fluentFormDefaultManager1;
            fluentDesignFormControl1.Name = "fluentDesignFormControl1";
            fluentDesignFormControl1.Size = new Size(1362, 31);
            fluentDesignFormControl1.TabIndex = 2;
            fluentDesignFormControl1.TabStop = false;
            // 
            // fluentFormDefaultManager1
            // 
            fluentFormDefaultManager1.Form = this;
            // 
            // frmLandingPage
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1362, 759);
            ControlContainer = fluentDesignFormContainer1;
            Controls.Add(fluentDesignFormContainer1);
            Controls.Add(accordionControl1);
            Controls.Add(fluentDesignFormControl1);
            FluentDesignFormControl = fluentDesignFormControl1;
            IconOptions.Image = (Image)resources.GetObject("frmLandingPage.IconOptions.Image");
            Name = "frmLandingPage";
            NavigationControl = accordionControl1;
            StartPosition = FormStartPosition.Manual;
            Text = "Diamond Wholesale";
            fluentDesignFormContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)navigationFrame1).EndInit();
            navigationFrame1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)accordionControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)fluentDesignFormControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)fluentFormDefaultManager1).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer fluentDesignFormContainer1;
        private DevExpress.XtraBars.Navigation.AccordionControl accordionControl1;
        private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl fluentDesignFormControl1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement1;
        private DevExpress.XtraBars.FluentDesignSystem.FluentFormDefaultManager fluentFormDefaultManager1;
        private DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame1;
        private DevExpress.XtraBars.Navigation.NavigationPage navigationPage1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement2;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement3;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement4;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement5;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement6;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement7;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElementUpdateAvailable;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement8;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement9;
    }
}