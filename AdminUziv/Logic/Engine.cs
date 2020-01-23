using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using Entity;
using System.Security.Cryptography;
using Xml;

namespace AdminUziv
{
	public class Engine
    {
        /// <summary>
        /// Všetci používatelia
        /// </summary>
		private HashSet<Pouzivatel> Pouzivatelia { get; set; }
        /// <summary>
        /// Všetky skupiny
        /// </summary>
		private HashSet<Skupina> Skupiny { get; set; }
        /// <summary>
        /// Konfiguračné nastavenia
        /// </summary>
        public NameValueCollection _nastavenia = ConfigurationManager.AppSettings;
        /// <summary>
        /// Dĺžka reťazca pre generovanie soleného hesla
        /// </summary>
        private static readonly int MaxSaltLength = 32;


        /// <summary>
        /// Načítanie vstupu aplikácie z databázy
        /// </summary>
        /// <param name="paPouzivatelia">Hashset používateľov</param>
        /// <param name="paSkupina">Hashet skupín</param>
        public void LoadDataFromDb(HashSet<Pouzivatel> paPouzivatelia, HashSet<Skupina> paSkupina)
        {
            Pouzivatelia = paPouzivatelia;
            Skupiny = paSkupina;
            foreach (var uzivatel in Pouzivatelia)
            {
                string[] zaradenie = uzivatel.SkupinyDbo.Split(';');
                foreach (var item in zaradenie)
                {
                    uzivatel.Skupiny.Add(item);
                }
            }

            foreach (var skupina in Skupiny)
            {
				string[] zaradenie = skupina.PodskupinyDbo.Split(';');
                foreach (var item in zaradenie)
                {
                    skupina.Podskupiny.Add(item);
                }
                zaradenie = skupina.ClenoviaDbo.Split(';');
                foreach (var item in zaradenie)
                {
                    skupina.Clenovia.Add(item);
                }
			}
        }

        /// <summary>
        /// Načítanie vstupu aplikácie z XML
        /// </summary>
        public void LoadDataXml()
        {
			Pouzivatelia = new HashSet<Pouzivatel>();
			Skupiny = new HashSet<Skupina>();
            XmlReadWrite RW = new XmlReadWrite();
            //RW.LoadPouzivatelia("..\\..\\..\\DATA\\test_pouzivatelia.xml", t_Uziv);
			//RW.LoadSkupiny("..\\..\\..\\DATA\\test_skupiny.xml", t_Skupiny);
            RW.LoadPouzivatelia(_nastavenia.Get("cestaXML_Pouzivatelia"), Pouzivatelia);
			RW.LoadSkupiny(_nastavenia.Get("cestaXML_Skupiny"), Skupiny);
        }

        /// <summary>
        /// Načítanie vstupu aplikácie z jednotlivého XML
        /// </summary>
        /// <param name="paCesta">Cesta k súboru</param>
        /// <param name="paNacitanie">Typ načítania True -> načíta skupiny, False -> načíta používateľov</param>
        public void LoadDataXmlImport(string paCesta, bool paNacitanie)
        {
            XmlReadWrite RW = new XmlReadWrite();
			if (paNacitanie)
            {
                Skupiny = new HashSet<Skupina>();
                RW.LoadSkupiny(paCesta, Skupiny);
			}
            else
            {
				Pouzivatelia = new HashSet<Pouzivatel>();
                RW.LoadPouzivatelia(paCesta, Pouzivatelia);
			}
        }

        /// <summary>
        /// Ukladanie dát aplikácie do XML súborov
        /// </summary>
        /// <param name="paCesta">Cesta pre ukladanie dát</param>
        public void SaveData(string paCesta)
		{
            XmlReadWrite RW = new XmlReadWrite();
            RW.SavePouzivatelia(paCesta + "_Users.xml", Pouzivatelia);
            RW.SaveSkupiny(paCesta + "_Groups.xml", Skupiny);
        }

        /// <summary>
        /// Getter pre druh vstupneho bodu aplikácie
        /// </summary>
        /// <returns>Vráti true ak sa dáta načítajú z lokálneho zdroja a false ak sa načítajú z databázy.</returns>
        public bool GetVstup()
        {
            return Convert.ToBoolean(_nastavenia.Get("vstupSystemu"));
        }

        /// <summary>
        /// Vyhladanie pouzivatela podla mena
        /// </summary>
        /// <param name="paKluc">Unikatne meno pouzivatela</param>
        /// <returns>Vrati objekt pouzivatela ak taky existuje. Inak null.</returns>
		public Pouzivatel GetPouzivatel(string paKluc)
		{
            return Pouzivatelia.FirstOrDefault(u => u.Meno == paKluc);
        }

