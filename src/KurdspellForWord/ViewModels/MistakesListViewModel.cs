using Kurdspell;
using KurdspellForWord.Helpers;
using KurdspellForWord.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace KurdspellForWord.ViewModels
{

    public class MistakesListViewModel : BindableBase
    {
        private readonly IDocumentService _documentService;
        private readonly SpellChecker _spellChecker;

        public MistakesListViewModel(IDocumentService documentService, SpellChecker spellChecker)
        {
            _documentService = documentService;
            _spellChecker = spellChecker;

            FindMistakesCommand = new DelegateCommand(_ =>
            {
                Mistakes.Clear();
                var words = _documentService.GetAllWords().ToList();
                foreach (var word in words)
                {
                    if (string.IsNullOrWhiteSpace(word.Literal)) continue;

                    if (_spellChecker.Check(word.Literal) == false)
                    {
                        var mistake = new MistakeViewModel
                        {
                            Start = word.Start,
                            End = word.End,
                            Text = word.Literal
                        };

                        mistake.Suggestions = _spellChecker.Suggest(word.Literal, 3)
                                                           .Select(i => new SuggestionViewModel(mistake, i))
                                                           .ToList();

                        Mistakes.Add(mistake);
                    }
                }
            });

            ApplyFixCommand = new DelegateCommand(p =>
            {
                var fix = p as SuggestionViewModel;
                if (fix is null) return;

                Mistakes.Remove(fix.Parent);
                var word = _documentService.Replace(fix.Parent.Start, fix.Parent.End, fix.Text);
                _documentService.Select(word);
                var delta = fix.Parent.Text.Length - fix.Text.Length;

                foreach (var mistake in Mistakes.Where(m => m.Start > fix.Parent.Start))
                {
                    mistake.Start -= delta;
                    mistake.End -= delta;
                }
            });

            AddToDictionaryCommand = new DelegateCommand(p =>
            {
                var mistake = p as MistakeViewModel;
                if (mistake is null) return;

                for (int i = Mistakes.Count - 1; i >= 0; i--)
                {
                    if (Mistakes[i].Text == mistake.Text)
                        Mistakes.Remove(Mistakes[i]);
                }

                _spellChecker.AddToDictionary(mistake.Text);
            });
        }

        private ObservableCollection<MistakeViewModel> _mistakes = new ObservableCollection<MistakeViewModel>();
        public ObservableCollection<MistakeViewModel> Mistakes
        {
            get { return _mistakes; }
            set { SetProperty(ref _mistakes, value); }
        }

        private MistakeViewModel _selectedItem;
        public MistakeViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public ICommand FindMistakesCommand { get; }

        public ICommand ApplyFixCommand { get; }

        public ICommand AddToDictionaryCommand { get; }

        public void ShowSelectedItem()
        {
            if (SelectedItem == null) return;
            _documentService.Select(new Word(SelectedItem.Text, SelectedItem.Start, SelectedItem.End));
        }
    }
}
