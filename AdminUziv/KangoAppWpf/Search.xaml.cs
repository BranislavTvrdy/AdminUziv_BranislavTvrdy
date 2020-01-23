using System;
using System.Windows;
using Entity;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        /// <summary>
        /// Meno hľadanej položky
        /// </summary>
        private string _sMeno;
        /// <summary>
        /// Typ hľadanej položky
        /// </summary>
        private FTyp sTyp = 0x0000000;
        /// <summary>
        /// Email hľadanej položky
        /// </summary>
        private string _sEmail;
        /// <summary>
        /// Telefónne číslo hľadanej položky
        /// </summary>
        private string _sTelefon;
        /// <summary>
        /// Aktívny status hľadanej položky
        /// </summary>
        private string _sAktivny;
        /// <summary>
        /// Vedúci hľadanej položky 
        /// </summary>
        private string _sVeduci;
        /// <summary>
        /// Indikator hľadanej položky
        /// T - uzivatel, F - skupina
        /// </summary>
        private bool _typHladania = true;
        /// <summary>
        /// Indikuje vykonané vyhľadávanie
        /// </summary>
        public bool Hladaj { get; set; }

        /// <summary>
        /// Getter mena hľadanej položky
        /// </summary>
        /// <returns>Meno hľadanej položky</returns>
        public string GetMeno() { return _sMeno; }
        /// <summary>
        /// Getter typu hľadanej položky
        /// </summary>
        /// <returns>Typ hľadanej položky</returns>
        public FTyp GetTyp() { return sTyp; }
        /// <summary>
        /// Getter emailu hľadanej položky
        /// </summary>
        /// <returns>Email hľadanej položky</returns>
        public string GetEmail() { return _sEmail; }
        /// <summary>
        /// Getter telefónneho čísla hľadanej položky
        /// </summary>
        /// <returns>Telefónne číslo hľadanej položky</returns>
        public string GetTelefon() { return _sTelefon; }
        /// <summary>
        /// Getter aktívneho statusu hľadanej položky
        /// </summary>
        /// <returns>Aktívny status hľadanej položky</returns>
        public string GetAktivny() { return _sAktivny; }
        /// <summary>
        /// Getter vedúceho hľadanej položky
        /// </summary>
        /// <returns>Vedúci hľadanej položky</returns>
        public string GetVeduci() { return _sVeduci; }
        /// <summary>
        /// Getter typu vyhľadávania
        /// </summary>
        /// <returns>Typ vyhľadávania</returns>
        public bool GetTypHladania() { return _typHladania; }

        /// <summary>
        /// Konštruktor triedy Search
        /// </summary>
        public Search()
        {
            InitializeComponent();
            cbS_TypUzivatel.ItemsSource = System.Enum.GetValues(typeof(FTyp));
            cbS_TypSkupina.ItemsSource = System.Enum.GetValues(typeof(FTyp));
            cbS_TypUzivatel.SelectedIndex = cbS_TypUzivatel.Items.IndexOf(FTyp.VSETKO);
            cbS_TypSkupina.SelectedIndex = cbS_TypSkupina.Items.IndexOf(FTyp.VSETKO);
            Hladaj = false;
        }

        /// <summary>
        /// Potvrdenie hladania alebo filtrovania
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tc_Vyhladavanie.SelectedIndex == 0)
            {
                _typHladania = true;
                Hladaj = true;
                if (txtS_MenoUzivatel.Text != "") { _sMeno = txtS_MenoUzivatel.Text; }
                if (cbS_TypUzivatel.Text != "")
                {
                    Enum.TryParse<FTyp>(cbS_TypUzivatel.SelectedValue.ToString(), out sTyp);
                }
                if (txtS_EmailUzivatel.Text != "") { _sEmail = txtS_EmailUzivatel.Text; }
                if (txtS_TelefonUzivatel.Text != "") { _sTelefon = txtS_TelefonUzivatel.Text; }
                if (cbS_AktivnyUzivatel.IsChecked != null && (bool) cbS_AktivnyUzivatel.IsChecked) { _sAktivny = "A"; }

            }
            if (tc_Vyhladavanie.SelectedIndex == 1)
            {
                _typHladania = false;
                Hladaj = true;
                if (txtS_MenoSkupina.Text != "") { _sMeno = txtS_MenoSkupina.Text; }
                if (cbS_TypSkupina.Text != "")
                {
                    Enum.TryParse<FTyp>(cbS_TypSkupina.SelectedValue.ToString(), out sTyp);
                }
                if (txtS_VeduciSkupina.Text != "") { _sVeduci = txtS_VeduciSkupina.Text; }
            }
            this.Close();
        }
    }
}
