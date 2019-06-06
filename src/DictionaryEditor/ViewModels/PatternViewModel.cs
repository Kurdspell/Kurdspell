using Kurdspell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DictionaryEditor.ViewModels
{
    public class PatternViewModel : BindableBase
    {
        private readonly IReadOnlyDictionary<string, Affix> _affixes;
        private static readonly IReadOnlyList<string> _emptyList = new List<string>();

        public PatternViewModel(
            Pattern p,
            IReadOnlyDictionary<string, Affix> affixes)
        {
            _affixes = affixes;
            Pattern = p;
            _template = p.Template;
            SetParts(p);
        }

        internal PatternViewModel Clone()
        {
            return new PatternViewModel(Pattern, _affixes);
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
                    IsDirty = true;
                }
            }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { SetProperty(ref _isDirty, value); }
        }

        public Pattern Pattern { get; private set; }

        private void SetParts(Pattern pattern)
        {
            _parts.Clear();

            var count = pattern.Parts?.Count ?? 0;
            for (int i = 0; i < count; i++)
            {
                var part = pattern.Parts[i];
                if (pattern.IsPartAnAffixFlags[i])
                {
                    if (!_affixes.ContainsKey(part))
                    {
                        _isValid = false;
                        break;
                    }
                    else
                    {
                        _parts.Add(new PatternPartViewModel(part, true, _affixes[part].Values));
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

        public string Text { get; }
        public bool IsAffix { get; }

        public IReadOnlyCollection<string> Possibilities { get; }
        public string Hint => string.Join(",", Possibilities);
    }
}
