using DictionaryEditor.ViewModels;
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
using System.Windows.Shapes;

namespace DictionaryEditor.Views
{
    /// <summary>
    /// Interaction logic for PatternDialog.xaml
    /// </summary>
    public partial class PatternDialog : Window
    {
        private readonly PatternViewModel _viewModel;
        private readonly SpellChecker _spellChecker;

        public PatternDialog(SpellChecker spellChecker, PatternViewModel viewModel)
        {
            InitializeComponent();
            DataContext = _viewModel = viewModel;
            _spellChecker = spellChecker;
        }

        private bool BraceIsProperlyClosed(string text, int startIndex)
        {
            for (int i = startIndex; i < text.Length; i++)
            {
                if (text[i] == '}') return true;
                if (text[i] == '{') return false;
            }

            return false;
        }

        private readonly List<char> _hindiNumbers = new List<char> { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };
        private readonly char[] _arabicNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private void PatternTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.Template)) return;

            bool modified = false;
            var position = patternTextBox.CaretIndex;

            foreach (var change in e.Changes)
            {
                if (change.RemovedLength > 0) continue;

                var text = _viewModel.Template.Substring(change.Offset, change.AddedLength);
                if (text.EndsWith("{") && !BraceIsProperlyClosed(_viewModel.Template, change.Offset + change.AddedLength))
                {
                    _viewModel.Template = _viewModel.Template.Insert(position, "}");
                    modified = true;
                }
            }

            for (int i = 0; i < _viewModel.Template.Length; i++)
            {
                var index = _hindiNumbers.IndexOf(_viewModel.Template[i]);
                if (index != -1)
                {
                    _viewModel.Template = _viewModel.Template.Replace(_hindiNumbers[index], _arabicNumbers[index]);
                    modified = true;
                }
            }

            if (modified)
            {
                patternTextBox.CaretIndex = position;
                return;
            }

            similarsList.ItemsSource =
                _spellChecker.GetPatterns()
                             .Select(p => new
                             {
                                 Distance = Levenshtein.GetDistanceOneRow(_viewModel.Template, p.Template),
                                 Pattern = p
                             })
                             .Where(i => i.Distance < 3)
                             .OrderBy(i => i.Distance)
                             .Select(i => i.Pattern.Template)
                             .ToList();
        }
    }
}
