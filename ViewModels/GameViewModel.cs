using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CardGame
{
    public enum GameResult
    {
        Winned,
        Failed,
    }
    public class GameViewModel : ReactiveObject
    {

        private DispatcherTimer _timer;
        private TimeSpan _time;
        public bool IsPaused { get; private set; }


        public int Volume { get; set; }

        public delegate void GameExit();
        public event GameExit GameExitEvent;
        [Reactive]
        public bool IsHit { get; set; }
        [Reactive]
        public Visibility GameResultVisibility { get; set; } = Visibility.Collapsed;
        [Reactive]
        public Visibility GamePauseVisibility { get; set; } = Visibility.Collapsed;
        [Reactive]
        public string ResultInfo { get; set; }

        [Reactive]
        public string TimeText { get; set; }
        [Reactive]
        public int Level { get; set; } = 1;

        public IReactiveCommand RestartGameCommand { get; set; }
        public IReactiveCommand ExitGameCommand { get; set; }
        public IReactiveCommand PauseResumeGameCommand { get; set; }

        [Reactive]
        public ObservableCollection<Card> DataCollection { get; set; }
        public GameViewModel()
        {
            RestartGameCommand = ReactiveCommand.Create(() => StartGame());

            ExitGameCommand = ReactiveCommand.Create(() =>
            {
                AudioPlayer.Instance?.Stop();
                StopTime();
                GameExitEvent?.Invoke();
            });
            PauseResumeGameCommand = ReactiveCommand.Create(() =>
            {
                if (IsPaused)
                {
                    IsPaused = false;
                    GamePauseVisibility = Visibility.Collapsed;
                    AudioPlayer.Instance?.Play();
                    StartTime();
                }
                else
                {
                    IsPaused = true;
                    StopTime();
                    GamePauseVisibility = Visibility.Visible;

                    AudioPlayer.Instance?.Pause();
                }
            });
        }
        private void ShowGameResult(GameResult result)
        {
            GameResultVisibility = Visibility.Visible;
            switch (result)
            {
                case GameResult.Winned:
                    {

                        ResultInfo = "ВЫ ВЫЙГРАЛИ!";
                        break;

                    }
                case GameResult.Failed:
                    {
                        ResultInfo = "ВЫ ПРОИГРАЛИ!";
                        break;
                    }
            }
            AudioPlayer.Instance.Stop();


        }
        private void HideGameResult() =>
            GameResultVisibility = Visibility.Hidden;
        public void StartGame()
        {
            SetLevelAndStartGame(1);

            AudioPlayer.Instance.Play();
        }
        public void TimeInit()
        {
            _time = TimeSpan.FromSeconds(GameConstants.TimeOfLevel[Level]);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Background, delegate
            {
                _time = _time.Add(TimeSpan.FromSeconds(-1));

                if (_time == TimeSpan.Zero)
                {
                    _timer.Stop();
                    if (Level > 1)
                    {
                        SetLevelAndStartGame(Level - 1);
                    }
                    else
                    {
                        ShowGameResult(GameResult.Failed);

                    }

                }
                TimeText = _time.ToString("c");

            }, System.Windows.Application.Current.Dispatcher);
            TimeText = _time.ToString("c");

        }
        public void StopTime() => _timer?.Stop();
        public void StartTime() => _timer?.Start();
        public Task SetLevelAndStartGame(int level)
        {

            DataCollection = new ObservableCollection<Card>();
            return Task.Run(async () =>
            {
                IsHit = false;
                Level = level;
                HideGameResult();
                TimeInit();

                var items = await LoadGameDataOfLevel(Level);

                foreach (var item in items)
                {
                    item.IsOpenned = true;

                    App.Current.Dispatcher.Invoke(() => { DataCollection.Add(item); });
                    await Task.Delay(100);
                };

                if (items.Count == 4)
                    await Task.Delay(300);

                else if (items.Count == 6)
                    await Task.Delay(500);

                else if (items.Count == 8)
                    await Task.Delay(800);

                else if (items.Count == 10)
                    await Task.Delay(1500);

                foreach (var item in DataCollection)
                {
                    item.IsOpenned = false;
                    await Task.Delay(100);
                };
                IsHit = true;
                StartTime();
            });
        }
        public void Shuffle<T>(List<T> list)
        {
            Random rand = new Random();
            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = list[j];
                list[j] = list[i];
                list[i] = tmp;
            }
        }
        public async Task<List<Card>> LoadGameDataOfLevel(int level)
        {
           return await Task.Run(() =>
           {

               var info = GameConstants.GameDataOfLevel[level];

               List<Card> list = LoadData(info.path, info.count, GameConstants.DefaultImageUri);
               Shuffle(list);
               return list;

           });
        }
        public List<Card> LoadData(string path, int count, string standartImagePath)
        {

            var GetFiles = new DirectoryInfo(path).GetFiles().ToList();
            List<Card> fileDict = new List<Card>();
            int k = 0;
            for (int i = 0; i < GetFiles.Count;)
            {
                if (k == count) break;
                string firstPath = GetFiles[i].FullName;
                string secondPath = Path.Combine(GetFiles[i].DirectoryName,
                                    Path.GetFileNameWithoutExtension(GetFiles[i].Name) + "_r" + GetFiles[i].Extension);

                if (GetFiles.FirstOrDefault(x => x.FullName == secondPath) != null)
                {

                    fileDict.Add(new Card()
                    {
                        Id = i,
                        ClosedImage = standartImagePath,
                        OpennedImage = firstPath,
                    });
                    fileDict.Add(new Card()
                    {
                        Id = i,
                        ClosedImage = standartImagePath,
                        OpennedImage = secondPath,
                    });

                    i += 2;
                    k++;
                }
                else
                {

                    fileDict.Add(new Card()
                    {
                        Id = i,
                        ClosedImage = standartImagePath,
                        OpennedImage = firstPath,
                    });
                    fileDict.Add(new Card()
                    {
                        Id = i,
                        ClosedImage = standartImagePath,
                        OpennedImage = firstPath,
                    });

                    i++;
                    k++;

                }

            }
            return fileDict;
        }
        public void CheckCards()
        {

            var items = DataCollection.Where(x => x.IsOpenned);
            if (items.Count() == 2)
            {
                var item1 = items.ElementAt(0);
                var item2 = items.ElementAt(1);
                if (item1.Id == item2.Id)
                {
                    DataCollection.Remove(item1);
                    DataCollection.Remove(item2);
                }
                else
                {
                    item1.IsOpenned = false;
                    item2.IsOpenned = false;
                }
            }

            if (DataCollection.Count == 0)
            {

                _timer.Stop();
                if (Level == 8)
                {
                    ShowGameResult(GameResult.Winned);

                }
                else
                {
                    SetLevelAndStartGame(Level + 1);
                }
            }
        }
    }
}

