using Kurdspell;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DictionaryEditor.ViewModels
{
    public class DictionaryEditorViewModel : BindableBase
    {
        private readonly Dictionary<string, string> _properties;
        private readonly Dictionary<string, Affix> _affixDictionary;
        public DictionaryEditorViewModel(SpellChecker spellChecker)
        {
            _properties = spellChecker.Properties;
            _affixDictionary = spellChecker.GetAffixes().ToDictionary(a => a.Key, a => a.Value);

            Patterns = new ObservableCollection<PatternViewModel>(
                          spellChecker.GetPatterns()
                                       .Select(p => new PatternViewModel(p, _affixDictionary))
                          );
            Affixes = new ObservableCollection<Affix>(spellChecker.GetAffixes().Values);

            RemovePatternCommand = new DelegateCommand(p =>
            {
                var pattern = p as PatternViewModel;
                if (pattern is null) return;

                Patterns.Remove(pattern);
            });
        }

        public ICommand RemovePatternCommand { get; }

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

        private ObservableCollection<Affix> _affixes;
        public ObservableCollection<Affix> Affixes
        {
            get { return _affixes; }
            set { SetProperty(ref _affixes, value); }
        }

        public void ReplaceAffix(Affix current, Affix changed)
        {
            var index = Affixes.IndexOf(current);
            Affixes[index] = changed;

            _affixDictionary.Remove(current.Name);
            _affixDictionary[changed.Name] = changed;

            if (current.Name == changed.Name)
                return;

            foreach (var pattern in Patterns)
            {
                for (int i = 0; i < pattern.Parts.Count; i++)
                {
                    if (!pattern.Pattern.IsPartAnAffixFlags[i])
                        continue;

                    if (pattern.Parts[i].Text == current.Name)
                    {
                        pattern.Template = pattern.Template.Replace("[" + current.Name + "]", "[" + changed.Name + "]");
                        break;
                    }
                }
            }
        }

        public List<Pattern> GetPatterns() => Patterns.Select(p => p.Pattern).ToList();
        public List<Affix> GetAffixes() => Affixes.ToList();
        public Dictionary<string, string> GetProperties() => _properties;

        public PatternViewModel CreatePattern()
        {
            return new PatternViewModel(new Pattern(), _affixDictionary);
        }
    }
}
