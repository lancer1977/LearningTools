using ReactiveUI.Fody.Helpers;
using System.Runtime.Serialization;

namespace SpellingTest.Core.ViewModels.CardGame
{

    public class Card : ViewModelAsyncBase
    {
        private static int _count;

        public Card()
        {
            Id = _count += 1;
            ShowName = true;
            ShowImage = true;
        }
        public Card(IDefinition def) : this()
        {
            Image = def.Image;
            Name = def.Name;
        }

        [DataMember(Name = "Image")]
        public string Image { get; set; }
        [DataMember(Name = "Name")]
        public string Name { get; set; }
        public int Id { get; set; }
        [Reactive] public bool ShowName { get; set; }
        [Reactive] public bool ShowImage { get; set; }
    }
}
