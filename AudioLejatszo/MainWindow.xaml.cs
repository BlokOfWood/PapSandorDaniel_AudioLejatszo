using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace AudioLejatszo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<string> _playlist = new BindingList<string>();
        MediaPlayer _mediaPlayer = new MediaPlayer();
        DispatcherTimer _mediaPlayerTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            OpenSongList.ItemsSource = _playlist;
            _mediaPlayer.MediaOpened += _mediaPlayer_MediaOpened;
            _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;
        }

        private void _mediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                timeSlider.Maximum = _mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                _mediaPlayerTimer.Interval = TimeSpan.FromMilliseconds(200);
                _mediaPlayerTimer.Tick += UpdateTime;
                _mediaPlayerTimer.Start();
                timeSlider.IsEnabled = true;
            }
        }

        void UpdateTime(object sender, EventArgs e)
        {
            timeSlider.Value = _mediaPlayer.Position.TotalMilliseconds;
            if(_mediaPlayer.NaturalDuration.HasTimeSpan)
            timeDisplay.Text = $"{_mediaPlayer.Position.ToString(@"m\:ss")}/{_mediaPlayer.NaturalDuration.TimeSpan.ToString(@"m\:ss")}";
            
        }

        private void _mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (OpenSongList.SelectedIndex == -1) return;

            if (OpenSongList.SelectedIndex != OpenSongList.Items.Count - 1)
            {
                OpenSongList.SelectedIndex++;
                PlayCurrentSong();
            }
            else
            {
                Pause();
                OpenSongList.SelectedIndex = -1;
                _mediaPlayer.Stop();
                timeSlider.IsEnabled = false;
            }
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Audio Files (*.mp3; *.flac; *.wav) |*.mp3;*.flac;*.wav"
            }; 
            //Valamilyen csodálatos okból "bool?"-t ad vissza, pedig elvileg nem is lehet null.
            if ((bool)fileDialog.ShowDialog())
            {
                fileDialog.FileNames.ToList().ForEach(x => 
                { 
                    _playlist.Add(x);
                });
                OpenSongList.SelectedIndex = 0;
                Play();
            }
        }

        private void PlayStop_Click(object sender, RoutedEventArgs e)
        {
            if ((string)PlayStop.Content == "Play" && _playlist.Count != 0)
            {
                Play();
            }
            else if ((string)PlayStop.Content == "Pause")
            {
                Pause();
            }
        }

        private void OpenSongList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OpenSongList.SelectedIndex == -1) return;
            try
            {
                PlayCurrentSong();
            }
            catch(Exception)
            {
                System.Windows.MessageBox.Show("Nem lehet lejátszani a kiválasztott fájlt!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void Play()
        {
            if (OpenSongList.SelectedIndex == -1) return;

            PlayStop.Content = "Pause";
            _mediaPlayer.Play();
        }

        void Pause()
        {
            PlayStop.Content = "Play";
            _mediaPlayer.Pause();
        }

        void PlayCurrentSong()
        {
            _mediaPlayer.Open(new Uri((string)OpenSongList.SelectedItem));
            Play();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Pause();
            OpenSongList.SelectedIndex = -1;
            _mediaPlayer.Stop();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (OpenSongList.SelectedIndex != 0)
                OpenSongList.SelectedIndex--;
            PlayCurrentSong();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (OpenSongList.SelectedIndex != OpenSongList.Items.Count-1)
                OpenSongList.SelectedIndex++;
            PlayCurrentSong();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaPlayer.Volume = e.NewValue;
        }

        private void FolderOpen_Click(object sender, RoutedEventArgs e)
        {
            //Valamilyen csodálatos okból a wpf-ben nincsen natív folder browser, legalábbis ezalatt a .net verzió alatt, 
            //szóval a windows formsosat kell használnom, ami egy kicsit necces, de annyira nagyon más egyszerű alternatíva nincs.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            string[] musicFiles = Directory.GetFiles(folderBrowserDialog.SelectedPath);
            musicFiles.ToList().ForEach(x => { 
                if (x.EndsWith(".mp3") || x.EndsWith(".flac") || x.EndsWith(".wav"))
                    _playlist.Add(x); 
            });
        }

        private void timeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaPlayer.Position = TimeSpan.FromMilliseconds(e.NewValue);
        }

        private void FolderOpen_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
