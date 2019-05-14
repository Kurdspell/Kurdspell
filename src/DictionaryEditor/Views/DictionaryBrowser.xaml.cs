using Kurdspell;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for DictionaryBrowser.xaml
    /// </summary>
    public partial class DictionaryBrowser : UserControl
    {
        private readonly SpellChecker _spellChecker;

        public DictionaryBrowser(SpellChecker spellChecker)
        {
            InitializeComponent();
            _spellChecker = spellChecker;
            patternsList.ItemsSource = _spellChecker.GetPatterns();
            FilterByTextBox(patternsList, filterPatternsList);
        }

        private void PatternsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (patternsList.SelectedItem == null) return;
            var selectedItem = (Pattern)patternsList.SelectedItem;
            variantsList.ItemsSource = selectedItem.GetVariants(_spellChecker.GetRules());
            FilterByTextBox(variantsList, filterVariantsList);
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
                string template = o is Pattern ? ((Pattern)o).Template : o as string;
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
    }
}
