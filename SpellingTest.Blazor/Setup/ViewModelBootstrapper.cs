using PolyhydraGames.Extensions;
using SpellingTest.Core.ViewModels.Quiz;

namespace SpellingTest.Web.Setup
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

        public static void RegisterViewModels(this WebApplicationBuilder builder)
        {
            var viewmodels = typeof(ViewModelBootstrapper).Assembly.GetViewModels()
                .Concat(typeof(QuizListPickerViewModel).Assembly.GetViewModels());
            foreach (var viewModelType in viewmodels)
            {
                builder.Services.AddScoped(viewModelType);
            }
        }
    }
}