using Microsoft.AspNetCore.Components;

namespace SpellingTest.Web.Models
{
    public class ListHolder<T>
    {
        public string Text { get; set; }
        public string Id { get; set; }
        public T Value { get; set; }
        public bool Selected { get; set; }
        public static T GetValue(List<ListHolder<T>> list, ChangeEventArgs args)
        {
            if (args == null) return default;
            var first = list.FirstOrDefault(x => x.Id.ToString() == args.Value?.ToString());
            return first == null ? default(T) : first.Value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}