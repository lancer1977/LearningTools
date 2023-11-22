using PolyhydraGames.Learning.Dtos;

namespace SpellingTest.Maui.Pages.Spelling
{
    public partial class SpellingListEditorPage
    {
        void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var selected = e.SelectedItem;
            list.SelectedItem = null;
            ViewModel.ItemPickCommand.Execute(selected as Quiz);
        }

        public SpellingListEditorPage()
        {
            InitializeComponent();
        }
    }
} 
