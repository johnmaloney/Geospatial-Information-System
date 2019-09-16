using Messaging;
using Messaging.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace AdminManagementApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<GeneralMessage> GeneralMessages { get; set; }

        public DbSet<GeneralCommand> GeneralCommands { get; set; }

        public DbSet<TopicMessage> TopicMessages { get; set; }

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GeneralCommand>().Ignore("CommandDataCollection");
        }
    }
}
