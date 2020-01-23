using System;
using System.Collections.Generic;
using System.Xml;
using Entity;

namespace Xml
{
	public class XmlReadWrite
	{

        /// <summary>
        /// Nacitanie XML dokumentu so vstupnymi datami pouzivatelov
        /// </summary>
        /// <param name="paNazov">Cesta k suboru</param>
        /// <param name="paPouzivatelia">Hashset pouzivatelov</param>
        public void LoadPouzivatelia(string paNazov, HashSet<Pouzivatel> paPouzivatelia)
		{
			XmlDocument dokument = new XmlDocument();
			dokument.Load(paNazov);
			foreach (XmlNode node in dokument.DocumentElement)
			{
				string meno = node.Attributes[0].InnerText;
				XmlNode decko = node.FirstChild;
				string heslo = decko.InnerText;
				decko = decko.NextSibling;
				string sol = decko.InnerText;
				decko = decko.NextSibling;
				string typ = decko.InnerText;
                Enum.TryParse<FTyp>(typ, out var typOzaj);
				decko = decko.NextSibling;
				string email = decko.InnerText;
				decko = decko.NextSibling;
				string telefon = decko.InnerText;
				decko = decko.NextSibling;
				string poznamka = decko.InnerText;
				decko = decko.NextSibling;
				bool aktivny = decko.InnerText == "true" ? true : false;
				decko = decko.NextSibling;
				string tDbo = decko.InnerText;
                string[] zaradenie = tDbo.Split(';');
				HashSet<string> zaradenie_polo = new HashSet<string>();
                foreach (string polozka in zaradenie) { zaradenie_polo.Add(polozka); }
                Pouzivatel novy = new Pouzivatel(meno, heslo, typOzaj, email, telefon, poznamka, aktivny)
                {
                    SkupinyDbo = tDbo, Sol = sol, Skupiny = zaradenie_polo
                };
                paPouzivatelia.Add(novy);
			}
		}

		/// <summary>
		/// Nacitanie XML dokumentu so vstupnymi datami skupin
		/// </summary>
		/// <param name="paNazov">Cesta k suboru</param>
		/// <param name="paSkupiny">Hashset skupin</param>
		public void LoadSkupiny(string paNazov, HashSet<Skupina> paSkupiny)
		{
			XmlDocument dokument = new XmlDocument();
			dokument.Load(paNazov);
			foreach (XmlNode node in dokument.DocumentElement)
			{
				string meno = node.Attributes[0].InnerText;
				XmlNode decko = node.FirstChild;
				string veduciSkupiny = decko.InnerText;
				decko = decko.NextSibling;
				string typ = decko.InnerText;
                Enum.TryParse<FTyp>(typ, out var typOzaj);
				decko = decko.NextSibling;
				string poznamka = decko.InnerText;
				decko = decko.NextSibling;
				string tDboPodskupiny = decko.InnerText;
                string[] podskupiny = tDboPodskupiny.Split(';');
				decko = decko.NextSibling;
				string tDboClenovia = decko.InnerText;
                string[] clenovia = tDboClenovia.Split(';');
				HashSet<string> podskupiny_polo = new HashSet<string>();
				HashSet<string> clenovia_polo = new HashSet<string>();
				foreach (string polozka in podskupiny) { podskupiny_polo.Add(polozka); }
				foreach (string polozka in clenovia) { clenovia_polo.Add(polozka); }
                Skupina nova = new Skupina(meno, typOzaj, poznamka, veduciSkupiny)
                {
                    PodskupinyDbo = tDboPodskupiny,
                    ClenoviaDbo = tDboClenovia,
                    Podskupiny = podskupiny_polo,
                    Clenovia = clenovia_polo
                };
                paSkupiny.Add(nova);
			}

		}

        /// <summary>
        /// Ukladanie XML dokumentu so vstupnymi datami pouzivatelov
        /// </summary>
        /// <param name="paNazov">Cesta k suboru</param>
        /// <param name="paPouzivatelia">Hashset pouzivatelov</param>
		public void SavePouzivatelia(string paNazov, HashSet<Pouzivatel> paPouzivatelia)
		{
			XmlWriter pisatel = XmlWriter.Create(paNazov);
			pisatel.WriteStartDocument();
			pisatel.WriteStartElement("root");
			foreach (Pouzivatel polozka in paPouzivatelia)
			{
				pisatel.WriteStartElement("uzivatel");
				pisatel.WriteAttributeString("meno", polozka.Meno);
				pisatel.WriteStartElement("heslo");
				pisatel.WriteValue(polozka.Heslo);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("sol");
				pisatel.WriteValue(polozka.Sol);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("typ");
				pisatel.WriteValue(polozka.Typ.ToString());
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("email");
				pisatel.WriteValue(polozka.Email);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("telefon");
				pisatel.WriteValue(polozka.Telefon);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("poznamka");
				pisatel.WriteValue(polozka.Poznamka);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("aktivny");
				pisatel.WriteValue(polozka.Aktivny);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("zaradenie");
				string akumulator = "";
				foreach (string clenstvo in polozka.Skupiny)
				{
					akumulator += clenstvo + ";";
				}
				pisatel.WriteValue(akumulator);
				pisatel.WriteEndElement();

				pisatel.WriteEndElement();
			}
			pisatel.WriteEndElement();
			pisatel.WriteEndDocument();
			pisatel.Close();
			pisatel.Flush();
		}

        /// <summary>
        /// Ukladanie XML dokumentu so vstupnymi datami skupin
        /// </summary>
        /// <param name="paNazov">Cesta k suboru</param>
        /// <param name="paSkupiny">Hashset skupin</param>
		public void SaveSkupiny(string paNazov, HashSet<Skupina> paSkupiny)
		{
			XmlWriter pisatel = XmlWriter.Create(paNazov);
			pisatel.WriteStartDocument();
			pisatel.WriteStartElement("root");
			foreach (Skupina polozka in paSkupiny)
			{
				pisatel.WriteStartElement("skupina");
				pisatel.WriteAttributeString("meno", polozka.Meno);

				pisatel.WriteStartElement("veduci");
				pisatel.WriteValue(polozka.VeduciSkupiny);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("typ");
				pisatel.WriteValue(polozka.Typ.ToString());
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("poznamka");
				pisatel.WriteValue(polozka.Poznamka);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("podskupiny");
				string akumulator_podskupiny = "";
				foreach (string clenstvo in polozka.Podskupiny)
				{
					akumulator_podskupiny += clenstvo + ";";
				}
				pisatel.WriteValue(akumulator_podskupiny);
				pisatel.WriteEndElement();

				pisatel.WriteStartElement("clenovia");
				string akumulator_clenovia = "";
				foreach (string clenstvo in polozka.Clenovia)
				{
					akumulator_clenovia += clenstvo + ";";
				}
				pisatel.WriteValue(akumulator_clenovia);
				pisatel.WriteEndElement();

				pisatel.WriteEndElement();
			}
			pisatel.WriteEndElement();
			pisatel.WriteEndDocument();
			pisatel.Close();
			pisatel.Flush();
		}
	}
}
