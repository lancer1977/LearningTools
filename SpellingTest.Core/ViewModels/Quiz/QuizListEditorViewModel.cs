//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using PolyhydraGames.Core.Interfaces;
//using PolyhydraGames.Core.IOC;
//using PolyhydraGames.Core.ReactiveUI;
//using PolyhydraGames.Extensions;
//using PolyhydraGames.Learning.Dtos;
//using PolyhydraGames.Learning.Interfaces;
//using ReactiveUI;
//using ReactiveUI.Fody.Helpers;
//using SpellingTest.Service;

//namespace SpellingTest.ViewModels.Spelling
//{
//    public class SpellingListEditorViewModel : ViewModelAsyncBase
//    {
//        private readonly IQuizService _service;
//        private readonly IDictionaryService _dictionaryService; 
//        private readonly IDialogService _display;
//        public ICommand AddGroupCommand { get; }
//        public ICommand SaveCommand { get; }
//        public ICommand MenuCommand { get; }
//        public ICommand ItemPickCommand { get; }
//        public ICommand AddUrlCommand { get; }

//        [Reactive] public string GroupText { get; set; }
//        [Reactive] public string URL { get; set; }
//        [Reactive] public List<IDefinition> Words { get; set; }
//        [Reactive] public IQuiz IQuiz { get; set; }





//        public SpellingListEditorViewModel(IQuizService service, IDialogService display, IDictionaryService dictionaryService )
//        {
//            _service = service;
//            _dictionaryService = dictionaryService; 
//            _display = display;
//            //_modelService
//            SaveCommand = ReactiveCommand.Create(() =>
//            {
//                _service.Update(IQuiz);
//                IOC.Get<INavigator>().PopAsync();
//            });

//            AddUrlCommand = ReactiveCommand.CreateFromTask(async () =>
//            {
//                try
//                {
//                    if (string.IsNullOrEmpty(URL))
//                    {
//                        return;
//                    }

//                    var response = await DataExtensions.GetStringFromUrlAsync(URL);
//                    List<Definition> words = null;
//                    if (URL.Contains(".json"))
//                    {
//                        words = response.FromJson<List<Definition>>();
//                    }
//                    if (URL.Contains(".csv"))
//                    {
//                        words = await DataExtensions.GetListFromCSV<Definition>(response);
//                    }
//                    foreach (var item in words)
//                    {
//                        item.ListId = IQuiz.Id;
//                        _modelService.Insert(item);
//                    };
//                    RefreshList();
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine(ex.Message);
//                }

//            });

//            ItemPickCommand = ReactiveCommand.Create<Definition>(async (i) =>
//            {
//                var choice = await _display.InputBoxAsync(
//                    i.Name + " Menu",
//                    new[] { "Update Title", "Update Definition", "Delete" });
//                switch (choice.Result)
//                {
//                    case "Update Title":
//                        UpdateWordTitle(i);
//                        break;
//                    case "Update Definition":
//                        UpdateDefinition(i);
//                        break;

//                    case "Delete":
//                        if ((await _display.InputBoxAsync("Really delete", i.Name)).Ok)
//                            _modelService.Delete(i);
//                        break;

//                }


//            });
//            MenuCommand = ReactiveCommand.Create(async () =>
//            {
//                var result = await display.GetStringAsync("Word", "Title");
//                if (result.Ok)
//                    await AddWord(result.Result);
//            });

//            AddGroupCommand = ReactiveCommand.Create(async () =>
//            {
//                var items = GroupText.Split(',');
//                var invalidItems = new List<string>();
//                foreach (var item in items)
//                {
//                    await AddWord(item.Trim().Capitalize());
//                }

//                GroupText = "";
//                if (invalidItems.Any())
//                {
//                    GroupText = invalidItems.Aggregate((a, b) => a + "," + b);
//                    await display.NotificationAsync("Invalid items were left in the entry window", "Error");
//                }
//                RefreshList();
//            });
//        }

//        private async Task AddWord(string word)
//        {
//            var Description = await _dictionaryService.GetAsync(word);
//            var model = new Definition()
//            {
//                Name = word,
//                ListId = IQuiz.Id,
//                Description = definition
//            };
//            if (string.IsNullOrEmpty(definition))
//            {
//                UpdateDefinition(model);
//            }
//            _modelService.Insert(model);
//        }
//        public async Task LoadAsync(IQuiz IQuiz)
//        {
//            IQuiz = IQuiz;
//            RefreshList();
//        }

//        private void RefreshList()
//        {
//            Words = _modelService.Items.Where(i => i.ListId == IQuiz.Id).ToList();
//        }
//        public void ItemSelectedAction(Definition value)
//        {
//            ItemPickCommand.Execute(value);

//        }
//        async void UpdateWordTitle(Definition model)
//        {
//            var result = await _display.GetStringAsync("Enter Title", model.Name);
//            if (result.Ok)
//            {
//                model.Name = result.Result;
//                _modelService.Update(model);
//            }

//        }
//        async void UpdateDefinition(Definition model)
//        {
//            var result = await _display.GetStringAsync("Enter definition", model.Name);
//            if (result.Ok)
//            {
//                model.Description = result.Result;
//                _modelService.Update(model);
//            }
//        }
//    }
//}
