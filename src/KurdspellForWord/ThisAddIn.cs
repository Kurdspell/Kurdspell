using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.IO;
using KurdspellForWord.Helpers;

namespace KurdspellForWord
{
    public partial class ThisAddIn
    {
        private async void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            using (var stream = File.Open("ckb-IQ.txt", FileMode.Open))
            {
                await TheGodFather.Please.LoadSpellCheckerAsync(stream);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
