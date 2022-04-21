using CardGame.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System.Windows;

namespace CardGame
{


    public class MainView : ReactiveObject
    {
        [Reactive]
        public Visibility GameViewIsVisible { get; set; } = Visibility.Collapsed;

        [Reactive]
        public Visibility AboutViewIsVisible { get; set; } = Visibility.Collapsed;
        [Reactive]
        public GameViewModel GameViewModel { get; set; }
        public AboutViewModel AboutViewModel { get; set; }
        public IReactiveCommand PlayCommand { get; set; }
        public IReactiveCommand AboutCommand { get; set; }
      
        public IReactiveCommand ExitCommand { get; set; }

        public MainView()
        {
            AudioPlayer.Instance.SetAudioFromResources(GameConstants.AudioUri);
            AudioPlayer.Instance.Volume = 1;
            AboutViewModel = new AboutViewModel();
            AboutViewModel.CloseEvent += AboutViewModel_CloseEvent;
            ExitCommand = ReactiveCommand.Create(() => App.Current.Shutdown());
            PlayCommand = ReactiveCommand.Create(() =>
            {
                GameViewModel = new GameViewModel();
                GameViewIsVisible = Visibility.Visible;

                GameViewModel.GameExitEvent += GameViewModel_GameExitEvent;
                GameViewModel.StartGame();

            });
            AboutCommand = ReactiveCommand.Create(() =>
            {
                AboutViewIsVisible = Visibility.Visible;
            });
        }

        private void AboutViewModel_CloseEvent()
        {
            AboutViewIsVisible = Visibility.Collapsed;
        }

        private void GameViewModel_GameExitEvent()
        {
            GameViewIsVisible = Visibility.Collapsed;
            GameViewModel.GameExitEvent -= GameViewModel_GameExitEvent;
        }
    }
}