        /// <summary>
        /// Zistenie existencie pouzivatela podla mena
        /// </summary>
        /// <param name="paKluc">Unikatne meno pouzivatela</param>
        /// <returns>Vrati true ak pouzivatel existuje. Inak false.</returns>
		public bool ContainsPouzivatel(string paKluc)
        {
            try
            {
                Pouzivatel ret = Pouzivatelia.FirstOrDefault(u => u.Meno == paKluc);
                return ret != null;
			}
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return false;
            }
            
        }

        /// <summary>
        /// Zistenie existencie skupiny podla mena
        /// </summary>
        /// <param name="paKluc">Unikatne meno skupiny</param>
        /// <returns>Vrati true ak skupina existuje. Inak false.</returns>
		public bool ContainsSkupina(string paKluc)
		{
            var ret = Skupiny.FirstOrDefault(u => u.Meno == paKluc);
            return ret != null;
		}

        /// <summary>
        /// Geter používateľov
        /// </summary>
        /// <returns>Vráti zoznam používateľov</returns>
		public HashSet<Pouzivatel> GetPouzivatalia() { return Pouzivatelia; }

        /// <summary>
        /// Geter skupín
        /// </summary>
        /// <returns>Vráti zoznam skupín</returns>
		public HashSet<Skupina> GetSkupiny() { return Skupiny; }

        /// <summary>
        /// Vyhľadanie skupiny podľa mena
        /// </summary>
        /// <param name="paKluc">Unikátne meno skupiny</param>
        /// <returns>Vrati objekt skupiny ak taká existuje. Inak null.</returns>
		public Skupina GetSkupina(string paMeno)
        {
            return Skupiny.FirstOrDefault(s => s.Meno == paMeno);
        }

        /// <summary>
        /// Pokus o vytvorenie nového používateľa
        /// </summary>
        /// <param name="paPouzivatel">Objekt používateľa</param>
        /// <returns>Vráti true ak sa podarilo vytvoriť nového užívateľa, inak false.</returns>
		public bool VytvorNovehoPouzivatela(Pouzivatel paPouzivatel)
		{
            var uzivatel = Pouzivatelia.Count(s => s.Meno == paPouzivatel.Meno);
            if (uzivatel != 0)
            {
                return false;
            }
            Pouzivatelia.Add(paPouzivatel);
			return true;
		}

        /// <summary>
        /// Vytvorenie novej skupiny
        /// </summary>
        /// <param name="paSkupina">Objekt skupiny</param>
		public void VytvorNovuSkupinu(Skupina paSkupina)
		{
			Skupiny.Add(paSkupina);
		}

        /// <summary>
        /// Prihlásenie používateľa do systemu
        /// </summary>
        /// <param name="paMeno">Meno používateľa</param>
        /// <param name="paHeslo">Heslo používateľa</param>
        /// <returns>Vrati true ak sa podarilo prihlasit, inak false.</returns>
		public bool Login(string paMeno, string paHeslo)
		{
			if (ContainsPouzivatel(paMeno))
			{
				// prihlasenie podla mena a hesla
				string hesloData = GetPouzivatel(paMeno).Heslo;
				string solData = GetPouzivatel(paMeno).Sol;
				if (hesloData == GenerujPosolenuHashku(paHeslo, solData))
				{
					return true;
				}
			}
			return false;
		}

        /// <summary>
        /// Generovanie solenej hash reprezentacie hesla
        /// </summary>
        /// <param name="paVstup">Heslo</param>
        /// <param name="paSol">Sol</param>
        /// <returns>Vráti hashovane heslo</returns>
		public string GenerujPosolenuHashku(string paVstup, string paSol)
		{
			using (HashAlgorithm algoritmus = new SHA256Managed())
			{
				byte[] solenyVstup = System.Text.Encoding.UTF8.GetBytes(paVstup + paSol);
				byte[] hashka = algoritmus.ComputeHash(solenyVstup);
				return ByteArrayToHexString(hashka);
			}
		}

