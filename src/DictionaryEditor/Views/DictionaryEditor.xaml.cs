using DictionaryEditor.ViewModels;
using Kurdspell;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
            Loaded += DictionaryEditor_Loaded;
            KeyUp += DictionaryEditor_KeyUp;
        }

        private void DictionaryEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.RemovePatternCommand.Execute(patternsList.SelectedItem);
            }
        }

        private void DictionaryEditor_Loaded(object sender, RoutedEventArgs e)
        {
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
            if (dialog.Result == true)
            {
                var index = _viewModel.Patterns.IndexOf(pattern);
                _viewModel.Patterns[index] = dialog.Pattern;
            }
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
                    _viewModel.ReplaceAffix(affix, newAffix);
                }
            }
        }

        private void AddPatternButton_Click(object sender, RoutedEventArgs e)
        {
            var pattern = _viewModel.CreatePattern();

            var dialog = new PatternDialog(pattern, _viewModel);
            dialog.ShowDialog();
            if (dialog.Result == true)
            {
                _viewModel.Patterns.Add(dialog.Pattern);
            }
        }
    }
}
