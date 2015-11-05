using QuizITT_WP.Common;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace QuizITT_WP
{
    public sealed partial class QuizPage : Page
    {
        //Dentro questa region ci sono le dichiarazioni che servono universalmente
        #region Dichiarazioni
        int domandaCorrente;
        int risposteCorrette = 0;
        Question[] domandeScelte;
        bool bottoneCliccato = false;
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
        //Navigation helper è un oggetto che gestisce la navigazione tra le varie pagine (form).
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
            //Questo metodo loadstate viene chiamato quando la pagina viene caricata ed è asincrono per permettere gli await
            lblIndicazione.Visibility = Visibility.Collapsed;
            //Leggo tutte le domande e le categorie dai file
            string modoQuiz = e.NavigationParameter as string;
            lblIntestazione.Text = modoQuiz;            
            Domande = new List<Question>();
            Categorie = new Dictionary<int, string>();
            //Espandi questa region per vedere 
            #region Carica domande
            try
            {
                //await è un metodo asincrono, l'app rimane sospesa finchè la lettura non è completata, per questo è necessario.
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

            }
            catch (Exception exc)
            {
                msg = new MessageDialog("Ho rilevato un'eccezione durante la lettura dei databases!\n" + exc.Message, "Hey user!");
                await msg.ShowAsync();
            }
            #endregion

            //Se scelgo un time attack allora preparo tutte le domande per la visualizzazione (al momento non si può usare perchè manca il timer)
            //Altrimenti nelle altre due modalità preparo le 10 domande che saranno proposte.
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
                //NON ANCORA PRONTO perchè manca il timer, il resto va.
                throw new NotImplementedException();


                //int quanteDomande = Domande.Count;
                //domandeScelte = new Question[quanteDomande];
                //for (int i = 0; i < quanteDomande; i++)
                //{
                //    domandeScelte[i] = Domande[r.Next(0, Domande.Count)];

                //    for (int a = 0; a < i; a++)
                //        if (domandeScelte[a] == domandeScelte[i])
                //        {
                //            //controllo se la domanda è già presente
                //            i--;
                //            break;
                //        }
                //}
                //preparo attack con timer
                //dt.Tick += new EventHandler(dt_Tick);  <------ questa è la parte da modificare
                //dt.Interval = new TimeSpan(0, 0, 1);
                //dt.Start();

            }
            //In ogni caso visualizzo la prima domanda
            VisualizzaDomanda();
        }

        //private void dt_Tick(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
        
        
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            //Questo metodo verrebbe chiamato all'uscita dalla page, non ci mettiamo niente perchè non ci serve.
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
            //Poichè ho visualizzato una nuova domanda l'utente deve nuovamente rispondere, resetto quindi il bottone.
            bottoneCliccato = false;
        }

        #region NavigationHelper registration

        
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
            //Metodo chiamato da tutti e 3 i bottoni
            Button btn = sender as Button;
            if (!bottoneCliccato)
            {
                lblIndicazione.Visibility = Visibility.Visible;
                bottoneCliccato = true;
                foreach (Answer risposta in domandeScelte[domandaCorrente].Answers)
                {
                    if (risposta.IsRight)
                    {
                        if ((string)btn.Content == risposta.Text)
                        {
                            risposteCorrette++;
                            btn.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                            //msg = new MessageDialog("Risposta Corretta", "Giusto!");  
                        }
                        else
                        {
                            //Visualizzo la risposta che era giusta e coloro quella scelta di rosso
                            btn.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            if ((string)btnR0.Content == risposta.Text)
                                btnR0.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                            if ((string)btnR1.Content == risposta.Text)
                                btnR1.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                            if ((string)btnR2.Content == risposta.Text)
                                btnR2.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                        }
                    }
                }
            }
            else
            {
                lblIndicazione.Visibility = Visibility.Collapsed;
                btnR0.Background = null;
                btnR1.Background = null;
                btnR2.Background = null; 
                domandaCorrente++;
                if (lblIntestazione.Text != "Time Attack")                
                    if (domandaCorrente < 10)
                        VisualizzaDomanda();
                    else
                    {
                        msg = new MessageDialog("Hai totalizzato " + risposteCorrette + " punti", "Game Over");
                        await msg.ShowAsync();
                        this.Frame.Navigate(typeof(MainPage));
                    }
            }
                
        }
    }
}
