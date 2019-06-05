using KurdspellForWord.Glue;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace KurdspellForWord
{
    public partial class ThisAddIn
    {
        private KeyboardHook _hook;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            _hook = new KeyboardHook();

            var keyboard = Observable.FromEventPattern<KeyboardHookEventArgs>(
                h => _hook.KeyPressed += h,
                h => _hook.KeyPressed -= h
               ).Buffer(TimeSpan.FromMilliseconds(1000))
                .Where(l => l.Any())
                .Select(list => new
                {
                    Start = list.MinBy(i => i.EventArgs.Position).EventArgs,
                    End = list.MaxBy(i => i.EventArgs.Position).EventArgs,
                })
                .Where(item => item.Start != item.End);

            keyboard.Subscribe(range =>
            {
                var actualEnd = Application.ActiveDocument.Range().End;

                var start = range.Start.Position;
                var end = Math.Min(range.End.Position, actualEnd);

                var text = Application.ActiveDocument.Range(start, end).Text;

                Debug.WriteLine($"{start} => {end} ({text})");
            });
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