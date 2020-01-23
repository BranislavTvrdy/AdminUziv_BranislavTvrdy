using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using AdminUziv;
using Entity;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for DetailGroup.xaml
    /// </summary>
    public partial class DetailGroup : Window
    {
        /// <summary>
        /// Meno editovanej skupiny
        /// </summary>
        private readonly string _nMeno;
        /// <summary>
        /// Typ editovanej skupiny
        /// </summary>
        private FTyp _nTyp = 0x0000000;
        /// <summary>
        /// Clenovia editovanej skupiny
        /// </summary>
        private readonly List<Pouzivatel> _clenovia;
        /// <summary>
        /// Podskupiny editovanej skupiny
        /// </summary>
        private readonly HashSet<Skupina> _podskupiny;

        /// <summary>
        /// Matersky objekt
        /// </summary>
        public new object Owner { get; set; }
        /// <summary>
        /// Indikuje pravo na vykonavanie zmien editacie
        /// </summary>
        public bool PravoZmeny;
        /// <summary>
        /// Aktualne zobrazovana skupina
        /// </summary>
        readonly Skupina _aktSkupina;
        /// <summary>
        /// Indikuje vykonanie editácie údajov o skupine
        /// </summary>
        public bool BolaZmena { get; set; }
        /// <summary>
        /// Indikuje pridanie podskupiny alebo člena do skupiny
        /// </summary>
        public bool BoloPridanie { get; set; }
        /// <summary>
        /// Indikuje aktivovany stlpec mazania
        /// </summary>
        private bool _delStlpec;
        /// <summary>
        /// Indikuje druh zobrazovanych udajov v komponente DataGrid
        /// </summary>
        private bool DgSwitch { get; set; }

        /// <summary>
        /// Inicializacia okna pre zobrazenie detailov skupiny
        /// </summary>
        /// <param name="paPravaUpravy">Prava pre upravu</param>
        /// <param name="paMenoOrig">Aktualne meno skupiny</param>
        /// <param name="paSkupina">Objekt aktualnej skupiny</param>
        /// <param name="paClenovia">List clenov skupiny</param>
        /// <param name="paPodskupiny">HashSet podskupin skupiny</param>
        public DetailGroup(bool paPravaUpravy, string paMenoOrig, Skupina paSkupina, List<Pouzivatel> paClenovia, HashSet<Skupina> paPodskupiny, object paSender)
        {
            InitializeComponent();
            PravoZmeny = paPravaUpravy;
            BolaZmena = false;
            BoloPridanie = false;
            _nMeno = paMenoOrig;
            _nTyp = paSkupina.Typ;
            cbDG_Typ.ItemsSource = System.Enum.GetValues(typeof(FTyp));
            txtDG_Veduci.Text = paSkupina.VeduciSkupiny;
            txtDG_Meno.Text = paMenoOrig;
            _aktSkupina = paSkupina;
            cbDG_Typ.SelectedItem = _nTyp;
            _delStlpec = false;


            new TextRange(txtDG_Poznamka.Document.ContentStart, txtDG_Poznamka.Document.ContentEnd)
            {
                Text = paSkupina.Poznamka
            };

            _clenovia = paClenovia;
            _podskupiny = paPodskupiny;

            if (!paPravaUpravy)
            {
                btn_EditujSkupinu.IsEnabled = false;
                btn_PridajPod.IsEnabled = false;
            }

            Owner = paSender;
            Button_Click_Podskupiny(this, null);
            DelPravo();
            DgSwitch = true; // T skupiny F uzivatelia

        }

        /// <summary>
        /// Vykonanie editacie udajov skupiny
        /// </summary>
        private void BtnEditujSkupinuClick(object sender, RoutedEventArgs e)
        {
            if (PravoZmeny)
            {
                bool veduci = false; bool meno = false; bool typ = false; bool poznamka = false;
                if (txtDG_Veduci.Text != "" && _aktSkupina.VeduciSkupiny != txtDG_Veduci.Text) { _aktSkupina.VeduciSkupiny = txtDG_Veduci.Text; veduci = true; }
                if (txtDG_Meno.Text != "" && _aktSkupina.Meno != txtDG_Meno.Text) { _aktSkupina.Meno = txtDG_Meno.Text; meno = true; }
                if (cbDG_Typ.Text != "" && cbDG_Typ.Text != _nTyp.ToString())
                {
                    if (cbDG_Typ.SelectedValue.ToString() != FTyp.VSETKO.ToString() || cbDG_Typ.SelectedValue.ToString() != FTyp.Administrátor.ToString())
                    {
                        Enum.TryParse<FTyp>(cbDG_Typ.SelectedValue.ToString(), out _nTyp);
                        _aktSkupina.Typ = _nTyp;
                        typ = true;
                    }
                    else
                    {
                        if (cbDG_Typ.SelectedValue.ToString() == FTyp.Administrátor.ToString() && ((MainWindow)Owner).PrihlasenyStav &&
                        FTyp.Administrátor.ToString() != ((MainWindow)Owner).Logika.GetPouzivatel(((MainWindow)Owner).PrihlasenyMeno).Typ.ToString())
                        {
                            Enum.TryParse<FTyp>(cbDG_Typ.SelectedValue.ToString(), out _nTyp);
                            _aktSkupina.Typ = _nTyp;
                            typ = true;
                        }
                        else
                        {
                            MessageBox.Show("Typ môže zvloiť len prihlásený administrátor!");
                        }
                        if (cbDG_Typ.SelectedValue.ToString() == FTyp.VSETKO.ToString())
                        {
                            MessageBox.Show("Nepovolený typ!");
                        }
                    }
                }
                TextRange textRange = new TextRange(txtDG_Poznamka.Document.ContentStart, txtDG_Poznamka.Document.ContentEnd);
                if (textRange.Text != "" && _aktSkupina.Poznamka != textRange.Text) { _aktSkupina.Poznamka = textRange.Text; poznamka = true; }
                if (meno || typ || veduci || poznamka)
                {
                    BolaZmena = true;
                }
            }
            this.Close();
        }

        /// <summary>
        /// Pridanie noveho clena alebo novej podskupiny
        /// </summary>
        private void BtnPridajPodClick(object sender, RoutedEventArgs e)
        {
            if (PravoZmeny && cbDG_NovPod.SelectedValue != null)
            {

                string obsah = cbDG_NovPod.SelectedValue.ToString();
                if (DgSwitch)
                {
                    Skupina novaPod = ((MainWindow)Owner).Logika.GetSkupina(obsah);
                    if (novaPod != null)
                    {
                        _podskupiny.Add(novaPod);
                        _aktSkupina.Podskupiny.Add(novaPod.Meno);
                        //DataGridPod.Items.Add(novaPod);

                        NaplnPodskupin();
                        VypisPodskupin();

                        // Email notifikacia
                        Pouzivatel adresat = ((MainWindow)Owner).Logika.GetPouzivatel(_aktSkupina.VeduciSkupiny);
                        EmailClient notifikacia = new EmailClient(adresat.Email, "NOTIFIKACIA KANGO", "<b> Práve ste pridali novú podskupinu. </b><br>" +
                            "Meno podskupiny: <b>" + novaPod.Meno + "</b><br>" +
                            "Typ: <b>" + novaPod.Typ.ToString() + "</b><br>", ((MainWindow)Owner).Nastavenia);
                        notifikacia.PoslatEmail();

                    }
                }
                if (!DgSwitch)
                {
                    Pouzivatel novyClen = ((MainWindow)Owner).Logika.GetPouzivatel(obsah);
                    if (novyClen != null)
                    {
                        novyClen.Skupiny.Add(_nMeno);
                        _clenovia.Add(novyClen);
                        _aktSkupina.Clenovia.Add(novyClen.Meno);
                        //DataGridPod.Items.Add(novyClen);

                        NaplnClenov();
                        VypisClenov();

                        // Email notifikacia
                        Pouzivatel adresat = ((MainWindow)Owner).Logika.GetPouzivatel(_aktSkupina.VeduciSkupiny);
                        EmailClient notifikacia = new EmailClient(novyClen.Email, "NOTIFIKACIA KANGO", "<b> Práve ste boli zaradený do novej skupiny.</b><br>" +
                            "Meno skupiny: <b>" + _aktSkupina.Meno + "</b><br>" +
                            "Typ: <b>" + _aktSkupina.Typ.ToString() + "</b><br>", ((MainWindow)Owner).Nastavenia, adresat);
                        notifikacia.PoslatEmail();
                    }
                }
                BoloPridanie = true;
            }
        }

        /// <summary>
        /// Zobrazenie podskupin skupiny
        /// </summary>
        private void Button_Click_Podskupiny(object paSender, RoutedEventArgs paE)
        {
            _delStlpec = false;
            l_PridajPod.Content = "Meno novej podskupiny:";
            VypisPodskupin();
            NaplnPodskupin();
            DelPravo();
            ButtonPodskupiny.IsEnabled = false;
            ButtonClenovia.IsEnabled = true;
            DgSwitch = true;
        }

        /// <summary>
        /// Zobrazenie clenov skupiny
        /// </summary>
        private void Button_Click_Clenovia(object paSender, RoutedEventArgs paE)
        {
            _delStlpec = false;
            l_PridajPod.Content = "Meno nového člena:";
            VypisClenov();
            NaplnClenov();
            DelPravo();
            ButtonPodskupiny.IsEnabled = true;
            ButtonClenovia.IsEnabled = false;
            DgSwitch = false;
        }

        /// <summary>
        /// Vypis clenov skupiny
        /// </summary>
        public void VypisClenov()
        {
            DataGridPod.ItemsSource = null;

            DataGridPod.Columns.Clear();
            DataGridTextColumn dgtcMeno = new DataGridTextColumn {Header = "Meno", Binding = new Binding("Meno")};
            DataGridPod.Columns.Add(dgtcMeno);

            DataGridTextColumn dgtcTyp = new DataGridTextColumn {Header = "Typ", Binding = new Binding("Typ")};
            DataGridPod.Columns.Add(dgtcTyp);

            DataGridTextColumn dgtcEmail = new DataGridTextColumn {Header = "Email", Binding = new Binding("Email")};
            DataGridPod.Columns.Add(dgtcEmail);

            DataGridTextColumn dgtcTelefon = new DataGridTextColumn
            {
                Header = "Telefon", Binding = new Binding("Telefon")
            };
            DataGridPod.Columns.Add(dgtcTelefon);

            DataGridTextColumn dgtcAktivny = new DataGridTextColumn
            {
                Header = "Aktivny", Binding = new Binding("Aktivny")
            };
            DataGridPod.Columns.Add(dgtcAktivny);

            DataGridPod.ItemsSource = _clenovia;
        }

        /// <summary>
        /// Vypis podskupin skupiny
        /// </summary>
        public void VypisPodskupin()
        {
            DataGridPod.ItemsSource = null;

            DataGridPod.Columns.Clear();
            DataGridTextColumn dgtcMeno = new DataGridTextColumn {Header = "Meno", Binding = new Binding("Meno")};
            DataGridPod.Columns.Add(dgtcMeno);

            DataGridTextColumn dgtcTyp = new DataGridTextColumn {Header = "Typ", Binding = new Binding("Typ")};
            DataGridPod.Columns.Add(dgtcTyp);

            DataGridTextColumn dgtcVeduciSkupiny = new DataGridTextColumn
            {
                Header = "VeduciSkupiny", Binding = new Binding("VeduciSkupiny")
            };
            DataGridPod.Columns.Add(dgtcVeduciSkupiny);

            DataGridPod.ItemsSource = _podskupiny;
        }

        /// <summary>
        /// Naplnenie Comboboxu pre pridavanie podskupin
        /// </summary>
        public void NaplnPodskupin()
        {
            if (PravoZmeny)
            {
                cbDG_NovPod.Items.Clear();
                var kontrolaCb = ((MainWindow)Owner).Logika.GetSkupiny();
                foreach (Skupina polozka in kontrolaCb)
                {
                    if (!_podskupiny.Contains(polozka) && polozka.Meno != _aktSkupina.Meno)
                    {
                        cbDG_NovPod.Items.Add(polozka);
                    }

                }
            }
        }

        /// <summary>
        /// Naplnenie Comboboxu pre pridavanie clenov
        /// </summary>
        public void NaplnClenov()
        {
            if (PravoZmeny)
            {
                cbDG_NovPod.Items.Clear();
                HashSet<Pouzivatel> kontrolaCB = ((MainWindow)Owner).Logika.GetPouzivatalia();
                foreach (Pouzivatel polozka in kontrolaCB)
                {
                    if (!_clenovia.Contains(polozka) && polozka.Meno != _aktSkupina.VeduciSkupiny)
                    {
                        cbDG_NovPod.Items.Add(polozka);
                    }

                }
            }
        }

        /// <summary>
        /// Uprava vypisu pre moznost odstranovania poloziek
        /// </summary>
        public void DelPravo()
        {
            if (!_delStlpec && ((MainWindow)Owner).PrihlasenyStav)
            {
                Pouzivatel kontrolovany = ((MainWindow)Owner).Logika.GetPouzivatel(((MainWindow)Owner).PrihlasenyMeno);
                if (FTyp.Administrátor.ToString() == kontrolovany.Typ.ToString() || kontrolovany.Meno == _aktSkupina.VeduciSkupiny)
                {
                    var col = new DataGridTemplateColumn
                    {
                        Header = DgSwitch ? "Odstrániť podskupinu:" : "Odstrániť člena:"
                    };
                    var template = new DataTemplate();
                    col.CellTemplate = template;
                    var buttonFactory = new FrameworkElementFactory(typeof(Button));
                    buttonFactory.SetValue(Button.ContentProperty, "Odstrániť");
                    //buttonFactory.Text = "Odstrániť";
                    buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(BtnDelete_Click));
                    template.VisualTree = buttonFactory;
                    col.CellEditingTemplate = template;
                    DataGridPod.Columns.Add(col);
                }
                _delStlpec = true;
            }
        }

        /// <summary>
        /// Odstranenie clena alebo podskupiny zo skupiny
        /// </summary>
        private void BtnDelete_Click(object paSender, RoutedEventArgs paE)
        {
            
            try
            {
                if (DgSwitch)
                {

                    if (PravoZmeny)
                    {
                        Skupina dataRowView = (Skupina)((Button)paE.Source).DataContext;

                        string tMenoOdchod = dataRowView.Meno;
                        foreach (Skupina polozka in _podskupiny)
                        {
                            if (polozka.Meno == tMenoOdchod)
                            {
                                _podskupiny.Remove(polozka);
                                _aktSkupina.Podskupiny.Remove(polozka.Meno);

                                // Email notifikacia
                                Pouzivatel odosielatel = ((MainWindow)Owner).Logika.GetPouzivatel(_aktSkupina.VeduciSkupiny);
                                Pouzivatel adresat = ((MainWindow)Owner).Logika.GetPouzivatel(polozka.VeduciSkupiny);
                                EmailClient notifikacia = new EmailClient(adresat.Email, "NOTIFIKACIA KANGO", "<b> Vaša skupina bola práve odstránená zo skupiny.</b><br>" +
                                                                                                              "Meno skupiny: <b>" + _aktSkupina.Meno + "</b><br>" +
                                                                                                              "Typ: <b>" + _aktSkupina.Typ.ToString() + "</b><br>", ((MainWindow)Owner).Nastavenia, odosielatel);
                                notifikacia.PoslatEmail();

                                break;
                            }
                        }
                        VypisPodskupin();
                        NaplnPodskupin();
                        DelPravo();
                        MessageBox.Show("Zo skupiny odišla podskupina: " + tMenoOdchod);
                        BoloPridanie = true;
                    }
                    else
                    {
                        MessageBox.Show("Nemáte právo editácie.");
                    }

                }
                else
                {
                    if (PravoZmeny)
                    {
                        Pouzivatel dataRowView = (Pouzivatel)((Button)paE.Source).DataContext;
                        string tMenoOdchod = dataRowView.Meno;
                        foreach (Pouzivatel polozka in _clenovia)
                        {
                            if (polozka.Meno == tMenoOdchod)
                            {
                                _clenovia.Remove(polozka);
                                _aktSkupina.Clenovia.Remove(polozka.Meno);
                                Pouzivatel uzivatelOdchod = ((MainWindow)Owner).Logika.GetPouzivatel(polozka.Meno);
                                uzivatelOdchod.Skupiny.Remove(_nMeno);

                                // Email notifikacia
                                Pouzivatel adresat = ((MainWindow)Owner).Logika.GetPouzivatel(_aktSkupina.VeduciSkupiny);
                                EmailClient notifikacia = new EmailClient(polozka.Email, "NOTIFIKACIA KANGO", "<b> Práve ste boli odstránený zo skupiny.</b><br>" +
                                                                                                              "Meno skupiny: <b>" + _aktSkupina.Meno + "</b><br>" +
                                                                                                              "Typ: <b>" + _aktSkupina.Typ.ToString() + "</b><br>", ((MainWindow)Owner).Nastavenia, adresat);
                                notifikacia.PoslatEmail();
                                break;
                            }
                        }
                        VypisClenov();
                        NaplnClenov();
                        DelPravo();
                        MessageBox.Show("Zo skupiny odišiel člen: " + tMenoOdchod);
                        BoloPridanie = true;
                    }
                    else
                    {
                        MessageBox.Show("Nemáte právo editácie.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }


    }
}
