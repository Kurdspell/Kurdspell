using Kurdspell;
using System.Collections.ObjectModel;
using System.Linq;

namespace DictionaryEditor.ViewModels
{
    public class DictionaryEditorViewModel : BindableBase
    {
        private readonly SpellChecker _spellChecker;

        public DictionaryEditorViewModel(SpellChecker spellChecker)
        {
            _spellChecker = spellChecker;
            Patterns = new ObservableCollection<PatternViewModel>(
                          _spellChecker.GetPatterns()
                                       .Select(p => new PatternViewModel(p, _spellChecker.GetAffixes()))
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
    }
}
