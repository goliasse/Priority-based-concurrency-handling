using Microsoft.EntityFrameworkCore;

namespace MessageProcessor.Core.Data;

public class YourDbContext : DbContext
{
    public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
    {
    }

    public DbSet<RawDomainFrame> RawDomainFrames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RawDomainFrame>()
            .Property(e => e.RowVersion)
            .IsRowVersion();
    }
}