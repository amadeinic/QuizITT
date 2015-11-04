using QuizITT_WP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Threading;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace QuizITT_WP
{   
    public sealed partial class QuizPage : Page
    {
        #region Dichiarazioni
        int domandaCorrente;
        int risposteCorrette = 0;
        Question[] domandeScelte;
        DispatcherTimer dt = new DispatcherTimer();
        Random r = new Random();
        public List<Question> Domande { get; set; }
        public Dictionary<int, string> Categorie { get; set; }
        MessageDialog msg;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        #endregion
        public QuizPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }       
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }        
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }        
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            string modoQuiz = e.NavigationParameter as string;
            lblIntestazione.Text = modoQuiz;
            msg = new MessageDialog("Aho LoadState"+modoQuiz.ToString());
            await msg.ShowAsync();
            Domande = new List<Question>();
            Categorie = new Dictionary<int, string>();
            #region carica domande
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
                msg = new MessageDialog("Ho rilevato un'eccezione durante la lettura dei databases!\n" + exc.Message, "Hey user!");
                await msg.ShowAsync();
            }
            #endregion
            if(modoQuiz!="Time Attack")
            {
                int quanteDomande = 10;
                domandeScelte = new Question[quanteDomande];
                for (int i = 0; i < quanteDomande; i++)
                {

                    domandeScelte[i] = Domande[r.Next(0, Domande.Count)];
                    if (modoQuiz == "Insane Mode" && domandeScelte[i].Level != 4)
                    {
                        i--;
                    }
                    else                    
                        for (int a = 0; a < i; a++)                        
                            if (domandeScelte[a] == domandeScelte[i])
                            {
                                //controllo se la domanda è già presente
                                i--;
                                break;
                            }
                }               
            }
            else
            {
                int quanteDomande = Domande.Count;
                domandeScelte = new Question[quanteDomande];
                for (int i = 0; i < quanteDomande; i++)
                {
                    domandeScelte[i] = Domande[r.Next(0, Domande.Count)];

                    for (int a = 0; a < i; a++)
                        if (domandeScelte[a] == domandeScelte[i])
                        {
                            //controllo se la domanda è già presente
                            i--;
                            break;
                        }
                }
                //preparo attack con timer
                //dt.Tick += new EventHandler(dt_Tick);
                //dt.Interval = new TimeSpan(0, 0, 1);
                //dt.Start();

            }
            //In ogni caso
            VisualizzaDomanda();
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            msg = new MessageDialog("Aho SaveState");
            await msg.ShowAsync();
        }
        
        private void VisualizzaDomanda()
        {
            Question miaDomanda = domandeScelte[domandaCorrente];
            lblDomanda.Text = miaDomanda.Text;
            miaDomanda.MixAnswers();
            btnR0.Content = miaDomanda.Answers[0].Text;
            btnR1.Content = miaDomanda.Answers[1].Text;
            btnR2.Content = miaDomanda.Answers[2].Text;
            lblTitolo.Text = Categorie[miaDomanda.Category];
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        private async void btnR_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            foreach (Answer risposta in domandeScelte[domandaCorrente].Answers)
            {
                if ((string)btn.Content == risposta.Text)
                {
                    if (risposta.IsRight)
                    {
                        risposteCorrette++;
                        msg = new MessageDialog("Risposta Corretta", "Giusto!");
                    }
                    else
                    {
                        msg = new MessageDialog("Risposta Errata", "Sbagliato!");
                    }
                    await msg.ShowAsync();
                    domandaCorrente++;
                    if (lblIntestazione.Text != "Time Attack")
                    {
                        if (domandaCorrente < 10)
                            VisualizzaDomanda();
                        else
                        {
                            msg = new MessageDialog("Hai totalizzato " +risposteCorrette +" punti", "Game Over");
                            await msg.ShowAsync();
                            this.Frame.Navigate(typeof(MainPage));
                        }
                    }
                        
                }

            }
                
        }
    }
}
