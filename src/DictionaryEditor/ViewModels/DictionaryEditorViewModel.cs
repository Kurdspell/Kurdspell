using Kurdspell;
using System;
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
                          spellChecker.GetPatterns()
                                       .Select(p => new PatternViewModel(p, spellChecker.GetAffixes(), spellChecker))
                          );
            Affixes = new ObservableCollection<Affix>(spellChecker.GetAffixes().Values);
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

        private ObservableCollection<Affix> _affixes;
        public ObservableCollection<Affix> Affixes
        {
            get { return _affixes; }
            set { SetProperty(ref _affixes, value); }
        }

        public void ReplaceAffix(Affix current, Affix changed)
        {;
            var index = Affixes.IndexOf(current);
            Affixes.Remove(current);
            Affixes.Insert(index, changed);
            _spellChecker.RemoveAffix(current);
            _spellChecker.AddAffix(changed);

            foreach (var pattern in Patterns)
            {
                for (int i = 0; i < pattern.Parts.Count; i++)
                {
                    if (!pattern.Pattern.IsPartAnAffixFlags[i])
                        continue;

                    if (pattern.Parts[i].Text == current.Name)
                    {
                        pattern.Template = pattern.Template.Replace("{" + current.Name + "}", "{" + changed.Name + "}");
                        break;
                    }
                }
            }
        }
    }
}
