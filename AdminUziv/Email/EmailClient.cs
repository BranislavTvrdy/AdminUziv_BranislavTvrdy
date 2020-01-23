using System;
using System.Net.Mail;
using Entity;
using System.Collections.Specialized;



namespace AdminUziv
{
	public class EmailClient
    {
        /// <summary>
        /// Adresát emailu
        /// </summary>
        public string Adresat { get; set; }
        /// <summary>
        /// Odosielateľ emailu
        /// </summary>
        public Pouzivatel Odosielatel { get; set; }
        /// <summary>
        /// Predmet emailu
        /// </summary>
        public string Predmet { get; set; }
        /// <summary>
        /// Správa emailu
        /// </summary>
        public string Sprava { get; set; }
        /// <summary>
        /// Emailova adresa emailového klienta
        /// </summary>
        public string SystemAdresa { get; set; }
        /// <summary>
        /// Heslo Emailového klienta
        /// </summary>
        public string SystemHeslo { get; set; }
        /// <summary>
        /// Emailovy klient
        /// </summary>
        public string SystemKlient { get; set; }

        /// <summary>
		/// Konstruktor emailoveho klienta
		/// </summary>
        public EmailClient(string paAdtesat, string paPredmet, string paSprava, NameValueCollection paNastavenia, Pouzivatel paOdosielatel = null)
        {
            this.Adresat = paAdtesat;
            this.Odosielatel = paOdosielatel;
            this.Predmet = paPredmet;
            this.Sprava = paSprava;
            this.SystemAdresa = paNastavenia.Get("emailClient_adresa");
            this.SystemHeslo = paNastavenia.Get("emailClient_heslo");
            this.SystemKlient = paNastavenia.Get("emailClient_sluzba");
        }

        /// <summary>
		/// Zaslanie emailu
		/// </summary>
        public bool PoslatEmail()
        {
            try
            {
                MailMessage email = new MailMessage();
                SmtpClient client = new SmtpClient(SystemKlient)
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(SystemAdresa, SystemHeslo),
                    EnableSsl = true
                };
                //465 alebo 587
                email.Subject = Predmet;
                email.IsBodyHtml = true;
                email.Body = Sprava;
                email.To.Add(new MailAddress(Adresat));
                if(Odosielatel != null)
                {
                    email.From = new MailAddress(Odosielatel.Email, Odosielatel.Meno);
                } else
                {
                    email.From = new MailAddress(SystemAdresa, "System KANGO");
                }
                client.Send(email);

            } catch(Exception e)
            {
                System.Console.WriteLine(e);
                return false;
            }
            return true;
        }

    }
}
