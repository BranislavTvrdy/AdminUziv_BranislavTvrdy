using System;
using System.Windows;
using System.Windows.Documents;
using Entity;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        /// <summary>
        /// Meno nového používateľa
        /// </summary>
        private string _nMeno;
        /// <summary>
        /// Heslo nového používateľa
        /// </summary>
        private string _nHeslo;
        /// <summary>
        /// Typ nového používateľa
        /// </summary>
        private FTyp _nTyp = 0x0000000;
        /// <summary>
        /// Email nového používateľa
        /// </summary>
        private string _nEmail;
        /// <summary>
        /// Telefónne číslo nového používateľa
        /// </summary>
        private string _nTelefon;
        /// <summary>
        /// Poznámka nového používateľa
        /// </summary>
        private string _nPoznamka;
        /// <summary>
        /// Property vyjadrujúca úspešnosť vytvorenia nového používateľa
        /// </summary>
        public bool PodariloSa { get; set; }

        /// <summary>
        /// GEtter mena nového používateľa
        /// </summary>
        /// <returns>Meno nového používateľa</returns>
        public string GetMeno() { return _nMeno; }
        /// <summary>
        /// Getter hesla nového používateľa
        /// </summary>
        /// <returns>Heslo nového používateľa</returns>
        public string GetHeslo() { return _nHeslo; }
        /// <summary>
        /// Getter typu nového používateľa
        /// </summary>
        /// <returns>Typ nového používateľa</returns>
        public FTyp GetTyp() { return _nTyp; }
        /// <summary>
        /// Getter emailu nového používateľa
        /// </summary>
        /// <returns>Email nového používateľa</returns>
        public string GetEmail() { return _nEmail; }
        /// <summary>
        /// Getter telefónneho čísla nového používateľa
        /// </summary>
        /// <returns>Telefónne číslo nového používateľa</returns>
        public string GetTelefon() { return _nTelefon; }
        /// <summary>
        /// Getter poznámky nového používateľa
        /// </summary>
        /// <returns>Poznámka nového používateľa</returns>
        public string GetPoznamka() { return _nPoznamka; }

        /// <summary>
        /// Inicializacia okna pre vytvorenie noveho uzivatela
        /// </summary>
        public AddUser()
        {
            InitializeComponent();
            cbN_Typ.ItemsSource = System.Enum.GetValues(typeof(FTyp));
            PodariloSa = false;
        }

        /// <summary>
        /// Potvrdenie vytvorenia noveho uzivatela
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool email, meno, heslo, typ = false;
            if (txtN_Meno.Text != "") { _nMeno = txtN_Meno.Text; meno = true; } else { meno = false; }
            if (txtN_Heslo.Text != "") { _nHeslo = txtN_Heslo.Text; heslo = true; } else { heslo = false; }
            if (cbN_Typ.Text != "")
            {
                if (cbN_Typ.SelectedValue.ToString() != FTyp.VSETKO.ToString() || cbN_Typ.SelectedValue.ToString() != FTyp.Administrátor.ToString())
                {
                    Enum.TryParse<FTyp>(cbN_Typ.SelectedValue.ToString(), out _nTyp);
                    typ = true;
                }
                else
                {
                    if (cbN_Typ.SelectedValue.ToString() == FTyp.Administrátor.ToString() && ((MainWindow)Owner).PrihlasenyStav &&
                    FTyp.Administrátor.ToString() != ((MainWindow)Owner).Logika.GetPouzivatel(((MainWindow)Owner).PrihlasenyMeno).Typ.ToString())
                    {
                        Enum.TryParse<FTyp>(cbN_Typ.SelectedValue.ToString(), out _nTyp);
                        typ = true;
                    }
                    else
                    {
                        MessageBox.Show("Typ môže zvloiť len prihlásený administrátor!");
                    }
                    if (cbN_Typ.SelectedValue.ToString() == FTyp.VSETKO.ToString())
                    {
                        MessageBox.Show("Nepovolený typ!");
                    }
                }
            }
            if (txtN_Email.Text != "" && txtN_Email.Text.Contains("@")) { _nEmail = txtN_Email.Text; email = true; } else { email = false; }
            if (txtN_Telefon.Text != "") { _nTelefon = txtN_Telefon.Text; }
            TextRange textRange = new TextRange(txtN_Poznamka.Document.ContentStart, txtN_Poznamka.Document.ContentEnd);
            if (textRange.Text != "") { _nPoznamka = textRange.Text; }
            if (email && meno && heslo && typ) { PodariloSa = true; }
            if (PodariloSa)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Nastala chyba. Skontrolujte si svoje údaje.");
            }
        }
    }
}
