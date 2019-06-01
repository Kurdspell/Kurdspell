using System.Text;
using System.Windows.Forms;
using KurdspellForWord.Helpers;
using Microsoft.Office.Tools.Ribbon;

namespace KurdspellForWord
{
    public partial class Ribbon
    {
        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            btnCheckSpelling.Click += BtnCheckSpelling_Click;
        }

        private void BtnCheckSpelling_Click(object sender, RibbonControlEventArgs e)
        {
            TheGodFather.Please.ShowTaskPane();
            TheGodFather.Please.GiveMeTheMistakesListViewModel()?.FindMistakesCommand?.Execute(null);
        }
    }
}
