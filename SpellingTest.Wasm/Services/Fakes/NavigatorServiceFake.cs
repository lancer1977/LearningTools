using PolyhydraGames.Core.Interfaces;

namespace SpellingTest.Wasm.Services.Fakes;

public class MainThreadDispatcher : IMainThreadDispatcher
{
    public void InvokeOnMainThread(Action action)
    {
        action.Invoke();
    }
}

public class NavigatorServiceFake : INavigatorAsync
{
    public async Task<IViewModelAsync> PopAsync(bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task<IViewModelAsync> PopPopupAsync(bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task<IViewModelAsync> PopModalAsync(bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task PopToRootAsync(bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task PushPopupAsync<TViewModel>(Func<TViewModel, Task> setStateAction, bool animated = true) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task PushPopupAsync<TViewModel, TPage>(TViewModel viewModel, TPage page, bool animated = true) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task PushPopupAsync(Type viewModel, bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task PushPopupAsync(Type viewModel, Func<object, Task> setStateAction = null, bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task PushModalAsync<TViewModel>(Func<TViewModel, Task> setStateAction = null, bool animated = true) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task PushModalAsync<TViewModel>(TViewModel viewModel, bool animated = true) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task PushModalAsync<TViewModel, TPage>(TViewModel viewModel, TPage page, bool animated = true) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task PushModalAsync(Type viewModel, Func<object, Task> setStateAction, bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task PushModalAsync(Type viewModel, bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task NavigateTo(Type type, Func<object, Task> setStateAction = null)
    {
        throw new NotImplementedException();
    }

    public async Task NavigateTo<TViewModel>(Func<TViewModel, Task> setStateAction = null) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task PushAsync<TViewModel>(TViewModel viewModel, bool animated = true) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task PushAsync<TViewModel>(Func<TViewModel, Task> setStateAction = null, bool animated = true) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task PushAsync(Type viewModel, bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task PushAsync(Type viewModel, Func<object, Task> setStateAction = null, bool animated = true)
    {
        throw new NotImplementedException();
    }

    public async Task SetRoot<TViewModel>(Func<TViewModel, Task> setStateAction = null) where TViewModel : class, IViewModelAsync
    {
        throw new NotImplementedException();
    }

    public async Task SetRoot(Type type, Func<object, Task> setStateAction = null)
    {
        throw new NotImplementedException();
    }
}