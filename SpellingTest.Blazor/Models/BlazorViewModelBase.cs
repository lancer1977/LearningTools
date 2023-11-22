using PolyhydraGames.Core.ReactiveUI;
using SpellingTest.Web.Services.CurrentPage;

namespace SpellingTest.Web.Models;

public class BlazorViewModelBase : ViewModelAsyncBase
{
    private readonly ICurrentPage _currentPage;

    public BlazorViewModelBase(ICurrentPage currentPage)
    {
        _currentPage = currentPage;
    }

    /// <summary>
    /// Initializes the view
    /// </summary>
    /// <returns></returns>
    public override async Task StartAsync()
    {
        await base.StartAsync();
        _currentPage.SetName(Title);

    }

    /// <summary>
    /// Any items that need to be pulled again
    /// </summary>
    /// <returns></returns>
    public async Task OnRefreshAsync()
    {
    }
}

public class BlazorViewModelModalBase : ViewModelAsyncBase
{
}