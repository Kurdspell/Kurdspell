using Kurdspell;
using System.Collections.ObjectModel;
using System.Linq;

namespace DictionaryEditor.ViewModels
{
    public class AffixDialogViewModel : BindableBase
    {
        private readonly Affix _affix;
        private readonly DictionaryEditorViewModel _parent;

        public AffixDialogViewModel(Affix affix, DictionaryEditorViewModel parent)
        {
            _affix = affix;
            _parent = parent;
            var vms = affix.Values.Select(a => new PossibilityViewModel { Value = a });
            Possibilities = new ObservableCollection<PossibilityViewModel>(vms);
            Name = affix.Name;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private ObservableCollection<PossibilityViewModel> _possibilities;
        public ObservableCollection<PossibilityViewModel> Possibilities
        {
            get { return _possibilities; }
            set { SetProperty(ref _possibilities, value); }
        }

        public class PossibilityViewModel : BindableBase
        {
            private string _value;
            public string Value
            {
                get { return _value; }
                set { SetProperty(ref _value, value); }
            }
        }
    }
}
