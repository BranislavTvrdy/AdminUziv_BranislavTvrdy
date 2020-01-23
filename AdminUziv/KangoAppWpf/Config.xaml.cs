using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KangoAppWpf
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Window
    {

        /// <summary>
        /// Indikuje vykonanie zmeny v konfiguračných nastaveniach
        /// </summary>
        public bool NastalaZmena { get; set; }
        /// <summary>
        /// Indikuje vstup systému
        /// True ak sa používa lokálny vstup, false ak sa používa vstup z DB
        /// </summary>
        public bool VstupSystemu { get; set; }
        /// <summary>
        /// Konštruktor triedy Config
        /// </summary>
        /// <param name="paNastavenia">True ak sa používa lokálny vstup, false ak sa používa vstup z DB</param>
        public Config(bool paNastavenia)
        {
            InitializeComponent();
            if (paNastavenia)
            {
                RadioButtonLocal.IsChecked = true;
                RadioButtonDb.IsChecked = false;
            }
            else
            {
                RadioButtonLocal.IsChecked = false;
                RadioButtonDb.IsChecked = true;
            }

            NastalaZmena = false;
        }

        /// <summary>
        /// Uloženie zmeny konfiguračných nastavení
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NastalaZmena = true;
            if (RadioButtonLocal.IsChecked != null && (bool) RadioButtonLocal.IsChecked)
            {
                VstupSystemu = true;
            }
            this.Close();
        }
    }
}
