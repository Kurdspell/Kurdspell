using Microsoft.Office.Interop.Word;
using System.Collections;
using System.Collections.Generic;

namespace KurdspellForWord.Services
{
    public interface IDocumentService
    {
        IEnumerable<Word> GetAllWords();
        void Select(Word selectedWord);
        Word Replace(int start, int end, string text);
    }

    public class DocumentSerivce : IDocumentService
    {
        private readonly Document _document;

        public DocumentSerivce(Document document)
        {
            _document = document;
        }

        public IEnumerable<Word> GetAllWords()
        {
            foreach(Range word in _document.Words)
            {
                if (word.Text.EndsWith(" "))
                {
                    var text = word.Text.TrimEnd();
                    yield return new Word(text, word.Start, word.Start + text.Length);
                }
                else
                {
                    yield return new Word(word.Text, word.Start, word.End);
                }
            }
        }

        public void Select(Word selectedWord)
        {
            var range = _document.Range(selectedWord.Start, selectedWord.End);
            range.Select();
        }

        public Word Replace(int start, int end, string text)
        {
            var range = _document.Range(start, end);
            range.Text = text;
            return new Word(range.Text, range.Start, range.End);
        }
    }

    public class Word
    {
        public Word()
        {

        }
        
        public Word(string literal, int start, int end)
        {
            Literal = literal;
            Start = start;
            End = end;
        }

        public string Literal { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
