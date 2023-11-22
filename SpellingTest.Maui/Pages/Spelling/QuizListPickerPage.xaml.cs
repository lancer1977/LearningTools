namespace SpellingTest.Maui.Pages.Spelling
{
    public partial class QuizListPickerPage
    { 

        public QuizListPickerPage()
        {
            InitializeComponent();

        }

        private   void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var selected = e.SelectedItem;
            list.SelectedItem = null;
            ViewModel.ActionPickCommand.Execute(selected);
        } 
    }
} 
