using Microsoft.Win32;
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
        BindingList<string> openFilePaths = new BindingList<string>();
        MediaPlayer mediaPlayer;

        public MainWindow()
        {
            InitializeComponent();
            OpenSongList.ItemsSource = openFilePaths;
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
                fileDialog.FileNames.ToList().ForEach(x => openFilePaths.Add(x));
            }
        }

        private void PlayStop_Click(object sender, RoutedEventArgs e)
        {
            if ((string)PlayStop.Content == "Play" && openFilePaths.Count != 0)
            {
                PlayStop.Content = "Stop";
            }
            else if ((string)PlayStop.Content == "Stop")
            {
                PlayStop.Content = "Play";
            }
        }
    }
}
