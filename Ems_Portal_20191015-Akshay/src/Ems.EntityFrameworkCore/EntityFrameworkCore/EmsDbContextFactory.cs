using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Ems.Configuration;
using Ems.Web;

namespace Ems.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class EmsDbContextFactory : IDesignTimeDbContextFactory<EmsDbContext>
    {
        public EmsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EmsDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), addUserSecrets: true);

            EmsDbContextConfigurer.Configure(builder, configuration.GetConnectionString(EmsConsts.ConnectionStringName));

            return new EmsDbContext(builder.Options);
        }
    }
}