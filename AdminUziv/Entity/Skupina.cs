using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace Entity
{
    [DataContract]
	public class Skupina
	{
        /// <summary>
        /// Meno skupiny
        /// </summary>
        /// <remarks> 
        /// String
        /// </remarks>
        [Key]
        [DataMember]
        public string Meno { get; set; }

        /// <summary>
        /// Textová poznámka
        /// </summary>
        /// <remarks> 
        /// String
        /// </remarks>
        [DataMember]
        public string Poznamka { get; set; }

        /// <summary>
        /// Interny bitovy identifikator skupiny v GVD modulu
        /// </summary>
        [DataMember]
        public FTyp Typ { get; set; }

        /// <summary>
		/// Zoznam skupin zaradenych v danej skupine
		/// </summary>
		/// <remarks> 
		/// HashSet mien podskupin
		/// </remarks>
        [DataMember]
        public HashSet<string> Podskupiny { get; set; }
        //http://www.vcskicks.com/csharp_data_structures2.php

        /// <summary>
        /// Zoznam skupin zaradenych v danej skupine pre DB
        /// </summary>
        /// <remarks> 
        /// string mien podskupin
        /// </remarks>
        [DataMember]
        public string PodskupinyDbo { get; set; }

        public void AddPodskupinu(string paMeno) {
            if(paMeno != Meno)
            {
                Podskupiny.Add(paMeno);
            }
        }

        /// <summary>
        /// Zoznam clenov zaradenych v danej skupine
        /// </summary>
        /// <remarks> 
        /// HashSet mien clenov
        /// </remarks>
        [DataMember]
        public HashSet<string> Clenovia { get; set; }

        /// <summary>
        /// Zoznam clenov zaradenych v danej skupine pre DB
        /// </summary>
        /// <remarks> 
        /// string mien clenov
        /// </remarks>
        [DataMember]
        public string ClenoviaDbo { get; set; }

        /// <summary>
        /// Výpis podskupína
        /// </summary>
        /// <returns>String obsahujúci podskupiny danej skupiny</returns>
        public string GetPodskupiny()
        {
            string tHelp = "";
            foreach (string meno in Podskupiny)
            {
                if(meno != "")
                {
                    tHelp += meno + ", ";
                }
            }
            if(tHelp.Length >= 2)
            {
                return tHelp.Substring(0, tHelp.Length - 2);
            } else
            {
                return tHelp;
            }
            
        }

        /// <summary>
        /// Výpis členov
        /// </summary>
        /// <returns>String obsahujúci členov danej skupiny</returns>
        public string GetClenov()
        {
            string tHelp = "";
            foreach(string meno in Clenovia)
            {
                if (meno != "")
                {
                    tHelp += meno + ", ";
                }
            }
            if (tHelp.Length >= 2)
            {
                return tHelp.Substring(0, tHelp.Length - 2);
            }
            else
            {
                return tHelp;
            }
        }

        /// <summary>
        /// Meno veduceho skupiny
        /// </summary>
        /// <remarks> 
        /// len tento pridava clenov
        /// </remarks>
        [DataMember]
        public string VeduciSkupiny { get; set; }
        
        /// <summary>
        /// Metoda ktorá pridá člena skupiny
        /// </summary>
        /// <param name="paMeno">Meno nového člena skupiny</param>
        public void AddClena(string paMeno)
        {
            Clenovia.Add(paMeno);
        }

        /// <summary>
        /// Konštruktor triedy Skupina
        /// </summary>
        /// <param name="paMeno">Meno skupiny</param>
        /// <param name="paTyp">Typ skupiny</param>
        /// <param name="paPoznamka">Poznámka skupiny</param>
        /// <param name="paVeduci">Veduci skupiny</param>
        public Skupina(string paMeno, FTyp paTyp, string paPoznamka, string paVeduci)
        {
            this.Meno = paMeno;
            this.Typ = paTyp;
            this.Poznamka = paPoznamka;
            this.VeduciSkupiny = paVeduci;
            this.Podskupiny = new HashSet<string>();
            this.Clenovia = new HashSet<string>();
        }

        /// <summary>
        /// Bezparametricky konštruktor triedy Skupina
        /// </summary>
        public Skupina()
        {
            this.Podskupiny = new HashSet<string>();
            this.Clenovia = new HashSet<string>();
        }

        /// <summary>
		/// Vypis skupiny ToString
		/// </summary>
        public override string ToString()
        {
            return this.Meno.ToString();
        }
    }
}
