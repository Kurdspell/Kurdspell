using DictionaryEditor.ViewModels;
using Kurdspell;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DictionaryEditor.Views
{
    /// <summary>
    /// Interaction logic for PatternDialog.xaml
    /// </summary>
    public partial class PatternDialog : Window
    {
        private readonly DictionaryEditorViewModel _parent;
        private readonly int _originalHashCode;

        public PatternDialog(PatternViewModel viewModel, DictionaryEditorViewModel parent)
        {
            _originalHashCode = viewModel.GetHashCode();

            InitializeComponent();
            DataContext = Pattern = viewModel.Clone();
            _parent = parent;
            Loaded += PatternDialog_Loaded;
        }

        private void PatternDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var menu = new ContextMenu();
            foreach (var affix in _parent.Affixes)
            {
                var item = new MenuItem
                {
                    Header = $"[{affix.Name}]: {string.Join(",", affix.Values)}",
                    Tag = affix
                };

                item.Click += InsertAffix_Click;
                menu.Items.Add(item);
            }

            patternTextBox.ContextMenu = menu;
        }

        private void InsertAffix_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            var affix = (Affix)item.Tag;

            var value = $"[{affix.Name}]";
            var caretIndex = patternTextBox.CaretIndex;

            if (string.IsNullOrEmpty(patternTextBox.SelectedText))
            {
                patternTextBox.Text = patternTextBox.Text.Insert(caretIndex, value);
            }
            else
            {
                var start = patternTextBox.SelectionStart;
                patternTextBox.Text = patternTextBox.Text.Remove(patternTextBox.SelectionStart, patternTextBox.SelectionLength);
                patternTextBox.Text = patternTextBox.Text.Insert(start, value);
            }

            patternTextBox.CaretIndex = caretIndex + value.Length;
        }

        private bool BraceIsProperlyClosed(string text, int startIndex)
        {
            for (int i = startIndex; i < text.Length; i++)
            {
                if (text[i] == ']') return true;
                if (text[i] == '[') return false;
            }

            return false;
        }

        public bool? Result { get; private set; }
        public PatternViewModel Pattern { get; }

        private readonly List<char> _hindiNumbers = new List<char> { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };
        private readonly char[] _arabicNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private void PatternTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Pattern.Template)) return;

            bool modified = false;
            var position = patternTextBox.CaretIndex;

            // Automatically close braces
            foreach (var change in e.Changes)
            {
                if (change.RemovedLength > 0) continue;

                var text = Pattern.Template.Substring(change.Offset, change.AddedLength);
                if (text.EndsWith("[") && !BraceIsProperlyClosed(Pattern.Template, change.Offset + change.AddedLength))
                {
                    Pattern.Template = Pattern.Template.Insert(position, "]");
                    modified = true;
                }
            }

            // Convert Hindi Numbers to Arabic numbers
            //for (int i = 0; i < _viewModel.Template.Length; i++)
            //{
            //    var index = _hindiNumbers.IndexOf(_viewModel.Template[i]);
            //    if (index != -1)
            //    {
            //        _viewModel.Template = _viewModel.Template.Replace(_hindiNumbers[index], _arabicNumbers[index]);
            //        modified = true;
            //    }
            //}

            if (modified)
            {
                patternTextBox.CaretIndex = position;
                return;
            }

            similarsList.ItemsSource =
                _parent.Patterns
                             .Where(p => p.GetHashCode() != _originalHashCode)
                             .Select(p => new
                             {
                                 Distance = Levenshtein.GetDistanceOneRow(Pattern.Template, p.Template),
                                 Pattern = p
                             })
                             .Where(i => i.Distance < 3)
                             .OrderBy(i => i.Distance)
                             .Select(i => i.Pattern.Template)
                             .ToList();
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }
    }
}
