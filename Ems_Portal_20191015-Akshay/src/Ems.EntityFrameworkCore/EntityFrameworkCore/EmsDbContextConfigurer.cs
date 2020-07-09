using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Ems.EntityFrameworkCore
{
    public static class EmsDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<EmsDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<EmsDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}