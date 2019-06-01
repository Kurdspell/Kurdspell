using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace KurdspellForWord.Glue
{
    public partial class WpfHost : UserControl
    {
        private ElementHost _elementHost1;

        public WpfHost()
        {
            InitializeComponent();
        }

        public ElementHost Host => _elementHost1;

        private void InitializeComponent()
        {
            this.Font = new System.Drawing.Font(
                        "Segoe UI",
                        12,
                        System.Drawing.FontStyle.Regular,
                        System.Drawing.GraphicsUnit.Point,
                        ((byte)(0)));
            this._elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this._elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._elementHost1.Location = new System.Drawing.Point(0, 0);
            this._elementHost1.Name = "elementHost1";
            this._elementHost1.Size = new System.Drawing.Size(150, 150);
            this._elementHost1.TabIndex = 0;
            this._elementHost1.Text = "elementHost1";
            this._elementHost1.Child = null;
            // 
            // TaskPaneWpfControlHost
            // 
            this.Controls.Add(this._elementHost1);
            this.Name = "TaskPaneWpfControlHost";
            this.ResumeLayout(false);

        }
    }
}
