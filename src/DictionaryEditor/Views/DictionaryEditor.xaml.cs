using DictionaryEditor.ViewModels;
using Kurdspell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DictionaryEditor.Views
{
    /// <summary>
    /// Interaction logic for DictionaryEditor.xaml
    /// </summary>
    public partial class DictionaryEditor : UserControl
    {
        private readonly DictionaryEditorViewModel _viewModel;

        public DictionaryEditor(SpellChecker spellChecker)
        {
            InitializeComponent();
            DataContext = _viewModel = new DictionaryEditorViewModel(spellChecker);
            FilterByTextBox(patternsList, filterPatternsList);
        }

        private void FilterPatternsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(patternsList.ItemsSource)?.Refresh();
        }

        private void FilterVariantsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(variantsList.ItemsSource)?.Refresh();
        }

        private void FilterByTextBox(ListView list, TextBox textBox)
        {
            var collectionView = CollectionViewSource.GetDefaultView(list.ItemsSource);
            if (collectionView == null) return;

            collectionView.Filter = o =>
            {
                string template = o is PatternViewModel ? ((PatternViewModel)o).Template : o as string;
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    return true;
                }
                else
                {
                    return template.IndexOf(textBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            };
        }

        private void PatternsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pattern = patternsList.SelectedItem as PatternViewModel;
            if (pattern == null) return;

            var dialog = new PatternDialog(pattern, _viewModel);
            dialog.ShowDialog();
        }

        private void PatternPart_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var button = sender as FrameworkElement;
            if (button.Tag is PatternPartViewModel part && part.IsAffix)
            {
                e.Handled = true;

                var affix = _viewModel.Affixes.First(a => a.Name == part.Text);
                var dialog = new AffixDialog(affix, _viewModel);
                dialog.ShowDialog();
                if (dialog.DialogResult == true)
                {
                    var newAffix = dialog.GetAffix();
                    if (newAffix != affix)
                    {
                        _viewModel.ReplaceAffix(affix, newAffix);
                    }
                }
            }
        }
    }
}
