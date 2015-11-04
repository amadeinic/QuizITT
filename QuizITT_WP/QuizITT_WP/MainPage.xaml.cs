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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Domande = new List<Question>();
            Categorie = new Dictionary<int, string>();
            try
            {
                var Folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                Folder = await Folder.GetFolderAsync("QuizData");
                var File = await Folder.GetFileAsync("questions.csv");
                var lines = await Windows.Storage.FileIO.ReadLinesAsync(File);
                lines.RemoveAt(0);
                foreach (string st in lines)
                {
                    Domande.Add(new Question(st));
                }

                File = await Folder.GetFileAsync("categories.csv");
                lines = await Windows.Storage.FileIO.ReadLinesAsync(File);
                lines.RemoveAt(0);
                foreach (string st in lines)
                {
                    Categorie.Add(Convert.ToInt32(st.Split(';')[0]), st.Split(';')[1]);
                }
                //foreach (Question q in Domande)
                //{
                //    if (Categorie.ContainsKey(q.Category))
                //    {

                //    }
                //}
                
            }
            catch (Exception exc)
            {
                MessageDialog msg = new MessageDialog("Ho rilevato un'eccezione durante la lettura dei databases!\n"+exc.Message,"Hey user!");
                await msg.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {            

        }

        private void btnPlayClassic_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(QuizPage),0);
        }

        private void btnPlayTime_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(QuizPage), 1);
        }

        private void btnPlayInsane_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(QuizPage), 2);
        }
    }
}
