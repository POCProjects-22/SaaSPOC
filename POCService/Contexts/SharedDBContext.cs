
using POCModel.Security;
using POCModel.UserInfo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace POCService.Contexts
{
    /// <summary>
    /// All free user's personal information
    /// </summary>
    public class SharedDBContext : IdentityDbContext
    {
        public SharedDBContext()
        {

        }
        public SharedDBContext(DbContextOptions options) : base(options)
        {

        }
        #region ctors and default methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var POCShared_db = "POCShared_db";
                var connectionString = configuration.GetConnectionString(POCShared_db);

                optionsBuilder.UseSqlServer(connectionString);

            }
        }


        #endregion ctors and default methods  
        public DbSet<FavoritePL> FavoritePL { get; set; }


    }
}
