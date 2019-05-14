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
        private Pattern _pattern;

        public PatternViewModel(Pattern p, IReadOnlyList<Affix> affixes)
        {
            _affixes = affixes;
            _pattern = p;
            _template = p.Template;
        }

        private ObservableCollection<PatternPartViewModel> _parts = new ObservableCollection<PatternPartViewModel>();
        public ReadOnlyObservableCollection<PatternPartViewModel> Parts
        {
            get { return new ReadOnlyObservableCollection<PatternPartViewModel>(_parts); }
        }

        public IEnumerable<string> Variants => IsValid ? _pattern.GetVariants(_affixes) : _emptyList;

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
                        _pattern = new Pattern(_template);
                        _parts.Clear();

                        foreach (var part in _pattern.Parts)
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
                                    _parts.Add(new PatternPartViewModel(true, _affixes[affix].Values));
                                }
                            }
                            else
                            {
                                _parts.Add(new PatternPartViewModel(false, _emptyList));
                            }
                        }
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
    }

    public class PatternPartViewModel
    {
        public PatternPartViewModel(bool isAffix, IReadOnlyCollection<string> possibilities)
        {
            IsAffix = isAffix;
            Possibilities = possibilities;
        }

        public bool IsAffix { get; }
        public IReadOnlyCollection<string> Possibilities { get; }
    }
}
