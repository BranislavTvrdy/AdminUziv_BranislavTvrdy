using System.Windows;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for RenewPass.xaml
    /// </summary>
    public partial class RenewPass : Window
    {

        public string VstupOkna { get; set; }

        /// <summary>
        /// Inicializacia okna pre obnovu alebo generovanie noveho hesla
        /// </summary>
        /// <param name="paPrihlaseny">Stav prihlasenia pouzivatela</param>
        public RenewPass(bool paPrihlaseny)
        {
            InitializeComponent();
            VstupOkna = "";
            if (paPrihlaseny)
            {
                lP_Cinnost.Content = "Zadajte vaše nové heslo:";
            }
            else
            {
                lP_Cinnost.Content = "Zadajte prihlasovacie meno vášho konta:";
            }
        }

        /// <summary>
        /// Potvrdenie generovania noveho hesla
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VstupOkna = txt_VstupOkna.Text;
            this.Close();
        }
    }
}
