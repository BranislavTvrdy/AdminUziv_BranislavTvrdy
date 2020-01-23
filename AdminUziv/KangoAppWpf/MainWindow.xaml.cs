using AdminUziv;
using ServiceApp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using Entity;
using Microsoft.Win32;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Inicializacia hlavného okna
        /// </summary>
        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            WcfClient = new ChannelFactory<IWcfKangoService>("KangoClient");
            WcfProxy = WcfClient.CreateChannel();
            Logika = new Engine();
            if (Logika.GetVstup())
            {
                Logika.LoadDataXml();
            }
            else
            {
                try
                {
                    Logika.LoadDataFromDb(WcfProxy.LoadPouzivatelia(), WcfProxy.LoadSkupiny());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    MessageBox.Show("Neboli načítané dáta z DB! (chyba na strane Wcf Hosta)", "Upozornenie",
                        MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            PrihlasenyStav = false;
            PrihlasenyMeno = "";
            DelPravo = false;
            PotrebaUlozit = false;
            MenuLoginOut.IsEnabled = false;
            MenuNewGroup.IsEnabled = false;
            MenuItemConfig.Visibility = Visibility.Hidden;
            MenuLoginIn_Click(this, null);
        }

        /// <summary>
        /// Stav prihlásenia používateľa
        /// </summary>
        public bool PrihlasenyStav { get; set; }
        /// <summary>
        /// Meno prihláseného používateľa
        /// </summary>
        public string PrihlasenyMeno { get; set; }
        /// <summary>
        /// Identifikátor práva pre mazanie
        /// </summary>
        public bool DelPravo { get; set; }
        /// <summary>
        /// Indikátor potreby uložiť obsah aplikácie
        /// </summary>
        public bool PotrebaUlozit { get; set; }

        /// <summary>
        /// Konfiguračné nastavenia aplikácie
        /// </summary>
        public NameValueCollection Nastavenia = ConfigurationManager.AppSettings;

        /// <summary>
        /// Objekt Wcf proxy pripojenia
        /// </summary>
        public IWcfKangoService WcfProxy { get; set; }

        /// <summary>
        /// Objekt Wcf klienta
        /// </summary>
        public ChannelFactory<IWcfKangoService> WcfClient { get; set; }
        
        /// <summary>
        /// Objekt triedy zaobstaravajúcej logiku
        /// </summary>
        public  Engine Logika { get; set; }

        /// <summary>
        /// Vypis vsetkeho obsahu
        /// </summary>
        public void Vypis()
        {
            VypisPouzivatelia();
            VypisSkupiny();
        }

        /// <summary>
        /// Vypis skupin
        /// </summary>
        private void VypisSkupiny()
        {
            DataGridGroups.ItemsSource = null;
            DataGridGroups.ItemsSource = Logika.GetSkupiny();
        }

        /// <summary>
        /// Vypis pouzivatelov
        /// </summary>
        private void VypisPouzivatelia()
        {
            DataGridUsers.ItemsSource = null;
            DataGridUsers.ItemsSource = Logika.GetPouzivatalia();
        }

        /// <summary>
        /// Zatváranie okna aplikácie
        /// </summary>
        /// <param name="paSender"></param>
        /// <param name="paE"></param>
        private void MenuItemExit_OnClick(object paSender, RoutedEventArgs paE)
        {
            MessageBoxResult result = MessageBox.Show("Prajete si zatvoriť aplikáciu ?", "Zavieranie aplikácie",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Handler pre kliknutie vypisu v Main okne
        /// </summary>
        private void MenuItem_Click_Vypis(object paSender, RoutedEventArgs paE)
        {
            Vypis();
        }

        /// <summary>
        /// Vytvaranie novych skupin
        /// </summary>
        private void MenuItem_Click_NovaSkupina(object paSender, RoutedEventArgs paE)
        {
            AddGroup pridavanie = new AddGroup {WindowStartupLocation = WindowStartupLocation.CenterOwner};
            pridavanie.ShowDialog();
            if (PrihlasenyStav && pridavanie.PodariloSa && !Logika.ContainsSkupina(pridavanie.GetMeno()))
            {
                Skupina nova = new Skupina(pridavanie.GetMeno(), pridavanie.GetTyp(), pridavanie.GetPoznamka(), PrihlasenyMeno);
                Logika.VytvorNovuSkupinu(nova);

                // Email notifikacia
                Pouzivatel konkretny = Logika.GetPouzivatel(PrihlasenyMeno);
                EmailClient notifikacia = new EmailClient(konkretny.Email, 
                    "NOTIFIKACIA KANGO", "<b> Práve ste vytvorili skupinu. <br></b>" +
                                "Údaje skupiny sú:<br>" +
                                "Meno: <b>" + nova.Meno + "</b><br>" +
                                "Typ: <b>" + nova.Typ.ToString() + "</b><br>" +
                                "Poznámka: <b>" + nova.Poznamka + "</b><br>", Nastavenia);
                notifikacia.PoslatEmail();
                MessageBox.Show("Vytvorená nová skupina!", "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
                PotrebaUlozit = true;
                VypisSkupiny();
            }
            pridavanie.Close();
        }

        /// <summary>
        /// Vytvaranie profilov novych pouzivatelov
        /// </summary>
        private void MenuItem_Click_NovyPouzivatel(object paSender, RoutedEventArgs paE)
        {
            AddUser pridavanie = new AddUser {WindowStartupLocation = WindowStartupLocation.CenterOwner};
            pridavanie.ShowDialog();
            if (pridavanie.PodariloSa && !Logika.ContainsPouzivatel(pridavanie.GetMeno()))
            {
                string sol = Logika.GenerujSol();
                string hesloSolene = Logika.GenerujPosolenuHashku(pridavanie.GetHeslo(), sol);
                Pouzivatel novy = new Pouzivatel(pridavanie.GetMeno(), hesloSolene, pridavanie.GetTyp(),
                    pridavanie.GetEmail(), pridavanie.GetTelefon(), pridavanie.GetPoznamka(), false) {Sol = sol};
                Logika.VytvorNovehoPouzivatela(novy);
                // Email notifikacia
                EmailClient notifikacia = new EmailClient(novy.Email, 
                    "NOTIFIKACIA KANGO", "<b> Práve ste sa registrovali do systému Kango. " +
                               "<br> Vitajte " + novy.Meno + "! </b> <br> Vaše osobné údaje sú:<br>" +
                               "Email: <b>" + novy.Email + "</b><br>" +
                               "Telefón: <b>" + novy.Telefon + "</b><br>" +
                               "Poznámka: <b>" + novy.Poznamka + "</b><br>", Nastavenia);
                notifikacia.PoslatEmail();
                MessageBox.Show("Nový používateľ vytvorený!", "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
                VypisPouzivatelia();
                PotrebaUlozit = true;

            }
            pridavanie.Close();
        }

        /// <summary>
        /// Vyhladavanie a filtrovanie podla zadanych parametrov
        /// </summary>
        private void MenuItem_Click_Vyhladavanie(object paSender, RoutedEventArgs paE)
        {
            Search vyhladavanie = new Search {WindowStartupLocation = WindowStartupLocation.CenterOwner};
            vyhladavanie.ShowDialog();
            if (vyhladavanie.Hladaj)
            {
                if (vyhladavanie.GetTypHladania())
                {
                    HashSet<Pouzivatel> vysledokUzivatelia = Logika.HladajPouzivatel(vyhladavanie.GetMeno(), vyhladavanie.GetTyp(), vyhladavanie.GetEmail(), vyhladavanie.GetTelefon(), vyhladavanie.GetAktivny());
                    DataGridUsers.ItemsSource = vysledokUzivatelia;
                }
                else
                {
                    HashSet<Skupina> vysledokSkupiny = Logika.HladajSkupina(vyhladavanie.GetMeno(), vyhladavanie.GetTyp(), vyhladavanie.GetVeduci());
                    DataGridGroups.ItemsSource = vysledokSkupiny;
                }
            }
            vyhladavanie.Close();
        }

        /// <summary>
        /// Spravovanie prihlasovacieho okna a vyhodnotenie stavu prihlasenia
        /// </summary>
        private void MenuLoginIn_Click(object sender, RoutedEventArgs paE)
        {
            try
            {
                Login prihlasit = new Login();
                prihlasit.SetData(Logika);
                prihlasit.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                prihlasit.ShowDialog();
                PrihlasenyStav = prihlasit.GetPrihlasStatus();
                if (PrihlasenyStav)
                {
                    PrihlasenyMeno = prihlasit.GetPrihlasMeno();
                    this.Title = "PRIHLÁSENÝ - Vitajte " + PrihlasenyMeno;
                    MenuLoginIn.IsEnabled = false;
                    MenuLoginOut.IsEnabled = true;
                    MenuNewGroup.IsEnabled = true;
                    if (Logika.GetPouzivatel(PrihlasenyMeno).Typ == FTyp.Administrátor)
                    {
                        MenuItemConfig.IsEnabled = true;
                        MenuItemConfig.Visibility = Visibility.Visible;
                    }
                    MenuItem_Click_Vypis(this, null);
                }
                prihlasit.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                MessageBox.Show("Zlyhalo prihlásenie! Pokračujete v neprihlásenom stave.", "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }

        /// <summary>
        /// Odhlasenie z konta
        /// </summary>
        private void MenuLoginOut_Click(object sender, RoutedEventArgs e)
        {
            if (DelPravo)
            {
                DelPravo = false;
            }
            PrihlasenyStav = false;
            PrihlasenyMeno = "";
            MenuLoginIn.IsEnabled = true;
            MenuLoginOut.IsEnabled = false;
            MenuNewGroup.IsEnabled = false;
            MenuItemConfig.IsEnabled = false;
            MenuItemConfig.Visibility = Visibility.Hidden;
            this.Title = "Administratíva systému KANGO";
            MessageBox.Show("Ste odhlásený!", "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Obnova a generovanie noveho hesla
        /// </summary>
        private void MenuRenewPass_OnClick(object paSender, RoutedEventArgs paE)
        {
            RenewPass obnova = new RenewPass(PrihlasenyStav) {WindowStartupLocation = WindowStartupLocation};
            obnova.ShowDialog();
            if (PrihlasenyStav && obnova.VstupOkna != "")
            {
                string heslo = obnova.VstupOkna;
                string sol = Logika.GenerujSol();
                Pouzivatel zabudlivec = Logika.GetPouzivatel(PrihlasenyMeno);
                zabudlivec.Heslo = Logika.GenerujPosolenuHashku(heslo, sol);
                zabudlivec.Sol = sol;
                //EMAIL -> HESLO
                EmailClient notifikacia = new EmailClient(zabudlivec.Email, 
                    "NOTIFIKACIA KANGO", "<b>Práve ste zmenili heslo do systému Kango.</b><br>" +
                             "Vaše osobné údaje sú:<br>" +
                             "Meno: <b>" + zabudlivec.Meno + "</b><br>" +
                             "Heslo: <b>" + heslo + "</b><br>" +
                             "Email: <b>" + zabudlivec.Email + "</b><br>" +
                             "Telefón: <b>" + zabudlivec.Telefon + "</b><br>", Nastavenia);
                notifikacia.PoslatEmail();
                MenuItem_Click_Save(this, paE);
            }
            if (!PrihlasenyStav && obnova.VstupOkna != "")
            {
                foreach (Pouzivatel polozka in Logika.GetPouzivatalia())
                {
                    if (polozka.Meno == obnova.VstupOkna)
                    {
                        string heslo = Logika.GenerujSol();
                        string sol = Logika.GenerujSol();
                        polozka.Heslo = Logika.GenerujPosolenuHashku(heslo, sol);
                        polozka.Sol = sol;
                        // EMAIL -> HESLO
                        EmailClient notifikacia = new EmailClient(polozka.Email, 
                            "NOTIFIKACIA KANGO", "<b>Práve ste zmenili heslo do systému Kango.</b><br>" +
                                      "Vaše osobné údaje sú:<br>" +
                                      "Meno: <b>" + polozka.Meno + "</b><br>" +
                                      "Heslo: <b>" + heslo + "</b><br>" +
                                      "Email: <b>" + polozka.Email + "</b><br>" +
                                      "Telefón: <b>" + polozka.Telefon + "</b><br>", Nastavenia);
                        notifikacia.PoslatEmail();
                        break;
                    }
                }
                MenuItem_Click_Save(this, paE);
            }
            obnova.Close();
        }

        /// <summary>
        /// Handler pre kliknutie ukladania v Main okne
        /// </summary>
        private void MenuItem_Click_Save(object paSender, RoutedEventArgs paE)
        {
            try
            {
                WcfProxy.SavePouzivatelia(Logika.GetPouzivatalia());
                WcfProxy.SaveSkupiny(Logika.GetSkupiny());
                PotrebaUlozit = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Zmeny neboli uložené! (možná chyba v spojení s databázou)", "Upozornenie", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Zobrazenie detailov o skupine
        /// </summary>
        private void BtnDetailGroup_OnClick(object paSender, RoutedEventArgs paE)
        {
            try
            {
                Skupina dataRowView = (Skupina)((Button)paE.Source).DataContext;
                string tMeno = dataRowView.Meno;
                FTyp tTyp = dataRowView.Typ;
                string tVeduci = dataRowView.VeduciSkupiny;
                // EDIT
                Skupina tSkupina = Logika.GetSkupina(tMeno);
                HashSet<Skupina> tPodskupiny = Logika.GetPodskupiny(tMeno);
                List<Pouzivatel> tPodUzivatelia = Logika.GetPodPouzivatelov(tMeno);
                bool povolenie = false;
                if (PrihlasenyStav)
                {
                    povolenie = (PrihlasenyMeno == tVeduci || Logika.GetPouzivatel(PrihlasenyMeno).Typ == FTyp.Administrátor) ? true : false;
                }
                DetailGroup editovanie =
                    new DetailGroup(povolenie, tSkupina.Meno, tSkupina, tPodUzivatelia, tPodskupiny, this)
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                editovanie.ShowDialog();
                if (editovanie.BoloPridanie)
                {
                    PotrebaUlozit = true;
                }
                if (PrihlasenyStav && editovanie.BolaZmena)
                {
                    dataRowView.Meno = tSkupina.Meno;
                    dataRowView.Typ = tSkupina.Typ;
                    dataRowView.VeduciSkupiny = tSkupina.VeduciSkupiny;
                    dataRowView.Poznamka = tSkupina.Poznamka;
                    // Email notifikacia
                    Pouzivatel konkretny = Logika.GetPouzivatel(tSkupina.VeduciSkupiny);
                    EmailClient notifikacia = new EmailClient(konkretny.Email, "NOTIFIKACIA KANGO", "<b> Práve ste editovali údaje skupiny. <br></b>" +
                        "Údaje skupiny sú:<br>" +
                        "Meno: <b>" + tSkupina.Meno + "</b><br>" +
                        "Typ: <b>" + tSkupina.Typ.ToString() + "</b><br>" +
                        "Poznámka: <b>" + tSkupina.Poznamka + "</b><br>" +
                        "Podskupiny: <b>" + tSkupina.GetPodskupiny() + "</b><br>" +
                        "Členovia: <b>" + tSkupina.GetClenov() + "</b><br>", Nastavenia);
                    notifikacia.PoslatEmail();
                    if (tSkupina.Meno != tMeno)
                    {
                        foreach (Pouzivatel polozka in Logika.GetPouzivatalia())
                        {
                            if (polozka.Skupiny.Contains(tMeno))
                            {
                                polozka.Skupiny.Remove(tMeno);
                                polozka.Skupiny.Add(tSkupina.Meno);
                            }
                        }
                    }
                    MessageBox.Show("Editovali ste skupinu : " + tMeno, "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
                    PotrebaUlozit = true;
                    VypisSkupiny();
                }
                editovanie.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Zobrazenie detailov o používateľovi
        /// </summary>
        private void BtnDetailUser_OnClick(object paSender, RoutedEventArgs paE)
        {
            try
            {
                Pouzivatel dataRowView = (Pouzivatel)((Button)paE.Source).DataContext;
                string tMeno = dataRowView.Meno;
                Pouzivatel konkretny = Logika.GetPouzivatel(tMeno);
                HashSet<Skupina> zaclenenie = null;
                if (konkretny.Skupiny.Count != 0)
                {
                    zaclenenie = Logika.GetZaclenenie(tMeno);
                }
                bool povolenie = false;
                if (PrihlasenyStav)
                {
                    povolenie = (PrihlasenyMeno == tMeno || Logika.GetPouzivatel(PrihlasenyMeno).Typ == FTyp.Administrátor) ? true : false;
                }
                DetailUser editovanie = new DetailUser(povolenie, konkretny, zaclenenie)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                editovanie.ShowDialog();
                if (PrihlasenyStav && editovanie.BolaZmena)
                {
                    dataRowView.Meno = konkretny.Meno;
                    dataRowView.Typ = konkretny.Typ;
                    dataRowView.Email = konkretny.Email;
                    dataRowView.Telefon = konkretny.Telefon;
                    dataRowView.Aktivny= konkretny.Aktivny;
                    // Email notifikacia
                    EmailClient notifikacia = new EmailClient(konkretny.Email, "NOTIFIKACIA KANGO", "<b> Práve ste editovali osobné údaje. <br></b>" +
                        "Vaše osobné údaje sú:<br>" +
                        "Meno: <b>" + konkretny.Meno + "</b><br>" +
                        "Email: <b>" + konkretny.Email + "</b><br>" +
                        "Telefón: <b>" + konkretny.Telefon + "</b><br>" +
                        "Poznámka: <b>" + konkretny.Poznamka + "</b><br>", Nastavenia);
                    notifikacia.PoslatEmail();
                    if (konkretny.Meno != tMeno)
                    {
                        foreach (Skupina polozka in Logika.GetSkupiny())
                        {
                            if (konkretny.Skupiny.Contains(polozka.Meno))
                            {
                                polozka.Clenovia.Remove(tMeno);
                                polozka.Clenovia.Add(konkretny.Meno);
                            }
                            if (polozka.VeduciSkupiny == tMeno)
                            {
                                polozka.VeduciSkupiny = konkretny.Meno;
                            }
                        }
                    }
                    MessageBox.Show("Editovali ste používateľa : " + tMeno, "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
                    PotrebaUlozit = true;
                    VypisPouzivatelia();
                }
                editovanie.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Handler pre zatvorenie Main okna
        /// </summary>
        private void MainWindow_OnClosing(object paSender, CancelEventArgs paE)
        {
            if (PotrebaUlozit)
            {
                MessageBoxResult odpoved = MessageBox.Show("Prajete si uložiť zmeny?", "Ukončenie aplikácie",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (odpoved == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Uložené!", "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
                    MenuItem_Click_Save(this, null);
                }
                else
                {
                    if (odpoved == MessageBoxResult.No)
                    {
                        MessageBox.Show("Neuložené!", "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        paE.Cancel = true;
                    }
                }
            }
        }

        /// <summary>
        /// Exportovanie dát aplikácie do súborov XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = $"XML files| *.xml;|Text files|*.txt;*.csv;*.tsv;|All files|*.*",
                InitialDirectory = @"Desktop",
                FileName = "KangoExport"
            };
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                fileName = fileName.Substring(0, fileName.Length - 4);
                Logika.SaveData(fileName);
            }
        }

        /// <summary>
        /// Importovanie používateľov z XML súborov
        /// </summary>
        /// <param name="paSender"></param>
        /// <param name="paE"></param>
        private void MenuItemImportUsers_OnClick(object paSender, RoutedEventArgs paE)
        {
            Importer(false);
        }

        /// <summary>
        /// Importovanie skupín z XML súborov
        /// </summary>
        /// <param name="paSender"></param>
        /// <param name="paE"></param>
        private void MenuItemImportGroups_OnClick(object paSender, RoutedEventArgs paE)
        {
            Importer(true);
        }

        /// <summary>
        /// Importovanie vstupných dát z XML súborov
        /// </summary>
        /// <param name="paTyp">Druh importovaných dát</param>
        private void Importer(bool paTyp)
        {
            var dialog = new OpenFileDialog
            {
                Filter = $"XML files| *.xml;|Text files|*.txt;*.csv;*.tsv;|All files|*.*",
                InitialDirectory = @"Desktop"
            };
            if (dialog.ShowDialog() != true) return;
            var fileName = dialog.FileName;
            Logika.LoadDataXmlImport(fileName, paTyp);
        }

        /// <summary>
        /// Zmena nastavenia konfigurácie
        /// </summary>
        /// <param name="paSender"></param>
        /// <param name="paE"></param>
        private void MenuItemConfig_OnClick(object paSender, RoutedEventArgs paE)
        {
            try
            {

                Config konfiguracia = new Config(Logika.GetVstup())
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                konfiguracia.ShowDialog();
                if (PrihlasenyStav && konfiguracia.NastalaZmena)
                {
                    Logika._nastavenia.Set("vstupSystemu", konfiguracia.VstupSystemu.ToString());

                    System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["vstupSystemu"].Value = konfiguracia.VstupSystemu.ToString();
                    config.Save(ConfigurationSaveMode.Modified);
                }
                konfiguracia.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
