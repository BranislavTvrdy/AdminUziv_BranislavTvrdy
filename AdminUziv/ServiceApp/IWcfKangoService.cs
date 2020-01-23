using System.Collections.Generic;
using System.ServiceModel;
using Entity;

namespace ServiceApp
{
    [ServiceContract]
    public interface IWcfKangoService
    {
        /// <summary>
        /// Deklarácia metódy pre načítanie používateľov
        /// </summary>
        /// <returns>Vráti načítaných používateľov</returns>
        [OperationContract]
        HashSet<Pouzivatel> LoadPouzivatelia();

        /// <summary>
        /// Deklarácia metódy pre načítanie skupín
        /// </summary>
        /// <returns>Vráti načítané skupiny</returns>
        [OperationContract]
        HashSet<Skupina> LoadSkupiny();

        /// <summary>
        /// Deklarácia metódy pre ukladanie používateľov
        /// </summary>
        /// <param name="paUsers">Používatelia pre uloženie</param>
        [OperationContract]
        void SavePouzivatelia(HashSet<Pouzivatel> paUsers);

        /// <summary>
        /// Deklarácia metódy pre ukladanie skupín
        /// </summary>
        /// <param name="paGroups">Skupiny pre uloženie</param>
        [OperationContract]
        void SaveSkupiny(HashSet<Skupina> paGroups);

    }
}
