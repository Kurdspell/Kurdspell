using DictionaryEditor.ViewModels;
using Kurdspell;
using System.Linq;
using System.Windows;

namespace DictionaryEditor.Views
{
    /// <summary>
    /// Interaction logic for AffixDialog.xaml
    /// </summary>
    public partial class AffixDialog : Window
    {
        private readonly AffixDialogViewModel _viewModel;
        private readonly DictionaryEditorViewModel _parent;
        private readonly string _oldName;

        public AffixDialog(Affix affix, DictionaryEditorViewModel parent)
        {
            InitializeComponent();
            DataContext = _viewModel = new AffixDialogViewModel(affix, parent);
            Closing += AffixDialog_Closing;
            _parent = parent;
            _oldName = affix.Name;
        }

        private void AffixDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == true &&
                _oldName != _viewModel.Name &&
                _parent.Affixes.Count(a => a.Name == _viewModel.Name) > 0)
            {
                MessageBox.Show("This name already exists!");
                e.Cancel = true;
            }
        }

        public Affix GetAffix() => 
            new Affix(_viewModel.Name, _viewModel.Possibilities.Select(p => p.Value).ToArray());

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void AddPosibilityButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Possibilities.Add(new AffixDialogViewModel.PossibilityViewModel());
        }

        private void RemovePosibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPossibility == null) return;
            _viewModel.Possibilities.Remove(_viewModel.SelectedPossibility);
        }
    }
}
