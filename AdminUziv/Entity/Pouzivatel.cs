using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Entity
{
	/// <summary>
	/// Enum vyjadrujúci interný bitový identifikátor z GVD modulu
	/// </summary>
	[Flags]
    public enum FTyp
	{
        VSETKO = 0x11111111,    // vsetko - ziadny filter
        Anonym = 0x0000000,                 // Read-only používateľ     
		Administrátor = 0x00000001,         // Super administrátor - všetky prístupové práva
		KmenKategorizacia = 0x00000080,		// Kategorizácia úsekov a bodov 
		KmenDynamikCP = 0x00000100,		    // Dynamik centrálneho pracoviska	 
		KmenDynamikOBS = 0x00000200,		// Dynamik OBS KANGO-Kmen
		TTPAdministrator = 0x00000400,		// Administrátor TTP
		KmenInspektor = 0x00000800,			// Inšpektor KANGO-Kmen
		KmenSpravcaInfrastruktury = 0x000001000, // Správca dát infraštruktúry
		KmenPriepustník = 0x000002000,		// Priepustník
		KmenSpravcaSR70 = 0x000004000,		// Správca SR70
		URMIZA = 0x000008000,				// URMIZA
		KmenSJRVSDZ = 0x000010000,			// Správca SJŘ
		KmenCI = 0x000020000,				// Správca CI
		TTPSpracovatelDynamik = 0x00040000,	// Spracovateľ TTP - Dynamik
		TTPSpracovatelPrechodnost = 0x00080000,	// Spracovateľ TTP - Prechodnosť
		TTPSpracovatelETCS = 0x00100000,		// Spracovateľ TTP - OAE-ETCS
		TTPSpracovatelElektro = 0x00200000,		// Spracovateľ TTP - OAE-Elektro
		TTPSpracovatelTUDC = 0x00400000,		// Spracovateľ TTP - TUDC
		TTPSpracovatelRizeniProvozu = 0x00800000, // Spracovateľ TTP - riadenie prevádzky
		//        GTN = 0x01000000,						// GTN používateľ 
		TTPSpracovatelSpravaTrati = 0x02000000,	// Spracovateľ TTP - Správa tratí
		TTPSpracovatelSSZT = 0x04000000,		// Spracovateľ TTP - Správa oznamovacej a zabezpečovacej techniky
		TTPSpracovatelSMT = 0x08000000,		// Spracovateľ TTP - Správa mostov a tunelov
		TTPSpracovatelSEE = 0x10000000,		// Spracovateľ TTP - Správa elektrotechniky a energetiky
		TTPHlavnySpracovatel = 0x20000000,		// Spracovateľ TTP - Hlavný spracovateľ
		TTPSpravca = 0x40000000,		// Spracovateľ TTP - Správca
	}

	//[Table("User")]
    [DataContract(IsReference = true)]
	public class Pouzivatel : IComparable<Pouzivatel>
	{

		/// <summary>
		/// Meno používateľa pre login. 
		/// </summary>
		/// <remarks> 
		/// String
		/// Nesmie byť null ani prázdny
		/// </remarks>
		[Key]
        [DataMember]
		public string Meno { get; set; }

		/// <summary>
		/// Heslo používateľa pre login. 
		/// </summary>
		/// <remarks> 
		/// String
		/// Nesmie byť null ani prázdny
		/// </remarks>
		[DataMember]
		public string Heslo { get; set; }

		/// <summary>
		/// Sol hesla používateľa pre login. 
		/// </summary>
		/// <remarks> 
		/// Nesmie byť null ani prázdny
		/// </remarks>
		[DataMember]
		public string Sol { get; set; }

		/// <summary>
		/// Typ používateľa  (konštruktér, vlakotvorca, inšpektor, dynamik, ...)
		/// </summary>
		[DataMember]
		public FTyp Typ { get; set; }

		/// <summary>
		/// e-mail používateľa
		/// </summary>
		/// <remarks> 
		/// String
		/// </remarks>
        [DataMember]
		public string Email { get; set; }

		/// <summary>
		/// Tel. číslo používateľa
		/// </summary>
		/// <remarks> 
		/// String
		/// </remarks>
        [DataMember]
		public string Telefon { get; set; }

		/// <summary>
		/// Textová poznámka
		/// </summary>
		/// <remarks> 
		/// String
		/// </remarks>
        [DataMember]
		public string Poznamka { get; set; }

		/// <summary>
		/// Indikátor aktívnosti používateľa
		/// </summary>
        [DataMember]
		public bool Aktivny { get; set; }

		/// <summary>
		/// Zoznam skupín zaradených v danej skupine pre ukladanie do DB
		/// </summary>
		/// <remarks> 
		/// String mien skupín
		/// </remarks>
		[DataMember]
		public string SkupinyDbo { get; set; }

		/// <summary>
		/// Zoznam skupín zaradenych v danej skupine
		/// </summary>
		/// <remarks> 
		/// HashSet mien skupín
		/// </remarks>
		[DataMember]
		public HashSet<string> Skupiny { get; set; }

		/// <summary>
		/// Konštruktor triedy Pouzivatel
		/// </summary>
		/// <param name="paMeno">Meno používateľa</param>
		/// <param name="paHeslo">Heslo používateľa</param>
		/// <param name="paTyp">Typ používateľa</param>
		/// <param name="paEmail">Email používateľa</param>
		/// <param name="paTelefon">Telefónne číslo používateľa</param>
		/// <param name="paPoznamka">Poznámka o používateľovi</param>
		/// <param name="paAktivny">Aktivnosť používateľa</param>
		public Pouzivatel(string paMeno, string paHeslo, FTyp paTyp, string paEmail, string paTelefon, string paPoznamka, bool paAktivny)
        {
            this.Meno = paMeno;
            this.Heslo = paHeslo;
            this.Typ = paTyp;
            this.Email = paEmail;
            this.Telefon = paTelefon;
            this.Poznamka = paPoznamka;
            this.Aktivny = paAktivny;
            Skupiny = new HashSet<string>();
            SkupinyDbo = "";
        }

		/// <summary>
		/// Bezparametricky konštruktor triedy Pouzivatel
		/// </summary>
        public Pouzivatel()
        {
            Skupiny = new HashSet<string>();
            SkupinyDbo = "";
		}

		/// <summary>
		/// Komparátor
		/// </summary>
		/// <param name="other">Porovnávajúci objekt typu Pouzivatel</param>
		/// <returns>Vráti výsledok porovnania objektov typu Pouzivatel</returns>
        public int CompareTo(Pouzivatel other)
        {
                return this.Meno.CompareTo(other.Meno);
        }

        /// <summary>
		/// Vypis uzivatela
		/// </summary>
        public override string ToString()
        {
            return this.Meno.ToString();
        }
    }
}
