using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
          : base(dbContextOptions)
        {

        }
        public DbSet<Persons> persons { get; set; }
        public DbSet<Movies> movies { get; set; }
        public DbSet<Genre> genres { get; set; }
        public DbSet<PersonsMovieMaps> personsMovieMaps { get; set;}
        public DbSet<UserAccounts> userAccounts { get; set; }
    }
}
