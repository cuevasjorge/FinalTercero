using FinalTercero.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalTercero.Data
{
    public class DBPrimerParcialJorgeCuevas : DbContext
    {
        public DBPrimerParcialJorgeCuevas(DbContextOptions<DBPrimerParcialJorgeCuevas> options) : base(options)
        {

        }

        public DbSet<Cliente> Cliente => Set<Cliente>();
        public DbSet<Usuario> Usuario => Set<Usuario>();

    }
}
