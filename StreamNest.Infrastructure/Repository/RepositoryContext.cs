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
        .HasForeignKey(vt => vt.VideoId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VideoTag>()
        .HasOne(vt => vt.Tag)
        .WithMany(t => t.VideoTags)
        .HasForeignKey(vt => vt.TagId)
        .OnDelete(DeleteBehavior.Cascade);

        // // Optional: make Tag name unique
        // modelBuilder.Entity<Tag>()
        // .HasIndex(t => t.Name)
        // .IsUnique();

        // Configure Comment entity
        modelBuilder.Entity<Comment>()
        .HasOne(c => c.User)
        .WithMany(u => u.Comments)
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
        .HasOne(c => c.Video)
        .WithMany(v => v.Comments)
        .HasForeignKey(c => c.VideoId)
        .OnDelete(DeleteBehavior.Cascade);

        // Configure Like entity
        modelBuilder.Entity<Like>()
        .HasOne(l => l.User)
        .WithMany(u => u.Likes)
        .HasForeignKey(l => l.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Like>()
        .HasOne(l => l.Video)
        .WithMany(v => v.Likes)
        .HasForeignKey(l => l.VideoId)
        .OnDelete(DeleteBehavior.Cascade);

    }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<VideoTag> VideoTags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }

}