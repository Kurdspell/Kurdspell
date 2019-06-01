namespace KurdspellForWord
{
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.kurdspell = this.Factory.CreateRibbonTab();
            this.spellchecking = this.Factory.CreateRibbonGroup();
            this.btnCheckSpelling = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.kurdspell.SuspendLayout();
            this.spellchecking.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group1);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // group1
            // 
            this.group1.Label = "group1";
            this.group1.Name = "group1";
            // 
            // kurdspell
            // 
            this.kurdspell.Groups.Add(this.spellchecking);
            this.kurdspell.Label = "Kurdspell";
            this.kurdspell.Name = "kurdspell";
            this.kurdspell.Tag = "kurdspell";
            // 
            // spellchecking
            // 
            this.spellchecking.Items.Add(this.btnCheckSpelling);
            this.spellchecking.Label = "Spellchecking";
            this.spellchecking.Name = "spellchecking";
            // 
            // btnCheckSpelling
            // 
            this.btnCheckSpelling.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnCheckSpelling.Image = global::KurdspellForWord.Properties.Resources.success;
            this.btnCheckSpelling.Label = "Check Spelling";
            this.btnCheckSpelling.Name = "btnCheckSpelling";
            this.btnCheckSpelling.ShowImage = true;
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.Tabs.Add(this.kurdspell);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.kurdspell.ResumeLayout(false);
            this.kurdspell.PerformLayout();
            this.spellchecking.ResumeLayout(false);
            this.spellchecking.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        private Microsoft.Office.Tools.Ribbon.RibbonTab kurdspell;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup spellchecking;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCheckSpelling;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon KurdspellRibbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
