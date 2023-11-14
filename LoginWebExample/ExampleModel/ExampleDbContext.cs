using Microsoft.EntityFrameworkCore;

namespace LoginWebExample.ExampleModel
{
    public partial class ExampleDbContext : DbContext
    {
        // Constuctors
        public ExampleDbContext() { }

        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options) { }

        // Tables
        public virtual DbSet<User> Users { get; set; } = null!;

        // Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(m =>
            {
                m.ToTable("");

                // TODO : DATA BASE DEFINITION
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}