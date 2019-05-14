namespace Sample3
{
    // from: https://github.com/lsd1991/SpellTextBox/blob/master/SpellTextBox/Word.cs
    public class Word
    {
        public int Index { get; set; }

        public string Text { get; set; }

        public int Length
        {
            get { return Text.Length; }
        }

        public Word()
        {
        }

        public Word(string text, int index)
        {
            Index = index;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
