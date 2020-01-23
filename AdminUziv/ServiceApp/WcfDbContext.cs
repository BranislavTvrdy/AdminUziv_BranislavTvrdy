using Entity;
using System.Data.Entity;
using System.Linq;
using ModelBuilder = System.Data.Entity.DbModelBuilder;

namespace ServiceApp
{
    public class WcfDbContext : DbContext
    {
        /// <summary>
        /// Dataset skupín
        /// </summary>
        public virtual System.Data.Entity.DbSet<Skupina> Groups { get; set; }
        /// <summary>
        /// Dataset používateľov
        /// </summary>
        public virtual System.Data.Entity.DbSet<Pouzivatel> Users { get; set; }

        /// <summary>
        /// Inicializacia spojenia s DB
        /// </summary>
        public WcfDbContext() : base("name=BaliContext")
        {
        }

        /// <summary>
        /// Metóda pre retrievnutie údajov z DB
        /// </summary>
        /// <param name="modelBuilder">Model builder pre čítanie tabuliek</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            System.Data.Entity.Database.SetInitializer<WcfDbContext>(null);

            modelBuilder.Entity<Skupina>().ToTable("Group");

            modelBuilder.Entity<Pouzivatel>().ToTable("User");

//            modelBuilder.Entity<Skupina>().HasMany(skupina => skupina.Podskupiny);
//            modelBuilder.Entity<Skupina>().HasMany(clenovia => clenovia.Clenovia);
//            modelBuilder.Entity<Uzivatel>().HasMany(skupina => skupina.Skupiny);

        }
    }
}
