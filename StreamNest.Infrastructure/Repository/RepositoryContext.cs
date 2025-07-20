using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StreamNest.Domain.Entities.Models;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add any additional configuration here
        modelBuilder.Entity<User>()
        .Property(u => u.Role)
        .HasConversion<string>();

        // Convert Genre and AgeRating enums to string
        modelBuilder.Entity<Video>()
        .Property(v => v.Genre)
        .HasConversion<string>();

        modelBuilder.Entity<Video>()
        .Property(v => v.AgeRating)
        .HasConversion<string>();
        
        // Video <-> Tag many-to-many
        modelBuilder.Entity<VideoTag>()
        .HasKey(vt => new { vt.VideoId, vt.TagId });

        modelBuilder.Entity<VideoTag>()
        .HasOne(vt => vt.Video)
        .WithMany(v => v.VideoTags)
        .HasForeignKey(vt => vt.VideoId);

        modelBuilder.Entity<VideoTag>()
        .HasOne(vt => vt.Tag)
        .WithMany(t => t.VideoTags)
        .HasForeignKey(vt => vt.TagId);

        // Optional: make Tag name unique
        modelBuilder.Entity<Tag>()
        .HasIndex(t => t.Name)
        .IsUnique();
    }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<VideoTag> VideoTags { get; set; }

}