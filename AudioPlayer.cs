using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CardGame
{
    public enum AudioPlayerStateEnum
    {
        Stopped,
        Paused,
        Played,
    }
    public class AudioPlayer
    {

        private MediaPlayer _MediaPlayer;
        private static AudioPlayer _instance;
        private double _volume;
        public static AudioPlayer Instance
        {
            get => _instance == null ? (_instance = new AudioPlayer()) : _instance;
        }

        public double Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                _MediaPlayer.Volume = _volume;

            }
        }

        public AudioPlayerStateEnum AudioPlayerState { get; set; }
        private AudioPlayer()
        {
            _MediaPlayer = new MediaPlayer();

            _MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            _MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
            AudioPlayerState = AudioPlayerStateEnum.Stopped;
        }

        public void Stop()
        {
            _MediaPlayer.Stop();
            AudioPlayerState = AudioPlayerStateEnum.Stopped;
        }

        public void Pause()
        {
            _MediaPlayer.Pause();
            AudioPlayerState = AudioPlayerStateEnum.Paused;
        }

        public void Play()
        {
            _MediaPlayer.Volume = 0;
            _MediaPlayer.Play();

            AudioPlayerState = AudioPlayerStateEnum.Played;
            Task.Run(async () =>
            {
                for (double i = 0; i < Volume; i += 0.05)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _MediaPlayer.Volume = i;
                    }, System.Windows.Threading.DispatcherPriority.Background);
                    await Task.Delay(300);
                }
            });


        }


        public void SetAudio(string path) => _MediaPlayer.Open(new Uri(path, UriKind.RelativeOrAbsolute));

        public void SetAudioFromResources(string name)
        {
            var obj = Properties.Resources.ResourceManager.GetObject(Path.GetFileNameWithoutExtension(name));
            if (obj is byte[])
            {
                var dStream = (byte[])obj;
                var tmpfile = Path.GetTempPath() + "CardGame" + name;
                if (!File.Exists(tmpfile))
                {
                    try
                    {
                        var fs = File.OpenWrite(tmpfile);
                        fs.Write(dStream, 0, dStream.Length);
                        fs.Close();
                        SetAudio(tmpfile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка расспаковки музыки:\n" + ex.Message);
                        File.Delete(tmpfile);

                    }
                }
                else SetAudio(tmpfile);
                dStream = null;

            }

        }


        private void MediaPlayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            AudioPlayerState = AudioPlayerStateEnum.Stopped;

        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            _MediaPlayer.Position = new TimeSpan(0);
            _MediaPlayer.Play();
        }



    }
}

