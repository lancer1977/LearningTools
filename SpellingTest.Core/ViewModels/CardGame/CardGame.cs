using PolyhydraGames.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;

namespace SpellingTest.Core.ViewModels.CardGame
{
    public class CardGame : ReactiveObject
    {
        private readonly ITextToSpeech _textToSpeech;

        [ObservableAsProperty] public string Instructions { get; set; }
        [Reactive] public Card CorrectCard { get; set; }
        [Reactive] public Card Card1 { get; set; }
        [Reactive] public Card Card2 { get; set; }
        [Reactive] public Card Card3 { get; set; }
        public Queue<Card> Cards { get; set; }
        [Reactive] public bool GameOver { get; set; }
        public Card[] VisibleCards => new[] { Card1, Card2, Card3 };

        public void SetVisibility(bool showName, bool showImage)
        {
            var allCards = Cards.Concat(VisibleCards);
            foreach (var item in allCards)
            {
                item.ShowImage = showImage;
                item.ShowName = showName;
            }
        }
        public CardGame(ITextToSpeech textToSpeech)
        {
            _textToSpeech = textToSpeech;
            this.WhenAnyValue(x => x.CorrectCard).Select(i => i != null ? $"Find \r\n {i.Name}" : "").ToPropertyEx(this, x => x.Instructions, "");
            this.WhenAnyValue(x => x.Instructions).Where(x => !string.IsNullOrEmpty(x)).Do(x =>
            {
                _textToSpeech.Speak(x);
            }).Subscribe();
        }
        public void Load(IEnumerable<Card> deck)
        {
            var cards = deck.ToList();
            cards.Shuffle();
            Cards = new Queue<Card>(cards);
        }

        internal void Initialize()
        {
            Card1 = Cards.Dequeue();
            Card2 = Cards.Dequeue();
            Card3 = Cards.Dequeue();
            CorrectCard = Card1;
        }

        public SelectionResponse PickCard(string cardName)
        {
            var correct = CorrectCard.Name == cardName;
            if (correct == false)
            {
                return new SelectionResponse { Message = $"You picked wrong:\r\n  {cardName}" };
            }
            try
            {
                var newCard = Cards.Dequeue();
                if (Card1.Name == cardName)
                {
                    Card1 = newCard;
                }
                else if (Card2.Name == cardName)
                {
                    Card2 = newCard;
                }
                else if (Card3.Name == cardName)
                {
                    Card3 = newCard;
                }
                var random = PolyhydraGames.Extensions.Dice.DiceRoll.RollDie(3) - 1;
                var cards = new[] { Card1, Card2, Card3 };
                CorrectCard = cards[random];
                return new SelectionResponse { Message = $"You picked the correct card!! \r\n {cardName}" };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return new SelectionResponse { GameOver = true, Message = "Hit Reset to start a new game" };
        }

    }
}
