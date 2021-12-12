
using POCModel.Security;
using POCModel.UserInfo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace POCService.Contexts
{

    /// <summary>
    /// only the paid user's personal information
    /// </summary>
    public class DedicatedDBContext : IdentityDbContext
    {
        private string _pocDedicatedDBName;
        private string _dedicatedSchemaDefinitionFile;
        public DedicatedDBContext()
        {

        }

        public DedicatedDBContext(string POCDedicatedDBName,string dedicatedSchemaDefinitionFile)
        {
            _pocDedicatedDBName = POCDedicatedDBName;
            _dedicatedSchemaDefinitionFile = dedicatedSchemaDefinitionFile;
        }
        public DedicatedDBContext(DbContextOptions options) : base(options)
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

                var POCMaster_db = "POCMaster_db";
                var connectionStringMaster = configuration.GetConnectionString(POCMaster_db);

                optionsBuilder.UseSqlServer(connectionStringMaster);

                //Only schema, not is so big
                string dedicatedDBSchemaFull = File.ReadAllText(_dedicatedSchemaDefinitionFile);
                //Run script using master_db
                var res=base.Database.ExecuteSqlRaw(dedicatedDBSchemaFull);

                ///////////////////////////////////////////////////
                var POCDedicated_db = "POCDedicated_db";
                var connectionString = configuration.GetConnectionString(POCDedicated_db).Replace("[dedicateddbname]", _pocDedicatedDBName);


                optionsBuilder.UseSqlServer(connectionString);

            }
        }


        #endregion ctors and default methods  
        public DbSet<FavoritePL> FavoritePL { get; set; }


    }
}
