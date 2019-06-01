using KurdspellForWord.Helpers;
using System.Collections.Generic;

namespace KurdspellForWord.ViewModels
{
    public class MistakeViewModel : BindableBase
    {
        public int Start { get; set; }
        public int End { get; set; }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public List<SuggestionViewModel> Suggestions { get; set; }
    }

    public class SuggestionViewModel : BindableBase
    {
        public SuggestionViewModel(MistakeViewModel parent, string text)
        {
            Text = text;
            Parent = parent;
        }

        public MistakeViewModel Parent { get; }

        public string Text { get; }
    }
}
