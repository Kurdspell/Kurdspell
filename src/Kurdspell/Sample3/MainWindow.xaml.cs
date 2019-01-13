using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using Kurdspell;

namespace Sample3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _text = string.Empty;
        private readonly List<string> _misspelledWords = new List<string>();
        private readonly SpellChecker _spellChecker = new SpellChecker("");
        private readonly TextDecoration _squigglyUnderline;

        public MainWindow()
        {
            InitializeComponent();

            var pen = FindResource("SquigglyPen") as Pen;

            _squigglyUnderline = new TextDecoration(TextDecorationLocation.Underline, pen, 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended);
        }

        private void Rich_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Back || e.Key == Key.Delete)
            {
                var range = new TextRange(rich.Document.ContentStart, rich.Document.ContentEnd);
                _text = range.Text;
            }
        }

        private void FindAndReplace(string misspelledWord, string correctWord)
        {
            var range = new TextRange(rich.Document.ContentStart, rich.Document.ContentEnd);
            var current = range.Start.GetInsertionPosition(LogicalDirection.Forward);

            while (current != null)
            {
                var text = current.GetTextInRun(LogicalDirection.Forward);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var index = text.IndexOf(misspelledWord);
                    if (index != -1)
                    {
                        var start = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                        var end = start.GetPositionAtOffset(misspelledWord.Length, LogicalDirection.Forward);
                        var selection = new TextRange(start, end);

                        selection.Text = correctWord;
                        selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        rich.Selection.Select(selection.Start, selection.End);
                        rich.Focus();
                    }
                }

                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        private void SpellCheck(string content)
        {
            _misspelledWords.Clear();

            var words = content.Split(' ');
            foreach (var word in words)
            {
                if (!_spellChecker.Check(word))
                {
                    _misspelledWords.Add(word);
                }
            }
        }

        private void Rich_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            foreach (MenuItem item in suggestionsMenu.Items)
            {
                item.Click -= SuggestionMenuItem_Click;
            }

            suggestionsMenu.Items.Clear();

            var range = GetCurrentRange();
            var word = range.Text.Trim();
            if (string.IsNullOrWhiteSpace(word))
                return;

            if (!_spellChecker.Check(word))
            {
                var suggestions = _spellChecker.Suggest(word, 5);

                if (suggestions.Count > 0)
                {
                    foreach (var suggestion in suggestions)
                    {
                        var menuItem = new MenuItem
                        {
                            Header = suggestion,
                            Tag = range,
                        };

                        menuItem.Click += SuggestionMenuItem_Click;

                        suggestionsMenu.Items.Add(menuItem);
                    }
                }
                else
                {
                    var menuItem = new MenuItem
                    {
                        Header = "No suggestions",
                    };

                    suggestionsMenu.Items.Add(menuItem);
                }

                rich.ContextMenu.Visibility = Visibility.Visible;
            }
            else
            {
                rich.ContextMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void SuggestionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var suggestion = menuItem.Header as string;
            var range = menuItem.Tag as TextRange;
            var current = range.Text;

            int start = 0;
            for (int i = 0; i < current.Length; i++)
            {
                if (!char.IsWhiteSpace(current[i]))
                {
                    start = i;
                    break;
                }
            }

            int end = 0;
            for (int i = current.Length - 1; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(current[i]))
                {
                    end = i + 1;
                    break;
                }
            }

            range.Text = current.Substring(0, start) + suggestion + current.Substring(end);
            TextDecorationCollection tdc = new TextDecorationCollection();
            range.ApplyPropertyValue(Inline.TextDecorationsProperty, tdc);
        }

        private TextRange GetCurrentRange()
        {
            var selected = rich.Selection.Text;
            var offset = rich.Document.ContentStart.GetOffsetToPosition(rich.CaretPosition);

            var prefix = new TextRange(rich.Document.ContentStart, rich.CaretPosition).Text;

            int start = 0;
            for (int i = prefix.Length - 1; i >= 0; i--)
            {
                if (prefix[i] != '\r' || prefix[i] != '\n')
                    start--;
                if (prefix[i] == ' ')
                    break;
            }

            var suffix = new TextRange(rich.CaretPosition, rich.Document.ContentEnd).Text;
            int end = 0;
            for (int i = 0; i < suffix.Length; i++)
            {
                if (suffix[i] != '\r' || suffix[i] != '\n')
                    end++;
                if (suffix[i] == ' ')
                    break;
            }

            return new TextRange(rich.CaretPosition.GetPositionAtOffset(start), rich.CaretPosition.GetPositionAtOffset(end));
        }

        private void Rich_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (var change in e.Changes)
            {
                var from = rich.Document.ContentStart.GetPositionAtOffset(change.Offset) ?? rich.Document.ContentStart;
                var to = from.GetPositionAtOffset(change.AddedLength - change.RemovedLength) ?? rich.Document.ContentEnd;

                var previous = from;
                while (previous != null)
                {
                    from = previous;
                    var text = new TextRange(from, to).Text;
                    if (text.Length > 0 && char.IsWhiteSpace(text[0]))
                        break;
                    previous = previous.GetPositionAtOffset(-1);
                }

                var next = to;
                while (next != null)
                {
                    to = next;
                    var last = new TextRange(from, to).Text.LastOrDefault();
                    if (last == default(char) || char.IsWhiteSpace(last))
                        break;
                    next = next.GetPositionAtOffset(1);
                }

                var range = new TextRange(from, to);
                if (range == null) continue;

                var tdc = new TextDecorationCollection();
                range.ApplyPropertyValue(Inline.TextDecorationsProperty, tdc);

                var currentFrom = from;
                var currentEnd = from;
                while (currentEnd.CompareTo(to) < 0)
                {
                    var currentRange = new TextRange(currentFrom, currentEnd);
                    if (currentRange.Text.EndsWith(" ") || currentEnd.CompareTo(to) == 0)
                    {
                        currentEnd = currentEnd.GetPositionAtOffset(-1);
                        currentRange = new TextRange(currentFrom, currentEnd);

                        if (!_spellChecker.Check(currentRange.Text))
                        {
                            tdc = new TextDecorationCollection();
                            tdc.Add(_squigglyUnderline);

                            currentRange.ApplyPropertyValue(Inline.TextDecorationsProperty, tdc);
                        }

                        currentEnd = currentFrom = currentEnd.GetPositionAtOffset(1);
                    }

                    currentEnd = currentEnd.GetPositionAtOffset(1);
                }
            }
        }
    }
}
