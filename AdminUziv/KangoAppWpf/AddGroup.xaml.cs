using System;
using System.Windows;
using System.Windows.Documents;
using Entity;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for AddGroup.xaml
    /// </summary>
    public partial class AddGroup : Window
    {
        /// <summary>
        /// Meno novej skupiny
        /// </summary>
        private string _nMeno;
        /// <summary>
        /// Typ novej skupiny
        /// </summary>
        private FTyp _nTyp = 0x0000000;
        /// <summary>
        /// Poznamka novej skupiny
        /// </summary>
        private string _nPoznamka;

        /// <summary>
        /// Property vyjadrujuca 
        /// </summary>
        public bool PodariloSa { get; set; }
        /// <summary>
        /// Getter pre meno novej skupiny
        /// </summary>
        /// <returns>Vrati meno novej skupiny</returns>
        public string GetMeno() { return _nMeno; }
        /// <summary>
        /// Getter pre typ novej skupiny
        /// </summary>
        /// <returns>Vrati typ novej skupiny</returns>
        public FTyp GetTyp() { return _nTyp; }
        /// <summary>
        /// Getter pre poznamku novej skupiny
        /// </summary>
        /// <returns>Vrati poznamku novej skupiny</returns>
        public string GetPoznamka() { return _nPoznamka; }

        /// <summary>
        /// Inicializacia okna pre vytvorenie novej skupiny
        /// </summary>
        public AddGroup()
        {
            InitializeComponent();
            cbNS_Typ.ItemsSource = System.Enum.GetValues(typeof(FTyp));
            PodariloSa = false;
        }

        /// <summary>
        /// Potvrdenie vytvorenia novej skupiny
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool meno = false; bool typ = false;
            if (txtNS_Meno.Text != "") { _nMeno = txtNS_Meno.Text; meno = true; }
            if (cbNS_Typ.Text != "")
            {
                if (cbNS_Typ.SelectedValue.ToString() != FTyp.VSETKO.ToString() || cbNS_Typ.SelectedValue.ToString() != FTyp.Administrátor.ToString())
                {
                    Enum.TryParse<FTyp>(cbNS_Typ.SelectedValue.ToString(), out _nTyp);
                    typ = true;
                }
                else
                {
                    if (cbNS_Typ.SelectedValue.ToString() == FTyp.Administrátor.ToString() && ((MainWindow)Owner).PrihlasenyStav &&
                        FTyp.Administrátor.ToString() != ((MainWindow)Owner).Logika.GetPouzivatel(((MainWindow)Owner).PrihlasenyMeno).Typ.ToString())
                    {
                        Enum.TryParse<FTyp>(cbNS_Typ.SelectedValue.ToString(), out _nTyp);
                        typ = true;
                    }
                    else
                    {
                        MessageBox.Show("Typ môže zvloiť len prihlásený administrátor!");
                    }
                    if (cbNS_Typ.SelectedValue.ToString() == FTyp.VSETKO.ToString())
                    {
                        MessageBox.Show("Nepovolený typ!");
                    }
                }
            }
            TextRange textRange = new TextRange(txtNS_Poznamka.Document.ContentStart, txtNS_Poznamka.Document.ContentEnd);
            if (textRange.Text != "") { _nPoznamka = textRange.Text; }
            if (meno && typ) { PodariloSa = true; }
            if (PodariloSa)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Nastala chyba. Skontrolujte si svoje údaje.");
            }
            this.Close();
        }
    }
}
