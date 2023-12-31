﻿using Microsoft.AspNetCore.Components;
using SpellingTest.Core.Interfaces;

namespace SpellingTest.Wasm.Services.Wrappers;

public class NavigationHelper : ISpellingNavigatorService
{
    private readonly NavigationManager _manager;

    public NavigationHelper(NavigationManager manager)
    {
        _manager = manager;
    }


    public async Task NavigateTo(string path)
    {
        _manager.NavigateTo(path);
    }

    public async Task ShowQuiz(ITopic topic)
    {
        _manager.NavigateTo($"quiz?id={topic.Id}");
    }

    public async Task ShowFlashCard(ITopic topic)
    {
        _manager.NavigateTo($"quiz?flash={topic.Id}");
    }
}