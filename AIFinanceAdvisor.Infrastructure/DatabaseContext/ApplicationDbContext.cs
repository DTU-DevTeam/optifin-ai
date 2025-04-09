using AIFinanceAdvisor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinanceAdvisor.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ChatMessageHistoryForUser> ChatMessageHistoryForUsers { get; set; }
        public DbSet<ChatMessageHistoryForAssitant> ChatMessageHistoryForAssistants { get; set; }

        public DbSet<ChatHistoryForUser> ChatHistoryForUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChatMessageHistoryForUser>().ToTable("ChatMessageHistoryForUsers");
            modelBuilder.Entity<ChatMessageHistoryForAssitant>().ToTable("ChatMessageHistoryForAssistants");
            modelBuilder.Entity<ChatHistoryForUser>().ToTable("ChatHistoryForUsers");
        }
    }
   
}
