using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.Storage;
using System;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace QuizITT_WP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {        
        public MainPage()
        {
            this.InitializeComponent();
            
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Questo metodo viene chiamato quando la pagina viene visualizzata
            //Proviamo a recuperare i setting dell'utente, se non li troviamo facciamo senza
            try
            {
                txtImpNome.Text = (string)ApplicationData.Current.LocalSettings.Values["NomeUtente"];
                txtImpCognome.Text = (string) ApplicationData.Current.LocalSettings.Values["CognomeUtente"];
                string dataPrelevata = (string) ApplicationData.Current.LocalSettings.Values["DataNascita"];
                string[] container = dataPrelevata.Split(';');
                dpkImpData.Date = new DateTime(Convert.ToInt32(container[0]), Convert.ToInt32(container[1]), Convert.ToInt32(container[2]));
                cmbImpSpec.SelectedIndex = (int)ApplicationData.Current.LocalSettings.Values["Special"];
                lblCiao.Text= "Ciao "+txtImpNome.Text+"! Bentornato su QuizITT";
                
            }
            catch(Exception)
            {                
                //Ci basta gestire l'eccezione, non dobbiamo fare nulla in caso di non recupero, vuol dire solo che è la prima volta che si gioca
            }
        }

        private void btnPlayClassic_Click(object sender, RoutedEventArgs e)
        {
            //Avviamo la navigazione verso la pagina quiz, passando come parametro la mdoalità scelta
            this.Frame.Navigate(typeof(QuizPage),btnPlayClassic.Content);
        }

        private void btnPlayTime_Click(object sender, RoutedEventArgs e)
        {
            /* Purtroppo al momento non si può giocare in modlaità time attack
            serve un timer che su windows phone ha una gestione diversa da quella di windows classico e non funziona
            non appena trovata una soluzione il progetto verrà aggiornato e sarà riscaricabile da github.com/amadeinic nel repository apposito.
            */
            this.Frame.Navigate(typeof(QuizPage),btnPlayTime.Content);
        }

        private void btnPlayInsane_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(QuizPage), btnPlayInsane.Content);
        }

        private void btnImpostaUtente_Click(object sender, RoutedEventArgs e)
        {
            //Alla pressione del bottone salva utente, salviamo i parametri nel dictionary dei settings, messo a disposizione dal sistema operativo
            ApplicationData.Current.LocalSettings.Values["NomeUtente"] = txtImpNome.Text;
            ApplicationData.Current.LocalSettings.Values["CognomeUtente"] = txtImpCognome.Text;
            ApplicationData.Current.LocalSettings.Values["DataNascita"] = dpkImpData.Date.Year + ";" + dpkImpData.Date.Month + ";" + dpkImpData.Date.Day;
            ApplicationData.Current.LocalSettings.Values["Special"] = cmbImpSpec.SelectedIndex;
        }
    }
}
