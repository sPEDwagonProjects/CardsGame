using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace CardGame
{
    public class Card : ReactiveObject
    {
        public string OpennedImage { get; set; }
        public string ClosedImage { get; set; }
        public int Id { get; set; }

        [Reactive]
        public bool IsOpenned { get; set; }

        public void StateChanged() => IsOpenned = !IsOpenned;

    }
}
