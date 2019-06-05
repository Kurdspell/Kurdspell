using KurdspellForWord.Glue;
using System;
using System.Diagnostics;

namespace KurdspellForWord
{
    public partial class ThisAddIn
    {
        private KeyboardHook _hook;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            _hook = new KeyboardHook();
            _hook.KeyPressed += Hook_KeyPressed;
        }

        private void Hook_KeyPressed(object sender, KeyboardHookEventArgs e)
        {
            Debug.WriteLine(e.Key);
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            _hook.Dispose();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion
    }
}