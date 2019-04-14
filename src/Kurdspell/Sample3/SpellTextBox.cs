using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Timers;
using Kurdspell;

namespace Sample3
{
    // from: https://github.com/lsd1991/SpellTextBox/blob/master/SpellTextBox/SpellTextBox.cs
    public class SpellTextBox : TextBox
    {
        static SpellTextBox()
        {
            TextProperty.OverrideMetadata(typeof(SpellTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
        }

        public SpellTextBox() : base()
        {
            CreateTimer();

            var cm = new ContextMenu();
            this.ContextMenu = cm;
            this.ContextMenu.Opened += OnContextMenuOpening;

            Loaded += (s, e) =>
            {
                Initialize();
                if (Window.GetWindow(this) != null)
                    Window.GetWindow(this).Closing += (s1, e1) => Dispose();
            };
        }

        #region Timer

        static Timer textChangedTimer = new System.Timers.Timer(500);
        ElapsedEventHandler textChangedTimerOnElapse;

        void CreateTimer()
        {
            textChangedTimer.AutoReset = false;
            textChangedTimerOnElapse = new ElapsedEventHandler(textChangedTimer_Elapsed);
            textChangedTimer.Elapsed += textChangedTimerOnElapse;

        }

        private static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((SpellTextBox)sender).IsSpellcheckCompleted = false;
            textChangedTimer.Stop();
            textChangedTimer.Start();
        }

        private void textChangedTimer_Elapsed(object sender,
        System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new System.Action(() =>
            {
                Checker.CheckSpelling(Text);
                RaiseSpellcheckCompletedEvent();
            }));
        }

        #endregion

        #region SpellcheckCompleted Event

        public static readonly RoutedEvent SpellcheckCompletedEvent = EventManager.RegisterRoutedEvent(
            "SpellcheckCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SpellTextBox));

        public event RoutedEventHandler SpellcheckCompleted
        {
            add { AddHandler(SpellcheckCompletedEvent, value); }
            remove { RemoveHandler(SpellcheckCompletedEvent, value); }
        }

        void RaiseSpellcheckCompletedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(SpellTextBox.SpellcheckCompletedEvent);
            RaiseEvent(newEventArgs);
            IsSpellcheckCompleted = true;
        }

        public bool IsSpellcheckCompleted { get; set; }

        #endregion

        protected void OnContextMenuOpening(object sender, RoutedEventArgs e)
        {
            if (Checker.MisspelledWords.Any(w => SelectionStart >= w.Index && SelectionStart <= w.Index + w.Length))
                Checker.SelectedMisspelledWord = Checker.MisspelledWords.First(w => SelectionStart >= w.Index && SelectionStart <= w.Index + w.Length);
            else
                Checker.SelectedMisspelledWord = null;

            this.ContextMenu.Items.Clear();
            foreach (var item in Checker.MenuActions)
            {
                var mi = new MenuItem();
                mi.Header = item.Name;
                mi.Command = item.Command;
                this.ContextMenu.Items.Add(mi);
            }
        }

        public static readonly DependencyProperty DictionaryPathProperty =
            DependencyProperty.Register(
            "DictionaryPath",
            typeof(string),
            typeof(SpellTextBox));

        public string DictionaryPath
        {
            get { return (string)this.GetValue(DictionaryPathProperty); }
            set { this.SetValue(DictionaryPathProperty, value); }
        }

        public static readonly DependencyProperty CustomDictionaryPathProperty =
            DependencyProperty.Register(
            "CustomDictionaryPath",
            typeof(string),
            typeof(SpellTextBox));

        public string CustomDictionaryPath
        {
            get { return (string)this.GetValue(CustomDictionaryPathProperty) ?? "custom.txt"; }
            set { this.SetValue(CustomDictionaryPathProperty, value); }
        }

        public static readonly DependencyProperty IsSpellCheckEnabledProperty =
            DependencyProperty.Register(
            "IsSpellCheckEnabled",
            typeof(bool),
            typeof(SpellTextBox));

        public bool IsSpellCheckEnabled
        {
            get { return (bool)this.GetValue(IsSpellCheckEnabledProperty); }
            set { this.SetValue(IsSpellCheckEnabledProperty, value); }
        }

        AdornerLayer myAdornerLayer;
        RedUnderlineAdorner myAdorner;

        public void Initialize()
        {
            myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
            myAdorner = new RedUnderlineAdorner(this);
            myAdornerLayer.Add(myAdorner);
        }

        public void Dispose()
        {
            myAdorner.Dispose();
            myAdornerLayer.Remove(myAdorner);
            this.ContextMenu.Opened -= OnContextMenuOpening;
            textChangedTimer.Elapsed -= textChangedTimerOnElapse;
            if (checker != null)
                checker.Dispose();
        }

        private SpellCheckerVM checker;

        public SpellCheckerVM Checker
        {
            get { return checker ?? CreateSpellCheker(); }
            set { checker = value; }
        }

        private SpellCheckerVM CreateSpellCheker()
        {
            checker = new SpellCheckerVM(new SpellChecker(PatternsRepository.GetPatterns(), PatternsRepository.GetRules()), this);
            checker.LoadCustomDictionary();
            return checker;
        }

        public void ReplaceSelectedWord(Word WordToReplaceWith)
        {
            if (WordToReplaceWith.Text != "No suggestions")
            {
                int index = Checker.SelectedMisspelledWord.Index;
                string replacement = WordToReplaceWith.Text;
                Text = Text.Remove(index, Checker.SelectedMisspelledWord.Length).Insert(index, replacement);
                SelectionStart = index + WordToReplaceWith.Length;
            }
        }

        public void FireTextChangeEvent()
        {
            int c = SelectionStart;
            string s = Text;
            Text = s + " ";
            Text = s;
            SelectionStart = c;
        }

        private ICommand _replace;
        public ICommand Replace
        {
            get
            {
                return _replace ?? (_replace = new DelegateCommand(
                        delegate
                        {
                            ReplaceSelectedWord(checker.SelectedSuggestedWord);
                        }));
            }
        }
    }
}
