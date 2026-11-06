using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudyWithChatgpt.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogPost>(entity =>
            {
                entity.Property(p => p.CategoryId).HasColumnName("CategoryID");
                entity.Property(p => p.ImageUrl).HasMaxLength(300);

                entity.HasOne(p => p.User)
                    .WithMany(u => u.BlogPosts)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.BlogPosts)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => p.CreatedAt);
                entity.HasIndex(p => p.UserId);
                entity.HasIndex(p => p.CategoryId);
            });

            builder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.BlogPost)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.BlogPostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(c => c.BlogPostId);
                entity.HasIndex(c => c.UserId);
            });

            builder.Entity<PostLike>(entity =>
            {
                entity.ToTable("PostLike");

                entity.HasOne(l => l.BlogPost)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(l => l.BlogPostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(l => new { l.UserId, l.BlogPostId }).IsUnique();
                entity.HasIndex(l => l.BlogPostId);
            });

            builder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(60).IsRequired();
                entity.HasIndex(c => c.Name).IsUnique();
            });
        }
    }
}
