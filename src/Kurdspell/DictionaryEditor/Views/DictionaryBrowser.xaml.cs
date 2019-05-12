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
        }

        private void PatternsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (patternsList.SelectedItem == null) return;
            var selectedItem = (Pattern)patternsList.SelectedItem;
            variantsList.ItemsSource = selectedItem.GetVariants(_spellChecker.GetRules());
        }
    }
}
