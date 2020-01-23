using System.Collections.Generic;
using System.Linq;
using Entity;

namespace ServiceApp
{
    class WcfKangoService : IWcfKangoService
    {
        /// <summary>
        /// Databázový kontext
        /// </summary>
        private readonly WcfDbContext _context;
        /// <summary>
        /// List Skupín z databázy
        /// </summary>
        public List<Skupina> Group { get; set; }
        /// <summary>
        /// List používateľov z databázy
        /// </summary>
        public List<Pouzivatel> User { get; set; }

        /// <summary>
        /// Konštruktor triedy WcfKangoService
        /// </summary>
        public WcfKangoService()
        {
            _context = new WcfDbContext();
            Group = _context.Groups.ToList();
            User = _context.Users.ToList();
        }

        /// <summary>
        /// Načítanie používateľov z DB
        /// </summary>
        /// <returns>Hashset používateľov</returns>
        public HashSet<Pouzivatel> LoadPouzivatelia()
        {
            return User.ToHashSet();
        }

        /// <summary>
        /// Načítanie skupín z DB
        /// </summary>
        /// <returns>Hashset skupín</returns>
        public HashSet<Skupina> LoadSkupiny()
        {
            return Group.ToHashSet();
        }

        /// <summary>
        /// Uloženie používateľov do DB
        /// </summary>
        /// <param name="paUsers">Používatelia pre uloženie</param>
        public void SavePouzivatelia(HashSet<Pouzivatel> paUsers)
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
            _context.Users.AddRange(paUsers);
            _context.SaveChanges();
        }

        /// <summary>
        /// Uloženie skupín do DB
        /// </summary>
        /// <param name="paGroups">Skupiny pre uloženie</param>
        public void SaveSkupiny(HashSet<Skupina> paGroups)
        {
            _context.Groups.RemoveRange(_context.Groups);
            _context.SaveChanges();
            _context.Groups.AddRange(paGroups);
            _context.SaveChanges();
        }

    }
}
