using Kurdspell;
using System.Collections.ObjectModel;
using System.Linq;

namespace DictionaryEditor.ViewModels
{
    public class DictionaryEditorViewModel : BindableBase
    {
        public DictionaryEditorViewModel(SpellChecker spellChecker)
        {
            SpellChecker = spellChecker;
            Patterns = new ObservableCollection<PatternViewModel>(
                          SpellChecker.GetPatterns()
                                       .Select(p => new PatternViewModel(p, SpellChecker.GetAffixes()))
                          );
        }

        private ObservableCollection<PatternViewModel> _patterns;
        public ObservableCollection<PatternViewModel> Patterns
        {
            get { return _patterns; }
            set { SetProperty(ref _patterns, value); }
        }

        private PatternViewModel _selectedPattern;
        public PatternViewModel SelectedPattern
        {
            get { return _selectedPattern; }
            set { SetProperty(ref _selectedPattern, value); }
        }

        public SpellChecker SpellChecker { get; }
    }
}
