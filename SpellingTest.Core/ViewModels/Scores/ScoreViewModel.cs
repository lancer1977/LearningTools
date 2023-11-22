using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Microsoft.AppCenter.Crashes;
using PolyhydraGames.Learning.Dtos;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpellingTest.Core.Helpers;

namespace SpellingTest.Core.ViewModels.Scores
{
    public class EnumSelection<T>
    {
        public T Selection { get; }
        public string Title { get; }
        public override string ToString()
        {
            return Title;
        }
        public EnumSelection(T selection, string title = null)
        {
            Selection = selection;
            Title = string.IsNullOrEmpty(title) ? selection.ToString().ToCapitalizedString() : title;
        }
    }
    public class ScoreViewModel : ViewModelAsyncBase
    {
        public ScoreViewModel(IMathScoreService mathService)
        {
            MathService = mathService;
         
            IdiomSelections = new List<EnumSelection<Idiom?>>()   
            {
                new EnumSelection<Idiom?>(null,"None")
            };
            foreach (Idiom item in Enum.GetValues(typeof(Idiom)))
            {
                IdiomSelections.Add(new EnumSelection<Idiom?>(item));
            }

            FeatureSelections = new List<EnumSelection<Feature?>>()
            {
                new EnumSelection<Feature?>(null,"None")
            };
            foreach (Feature item in Enum.GetValues(typeof(Feature)))
            {
                
                FeatureSelections.Add(new EnumSelection<Feature?>(item));
            }

            var connection = _items.Connect();
            var filterFunc = this.WhenAnyValue(x => x.SelectedIdiom, x => x.SelectedFeature).Select((x) => {
                return new Func<SpeedMathResult, bool>(result =>
                {
                    if (x.Item1.Selection != null && x.Item1.Selection != Idiom.TV) return false;
                    if (x.Item2.Selection != null && x.Item2.Selection != result.Operation) return false;
                    return true;
                });
            });
            connection.ObserveOn(RxApp.MainThreadScheduler)
                .Sort(SortExpressionComparer<SpeedMathResult>.Descending(x => x.Date))
                .Filter<SpeedMathResult>(filterFunc)
                .Bind(out _collection)
                .Subscribe();
            SelectedIdiom = IdiomSelections.First();
            SelectedFeature = FeatureSelections.First();
        }
        public List<EnumSelection<Idiom?>> IdiomSelections { get;  }
        [Reactive] public EnumSelection<Idiom?> SelectedIdiom { get; set; }
        public List<EnumSelection<Feature?>> FeatureSelections { get; }
        [Reactive] public EnumSelection<Feature?> SelectedFeature { get; set; }
        private SourceList<SpeedMathResult> _items = new SourceList<SpeedMathResult>();
        private ReadOnlyObservableCollection<SpeedMathResult> _collection;
        public ReadOnlyObservableCollection<SpeedMathResult> Items => _collection;
        private IMathScoreService MathService { get; }
        
        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            await GatherItems();
        }

        private async Task GatherItems()
        {
            try
            {
                var getItems = await MathService.Items();
                if (!getItems.Any()) return;
                _items.Edit(x =>
                {
                    x.Clear();
                    x.AddRange(getItems.Cast<SpeedMathResult>());
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Crashes.TrackError(ex);
            }

        }


    }
}