using Kurdspell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DictionaryEditor.ViewModels
{
    public class PatternViewModel : BindableBase
    {
        private readonly IReadOnlyList<Affix> _affixes;
        private static readonly IReadOnlyList<string> _emptyList = new List<string>();

        public PatternViewModel(Pattern p, IReadOnlyList<Affix> affixes)
        {
            _affixes = affixes;
            Pattern = p;
            _template = p.Template;
            SetParts(p);
        }

        private ObservableCollection<PatternPartViewModel> _parts = new ObservableCollection<PatternPartViewModel>();
        public ReadOnlyObservableCollection<PatternPartViewModel> Parts
        {
            get { return new ReadOnlyObservableCollection<PatternPartViewModel>(_parts); }
        }

        public IEnumerable<string> Variants => IsValid ? Pattern.GetVariants(_affixes) : _emptyList;

        private bool _isValid = true;
        public bool IsValid
        {
            get { return _isValid; }
            private set { SetProperty(ref _isValid, value); }
        }

        private string _template;
        public string Template
        {
            get { return _template; }
            set
            {
                if (SetProperty(ref _template, value))
                {
                    bool isValid = true;

                    try
                    {
                        Pattern = new Pattern(_template);
                        SetParts(Pattern);   
                    }
                    catch (Exception)
                    {
                        isValid = false;
                    }

                    IsValid = isValid;
                    RaisePropertyChanged(nameof(Variants));
                }
            }
        }

        public Pattern Pattern { get; private set; }

        private void SetParts(Pattern pattern)
        {
            _parts.Clear();

            foreach (var part in pattern.Parts)
            {
                if (part is int affix)
                {
                    if (affix > _affixes.Count - 1)
                    {
                        _isValid = false;
                        break;
                    }
                    else
                    {
                        _parts.Add(new PatternPartViewModel($"{{{affix}}}", true, _affixes[affix].Values));
                    }
                }
                else
                {
                    _parts.Add(new PatternPartViewModel(part as string, false, _emptyList));
                }
            }
        }
    }

    public class PatternPartViewModel
    {
        public PatternPartViewModel(string text, bool isAffix, IReadOnlyCollection<string> possibilities)
        {
            Text = text;
            IsAffix = isAffix;
            Possibilities = possibilities;
        }

        public bool IsAffix { get; }
        public string Text { get; }
        public IReadOnlyCollection<string> Possibilities { get; }
        public string Hint => string.Join(",", Possibilities);
    }
}
