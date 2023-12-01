using Microsoft.AspNetCore.Components;
using ReactiveUI;
using System.Windows.Input;

namespace SpellingTest.Wasm.Models;

public abstract class EditorViewModelBase<T> : BlazorViewModelModalBase
{
    public T Model { get; set; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; set; }
    public abstract Task Save(T item);
    public abstract Task Delete(T item);
    public abstract Task<bool> Validate(T item);
    public abstract void Load(T item, EventCallback<T> callback);

    protected EditorViewModelBase()
    {
        Model = Activator.CreateInstance<T>();
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!await Validate(Model)) return;
            await Save(Model);
        });

        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!await ConfirmDelete(Model)) return;
            await Delete(Model);
        });
    }

    private async Task<bool> ConfirmDelete(T model)
    {
        //var dialogResult = Dialog.GetConfirmation("Are you sure you want to delete?","yes","no");
        return true;
    }
}