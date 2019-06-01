using Kurdspell;
using KurdspellForWord.Glue;
using KurdspellForWord.Services;
using KurdspellForWord.ViewModels;
using KurdspellForWord.Views;
using Microsoft.Office.Tools;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KurdspellForWord.Helpers
{
    public class TheGodFather
    {
        private TheGodFather()
        {

        }

        private static readonly Lazy<TheGodFather> _lazy
            = new Lazy<TheGodFather>(() => new TheGodFather());
        private SpellChecker _spellChecker;
        private CustomTaskPane _taskPane;
        private MistakesListViewModel _mistakesListVM;

        public static TheGodFather Please => _lazy.Value;

        private ThisAddIn _addIn;

        public async Task LoadSpellCheckerAsync(Stream stream)
        {
            _spellChecker = await SpellChecker.FromStreamAsync(stream);
        }

        public SpellChecker GiveMeTheSpellChecker() => _spellChecker;

        public MistakesListViewModel GiveMeTheMistakesListViewModel() => _mistakesListVM;

        public CustomTaskPane ShowTaskPane()
        {
            if (!Globals.ThisAddIn.CustomTaskPanes.Any())
            {
                var service = new DocumentSerivce(Globals.ThisAddIn.Application.ActiveDocument);
                _mistakesListVM = new MistakesListViewModel(service, _spellChecker);
                var control = new WpfHost();
                control.Host.Child = new MistakesList(_mistakesListVM);
                _taskPane = Globals.ThisAddIn.CustomTaskPanes.Add(control, "Kurdspell");
                _taskPane.Visible = true;
            }
            
            return _taskPane;
        }
    }
}
