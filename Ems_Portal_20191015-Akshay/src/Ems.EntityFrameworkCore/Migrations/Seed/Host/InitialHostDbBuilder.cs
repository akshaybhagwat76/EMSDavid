using Ems.Configuration;
using Ems.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Ems.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly EmsDbContext _context;

        public InitialHostDbBuilder(EmsDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
