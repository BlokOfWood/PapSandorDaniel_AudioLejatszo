﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AudioLejatszo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<string> _playlist = new BindingList<string>();
        MediaPlayer _mediaPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            OpenSongList.ItemsSource = _playlist;
            _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;
        }

        private void _mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (OpenSongList.SelectedIndex == -1) return;

            OpenSongList.SelectedIndex++;
            if (OpenSongList.SelectedIndex != OpenSongList.Items.Count-1)
            {
                _mediaPlayer.Open(new Uri((string)OpenSongList.SelectedItem));
                Play();
            }
            else
            {
                Pause();
                OpenSongList.SelectedIndex = -1;
                _mediaPlayer.Stop();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
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
                _mediaPlayer.Open(new Uri((string)OpenSongList.SelectedItem));
                Play();
            }
            catch(Exception)
            {
                MessageBox.Show("Nem lehet lejátszani a kiválasztott fájlt!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
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

        
    }
}
