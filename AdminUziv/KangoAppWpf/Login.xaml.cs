using System;
using System.Windows;
using AdminUziv;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private Engine _data;
        private bool _podariloSa;
        private string _meno;

        /// <summary>
        /// Seter pre atribút _data
        /// </summary>
        /// <param name="paData">logika aplikácie</param>
        public void SetData(Engine paData)
        {
            this._data = paData;
        }
        /// <summary>
        /// Getter pre vysledok prihlasovania
        /// </summary>
        /// <returns>vráti true ak sa podarilo prihlásiť, false inak</returns>
        public bool GetPrihlasStatus()
        {
            return _podariloSa;
        }
        /// <summary>
        /// Geter pre meno prihláseného
        /// </summary>
        /// <returns>Vráti meno prihláseného</returns>
        public string GetPrihlasMeno()
        {
            return _meno;
        }
        /// <summary>
        /// Konštruktor triedy Login
        /// </summary>
        public Login()
        {
            InitializeComponent();
            _podariloSa = false;
            _meno = "";
        }

        /// <summary>
        /// Handler stlačenia prihlasovacieho tlačidla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtLoginMeno.Text) && !String.IsNullOrWhiteSpace(txtLoginHeslo.Password))
            {
                if (!_data.Login(txtLoginMeno.Text, txtLoginHeslo.Password))
                {
                    MessageBox.Show("Pri prihlasovaní sa vyskytla chyba. \n " +
                                    "Skontrolujte si vstupné údaje.");
                    txtLoginMeno.Focus();
                }
                else
                {
                    _podariloSa = true;
                    _meno = txtLoginMeno.Text;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Zadajte vstup!");
                txtLoginMeno.Text = string.Empty;
                txtLoginHeslo.Password = string.Empty;
                txtLoginMeno.Focus();
            }
        }
    }
}
