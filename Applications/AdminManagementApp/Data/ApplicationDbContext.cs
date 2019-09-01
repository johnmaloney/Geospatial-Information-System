using Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminManagementApp.Data
{
    public class ApplicationDbContext : DbContext
    {    
        public DbSet<GeneralMessage> Messages { get; set; }

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {

        }
    }
}
