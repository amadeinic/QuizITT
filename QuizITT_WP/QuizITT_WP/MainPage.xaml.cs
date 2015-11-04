using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace QuizITT_WP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class MainPage : Page
    {
        public List<Question> Domande { get; set; }
        public Dictionary<int,string> Categorie { get; set; }
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {            

        }

        private void btnPlayClassic_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(QuizPage),btnPlayClassic.Content);
        }

        private void btnPlayTime_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(QuizPage),btnPlayTime.Content);
        }

        private void btnPlayInsane_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(QuizPage), btnPlayInsane.Content);
        }
    }
}
