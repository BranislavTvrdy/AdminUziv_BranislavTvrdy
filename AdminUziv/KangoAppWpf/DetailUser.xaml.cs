using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Entity;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for DetailUser.xaml
    /// </summary>
    public partial class DetailUser : Window
    {
        /// <summary>
        /// Typ zobrazeného používateľa
        /// </summary>
        private FTyp nTyp = 0x0000000;
        /// <summary>
        /// Email zobrazeného používateľa
        /// </summary>
        private readonly string _nEmail;

        /// <summary>
        /// Právo na editáciu zobrazeného používateľa
        /// </summary>
        public bool PravoZmeny;
        /// <summary>
        /// Objekt zobrazeného používateľa
        /// </summary>
        private readonly Pouzivatel _aktUzivatel;
        /// <summary>
        /// Indikátor vykonanej editácie zobrazeného používateľa
        /// </summary>
        public bool BolaZmena { get; set; }

        /// <summary>
        /// Inicializacia okna pre zobrazenie detailov uzivatela
        /// </summary>
        /// <param name="paPravaUpravy"></param>
        /// <param name="paUzivatelOrig"></param>
        /// <param name="paZaclenenie"></param>
        public DetailUser(bool paPravaUpravy, Pouzivatel paUzivatelOrig, HashSet<Skupina> paZaclenenie)
        {
            InitializeComponent();
            PravoZmeny = paPravaUpravy;
            BolaZmena = false;
            var nMeno = paUzivatelOrig.Meno;
            nTyp = paUzivatelOrig.Typ;
            _nEmail = paUzivatelOrig.Email;
            var nTelefon = paUzivatelOrig.Telefon;
            var nPoznamka = paUzivatelOrig.Poznamka;
            _aktUzivatel = paUzivatelOrig;
            cbDU_Typ.ItemsSource = System.Enum.GetValues(typeof(FTyp));

            // VIZUAL
            txtDU_Meno.Text = nMeno;
            cbDU_Typ.SelectedItem = nTyp;
            txtDU_Email.Text = _nEmail;
            txtDU_Telefon.Text = nTelefon;
            
            new TextRange(txtDU_Poznamka.Document.ContentStart, txtDU_Poznamka.Document.ContentEnd)
            {
                Text = nPoznamka
            };

            DataGridGroups.ItemsSource = paZaclenenie;
            if (!paPravaUpravy)
            {
                btnDU_Edituj.IsEnabled = false;
            }
            
        }

        /// <summary>
        /// vykonanie editácie udajov uzivatela
        /// </summary>
        private void BtnDuEditujClick(object sender, RoutedEventArgs e)
        {
            if (PravoZmeny)
            {
                bool meno = false; bool typ = false; bool email; bool telefon = false; bool poznamka = false;
                if (txtDU_Meno.Text != "" && _aktUzivatel.Meno != txtDU_Meno.Text) { _aktUzivatel.Meno = txtDU_Meno.Text; meno = true; }
                //https://stackoverflow.com/questions/906899/binding-an-enum-to-a-winforms-combo-box-and-then-setting-it
                if (cbDU_Typ.Text != "" && cbDU_Typ.Text != nTyp.ToString())
                {
                    if (cbDU_Typ.SelectedValue.ToString() != FTyp.VSETKO.ToString() || cbDU_Typ.SelectedValue.ToString() != FTyp.Administrátor.ToString())
                    {
                        Enum.TryParse<FTyp>(cbDU_Typ.SelectedValue.ToString(), out nTyp);
                        _aktUzivatel.Typ = nTyp;
                        typ = true;
                    }
                    else
                    {
                        if (cbDU_Typ.SelectedValue.ToString() == FTyp.Administrátor.ToString() && ((MainWindow)Owner).PrihlasenyStav &&
                        FTyp.Administrátor.ToString() != ((MainWindow)Owner).Logika.GetPouzivatel(((MainWindow)Owner).PrihlasenyMeno).Typ.ToString())
                        {
                            Enum.TryParse<FTyp>(cbDU_Typ.SelectedValue.ToString(), out nTyp);
                            _aktUzivatel.Typ = nTyp;
                            typ = true;
                        }
                        else
                        {
                            MessageBox.Show("Typ môže zvloiť len prihlásený administrátor!");
                        }
                        if (cbDU_Typ.SelectedValue.ToString() == FTyp.VSETKO.ToString())
                        {
                            MessageBox.Show("Nepovolený typ!");
                        }
                    }
                }
                if (txtDU_Email.Text != _nEmail && txtDU_Email.Text != "") { _aktUzivatel.Email = txtDU_Email.Text; email = true; } else { email = false; }
                if (txtDU_Telefon.Text != "" && _aktUzivatel.Telefon != txtDU_Telefon.Text) { _aktUzivatel.Telefon = txtDU_Telefon.Text; telefon = true; }
                TextRange textRange = new TextRange(txtDU_Poznamka.Document.ContentStart, txtDU_Poznamka.Document.ContentEnd);
                if (textRange.Text != "" && _aktUzivatel.Poznamka != textRange.Text) { _aktUzivatel.Poznamka = textRange.Text; poznamka = true; }
                if (meno || typ || email || telefon || poznamka)
                {
                    if (!txtDU_Email.Text.Contains("@")) { MessageBox.Show("Zlý formát EMAIL-u."); }
                    BolaZmena = true;
                }
                this.Close();
            }
        }

    }
}
