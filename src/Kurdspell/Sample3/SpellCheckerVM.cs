using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Input;
using System;
using System.IO;
using Kurdspell;

namespace Sample3
{
    // from: https://github.com/lsd1991/SpellTextBox/blob/master/SpellTextBox/SpellChecker.cs
    public class SpellCheckerVM : INotifyPropertyChanged
    {
        private SpellTextBox _box;
        private SpellChecker _kurdspell;
        private List<Word> _words;
        private ObservableCollection<Word> _misspelledWords;
        private ObservableCollection<Word> _suggestedWords;
        private List<Word> _ignoredWords;
        private Word _selectedMisspelledWord;
        private Word _selectedSuggestedWord;

        public SpellCheckerVM(SpellChecker kurdspell, SpellTextBox parent)
        {
            _kurdspell = kurdspell;
            _box = parent;
            Words = new List<Word>();
            MisspelledWords = new ObservableCollection<Word>();
            IgnoredWords = new List<Word>();
            SuggestedWords = new ObservableCollection<Word>();
        }

        public void Dispose()
        {

        }

        public List<Word> Words
        {
            get { return _words; }
            set
            {
                _words = value;
                OnPropertyChanged("Words");
            }
        }

        public ObservableCollection<Word> MisspelledWords
        {
            get { return _misspelledWords; }
            set
            {
                _misspelledWords = value;
                OnPropertyChanged("MisspelledWords");
                if (SelectedMisspelledWord != null)
                    SelectedMisspelledWord = null;
            }
        }

        public ObservableCollection<MenuAction> MenuActions
        {
            get
            {
                List<MenuAction> commands = SuggestedWords.Select(w => new MenuAction()
                {
                    Name = w.Text,
                    Command = new DelegateCommand(
                        delegate
                        {
                            _box.ReplaceSelectedWord(w);
                        })
                }).ToList();

                if (commands.Count == 0)
                {
                    commands.Add(new MenuAction()
                    {
                        Name = "Copy",
                        Command = ApplicationCommands.Copy
                    });
                    commands.Add(new MenuAction()
                    {
                        Name = "Cut",
                        Command = ApplicationCommands.Cut
                    });
                    commands.Add(new MenuAction()
                    {
                        Name = "Paste",
                        Command = ApplicationCommands.Paste
                    });
                }
                else
                {
                    //commands.Add(new MenuAction()
                    //{
                    //    Name = "Add custom",
                    //    Command = new DelegateCommand(
                    //        delegate
                    //        {
                    //            SaveToCustomDictionary(SelectedMisspelledWord);

                    //            _box.FireTextChangeEvent();
                    //        })
                    //});
                }

                return new ObservableCollection<MenuAction>(commands);
            }
        }

        public ObservableCollection<Word> SuggestedWords
        {
            get { return _suggestedWords; }
            set
            {
                _suggestedWords = value;
                OnPropertyChanged("SuggestedWords");
            }
        }

        public List<Word> IgnoredWords
        {
            get { return _ignoredWords; }
            set
            {
                _ignoredWords = value;
                OnPropertyChanged("IgnoredWords");
            }
        }

        public Word SelectedMisspelledWord
        {
            get { return _selectedMisspelledWord; }
            set
            {
                _selectedMisspelledWord = value;
                LoadSuggestions(value);
                OnPropertyChanged("SelectedMisspelledWord");
                OnPropertyChanged("IsReplaceEnabled");
            }
        }

        public Word SelectedSuggestedWord
        {
            get { return _selectedSuggestedWord; }
            set
            {
                _selectedSuggestedWord = value;
                OnPropertyChanged("SelectedSuggestedWord");
                OnPropertyChanged("IsReplaceEnabled");
            }
        }

        public void LoadSuggestions(Word misspelledWord)
        {
            if (misspelledWord != null)
            {
                SuggestedWords = new ObservableCollection<Word>(_kurdspell.Suggest(misspelledWord.Text, 5).Select(s => new Word(s, misspelledWord.Index)));
                if (SuggestedWords.Count == 0) SuggestedWords = new ObservableCollection<Word> { new Word("No suggestions", 0) };
            }
            else
            {
                SuggestedWords = new ObservableCollection<Word>();
            }
            OnPropertyChanged("SuggestedWords");
        }

        public void ClearLists()
        {
            Words.Clear();
            MisspelledWords.Clear();
        }

        public void CheckSpelling(string content)
        {
            if (_box.IsSpellCheckEnabled)
            {
                ClearLists();

                var matches = Regex.Matches(content, @"\w+[^\s]*\w+|\w");

                foreach (Match match in matches)
                {
                    Words.Add(new Word(match.Value.Trim(), match.Index));
                }

                foreach (var word in Words)
                {
                    bool isIgnored = IgnoredWords.Contains(word);
                    if (!isIgnored)
                    {
                        bool exists = _kurdspell.Check(word.Text);
                        if (exists)
                            IgnoredWords.Add(word);
                        else
                            MisspelledWords.Add(word);
                    }
                }

                OnPropertyChanged("MisspelledWords");
                OnPropertyChanged("IgnoredWords");
            }
        }

        public void LoadCustomDictionary()
        {
            //string[] strings = File.ReadAllLines(_box.CustomDictionaryPath);
            //foreach (var str in strings)
            //{
            //    _kurdspell.Add(str);
            //}
        }

        public void SaveToCustomDictionary(Word word)
        {
            //File.AppendAllText(_box.CustomDictionaryPath, string.Format("{0}{1}", word.Text.ToLower(), Environment.NewLine));
            //_kurdspell.Add(word.Text);
            IgnoredWords.Add(word);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MenuAction : INotifyPropertyChanged
    {
        private string name;
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        private ICommand command;
        public ICommand Command { get { return command; } set { command = value; OnPropertyChanged("Command"); } }

        public override string ToString()
        {
            return Name;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
