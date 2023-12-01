using PolyhydraGames.Extensions;
using System.Reflection;

namespace SpellingTest.Wasm.Setup
{
    public static class ViewModelBootstrapper
    {
        public static List<Type> ExceptionTypes = new List<Type>
        {
        };

        public static IEnumerable<Type> GetViewModels(this Assembly assembly)
        {
            return assembly.GetTypes().EndingWith("ViewModel").Except(ExceptionTypes)
                .Where(x => !(x.IsAbstract || x.IsInterface)).ToList();
        }

        public static void RegisterViewModels(this IServiceCollection builder)
        {
            var viewmodels = typeof(ViewModelBootstrapper).Assembly.GetViewModels()
                .Concat(typeof(QuizListPickerViewModel).Assembly.GetViewModels());
            foreach (var viewModelType in viewmodels)
            {
                builder.AddScoped(viewModelType);
            }
        }
    }
}