        /// <summary>
        /// Konverzia Byte poľa na hexa string
        /// </summary>
        /// <param name="paPolo">Byte pole</param>
        /// <returns>Vráti hexa string</returns>
		public static string ByteArrayToHexString(byte[] paPolo)
		{
			StringBuilder hex = new StringBuilder(paPolo.Length * 2);
			foreach (byte b in paPolo)
				// reprezentujem prvky byte pola ako hex
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

        /// <summary>
        /// Generovanie náhodného stringu
        /// </summary>
        /// <returns>Vráti string o dĺžke - MaxSaltLength</returns>
		public string GenerujSol()
		{
			using (RandomNumberGenerator random = new RNGCryptoServiceProvider())
			{
				byte[] sol = new byte[MaxSaltLength];
				random.GetNonZeroBytes(sol);
				return Convert.ToBase64String(sol);
			}
		}

        /// <summary>
        /// Vyhľadávanie používateľov
        /// </summary>
        /// <param name="paMeno">Meno používateľa</param>
        /// <param name="paTyp">Typ používateľa</param>
        /// <param name="paEmail">Email používateľa</param>
        /// <param name="paTelefon">Telefonne cislo používateľa</param>
        /// <param name="paAktivny">Stav prihlasenia používateľa</param>
        /// <returns>Vrati vysledok vyhladavania ako HashSet</returns>
		public HashSet<Pouzivatel> HladajPouzivatel(string paMeno = null, FTyp paTyp = FTyp.VSETKO, string paEmail = null, string paTelefon = null, string paAktivny = null)
		{
            // vyhľadávanie používateľov
            HashSet<Pouzivatel> navrat = new HashSet<Pouzivatel>();
			foreach (Pouzivatel polozka in Pouzivatelia) // Uzivatelia.Values
			{
				// hľadanie ihly
				bool presiel = false;
				if (paMeno != null) { presiel = polozka.Meno.Contains(paMeno); }
				if (paTyp != FTyp.VSETKO) { presiel = paTyp == polozka.Typ ? true : false; } //else { presiel = true; }
				if (paEmail != null) { presiel = polozka.Email.Contains(paEmail); }
				if (paTelefon != null) { presiel = polozka.Telefon.Contains(paTelefon); }
				if (paAktivny != null)
				{
					if (paAktivny == "A") { presiel = true == polozka.Aktivny ? true : false; }
					if (paAktivny == "N") { presiel = false == polozka.Aktivny ? true : false; }
				}
				if (presiel)
				{
					navrat.Add(polozka);
				}
			}
			return navrat;
		}

        /// <summary>
        /// Hľadanie skupiny
        /// </summary>
        /// <param name="paMeno">Meno skupiny</param>
        /// <param name="paTyp">Typ skupiny</param>
        /// <param name="paVeduci">Vedúci skupiny</param>
        /// <returns>Vráti výsledok vyhľadávania ako HashSet</returns>
		public HashSet<Skupina> HladajSkupina(string paMeno = null, FTyp paTyp = FTyp.VSETKO, string paVeduci = null)
		{
			// vyhľádavanie skupín
			HashSet<Skupina> navrat = new HashSet<Skupina>();
			foreach (Skupina polozka in Skupiny)
			{
				// hľadanie ihly
				bool presiel = false;
				if (paMeno != null) { presiel = polozka.Meno.Contains(paMeno); }
				if (paVeduci != null) { presiel = polozka.VeduciSkupiny.Contains(paVeduci); }
				if (paTyp != FTyp.VSETKO) { presiel = paTyp == polozka.Typ ? true : false; } //else { presiel = true; }
                // premysliet si VSETKO a NIC -> VSETKO bude znamenat akykolvek typ
				if (presiel)
				{
					navrat.Add(polozka);
				}
			}
			return navrat;
		}

        /// <summary>
        /// Ziskavanie podskupin skupiny
        /// </summary>
        /// <param name="paMeno">Meno skupiny</param>
        /// <returns>Vrati HashSet skupin ktore su podskupinami skupiny.</returns>
		public HashSet<Skupina> GetPodskupiny(string paMeno)
		{
			// retrievnutie podskupin
			HashSet<Skupina> result = new HashSet<Skupina>();
			HashSet<string> tMena = GetSkupina(paMeno).Podskupiny;
			foreach (Skupina polozka in Skupiny)
			{
				if (polozka.Meno != paMeno && tMena.Contains(polozka.Meno))
				{
					result.Add(polozka);
				}
			}
			return result;
		}

        /// <summary>
        /// Ziskavanie clenskych skupin
        /// </summary>
        /// <param name="paMeno">Meno pouzivatela</param>
        /// <returns>Vrati HashSet skupin, ktorych je pouzivatel clenom.</returns>
		public HashSet<Skupina> GetZaclenenie(string paMeno)
		{
			// retrievnutie clenstva
			HashSet<Skupina> result = new HashSet<Skupina>();
			HashSet<string> tMena = GetPouzivatel(paMeno).Skupiny;
			foreach (Skupina polozka in Skupiny)
			{
				if (polozka.Meno != paMeno && tMena.Contains(polozka.Meno))
				{
					result.Add(polozka);
				}
			}
			return result;
		}

        /// <summary>
        /// Ziskavanie clenov skupiny
        /// </summary>
        /// <param name="paMeno">Meno skupiny</param>
        /// <returns>Vrati HashSet pouzivatelov, ktory su slenmi skupiny</returns>
		public List<Pouzivatel> GetPodPouzivatelov(string paMeno)
		{
			// retrievnutie uzivatelov skupiny
			List<Pouzivatel> result = new List<Pouzivatel>();
			HashSet<string> tMena = GetSkupina(paMeno).Clenovia;
			foreach (Pouzivatel polozka in Pouzivatelia)
			{
				if (tMena.Contains(polozka.Meno))
				{
					result.Add(polozka);
				}
			}
			return result;
		}
	}
}
