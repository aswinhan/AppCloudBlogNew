namespace AppCloudBlog.Infrastructure.Persistence.Context;

// Inherit from IdentityDbContext to integrate with ASP.NET Core Identity
// Specify ApplicationUser, ApplicationRole, and Guid for primary key types
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    // DbSet properties for all our domain entities
    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<SavedPost> SavedPosts { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // DbSet properties for explicit join entities (Many-to-Many relationships)
    public DbSet<PostCategory> PostCategories { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<UserFollow> UserFollows { get; set; } // Renamed to plural for DbSet convention

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Call the base method to configure Identity tables
        base.OnModelCreating(modelBuilder);

        // Configure Guid primary keys to auto-generate using NEWID() for BaseEntity derived types
        // This applies to Post, Category, Tag, Comment, Like, SavedPost, Notification, PostCategory, PostTag, UserFollow
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                   .Property<Guid>(nameof(BaseEntity.Id))
                   .HasDefaultValueSql("NEWID()"); // [2]
            }
        }

        // --- Entity Configurations ---

        // ApplicationUser (IdentityUser<Guid>)
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(2000); // URL length
            entity.Property(e => e.RegistrationDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();

            // One-to-Many: ApplicationUser (Author) to Posts
            entity.HasMany(u => u.Posts)
                 .WithOne(p => p.Author)
                 .HasForeignKey(p => p.AuthorId)
                 .OnDelete(DeleteBehavior.Restrict); // Prevent deleting user if they have posts

            // One-to-Many: ApplicationUser (Commenter) to Comments
            entity.HasMany(u => u.Comments)
                 .WithOne(c => c.Commenter)
                 .HasForeignKey(c => c.CommenterId)
                 .OnDelete(DeleteBehavior.Restrict); // Prevent deleting user if they have comments

            // One-to-Many: ApplicationUser to Likes
            entity.HasMany(u => u.Likes)
                 .WithOne(l => l.User)
                 .HasForeignKey(l => l.UserId)
                 .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their likes are deleted

            // One-to-Many: ApplicationUser to SavedPosts
            entity.HasMany(u => u.SavedPosts)
                 .WithOne(sp => sp.User)
                 .HasForeignKey(sp => sp.UserId)
                 .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their saved posts are deleted

            // One-to-Many: ApplicationUser to Notifications
            entity.HasMany(u => u.Notifications)
                 .WithOne(n => n.User)
                 .HasForeignKey(n => n.UserId)
                 .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their notifications are deleted

            // One-to-Many: ApplicationUser to RefreshTokens
            entity.HasMany(u => u.RefreshTokens)
                 .WithOne(rt => rt.User)
                 .HasForeignKey(rt => rt.UserId)
                 .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their refresh tokens are deleted

            // Many-to-Many with self (UserFollow)
            entity.HasMany(u => u.Following) // Users this user is following
                 .WithOne(uf => uf.Follower)
                 .HasForeignKey(uf => uf.FollowerId)
                 .OnDelete(DeleteBehavior.Restrict); // Prevent circular delete issues

            entity.HasMany(u => u.Followers) // Users who are following this user
                 .WithOne(uf => uf.Following)
                 .HasForeignKey(uf => uf.FollowingId)
                 .OnDelete(DeleteBehavior.Restrict); // Prevent circular delete issues
        });

        // Post
        modelBuilder.Entity<Post>(entity =>
        {
            entity.Property(p => p.Title).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Content).IsRequired(); // No max length for rich text HTML
            entity.Property(p => p.Excerpt).HasMaxLength(500);
            entity.Property(p => p.Slug).IsRequired().HasMaxLength(250);
            entity.HasIndex(p => p.Slug).IsUnique(); // Ensure slugs are unique
            entity.Property(p => p.PublishDate).IsRequired();
            entity.Property(p => p.IsPublished).IsRequired();
            entity.Property(p => p.ViewCount).IsRequired();
            entity.Property(p => p.FeaturedImageUrl).HasMaxLength(2000); // URL length

            // One-to-Many: Post to Comments
            entity.HasMany(p => p.Comments)
                 .WithOne(c => c.Post)
                 .HasForeignKey(c => c.PostId)
                 .OnDelete(DeleteBehavior.Cascade); // If post is deleted, its comments are deleted

            // One-to-Many: Post to Likes
            entity.HasMany(p => p.Likes)
                 .WithOne(l => l.Post)
                 .HasForeignKey(l => l.PostId)
                 .OnDelete(DeleteBehavior.Cascade); // If post is deleted, its likes are deleted

            // One-to-Many: Post to SavedPosts
            entity.HasMany(p => p.SavedPosts)
                 .WithOne(sp => sp.Post)
                 .HasForeignKey(sp => sp.PostId)
                 .OnDelete(DeleteBehavior.Cascade); // If post is deleted, its saved posts are deleted
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(c => c.Slug).IsUnique(); // Ensure slugs are unique
            entity.Property(c => c.Description).HasMaxLength(500);
        });

        // Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Slug).IsRequired().HasMaxLength(50);
            entity.HasIndex(t => t.Slug).IsUnique(); // Ensure slugs are unique
        });

        // Comment (Self-referencing One-to-Many for replies)
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(c => c.Content).IsRequired().HasMaxLength(2000);
            entity.Property(c => c.CommentDate).IsRequired();
            entity.Property(c => c.IsApproved).IsRequired();

            // Self-referencing relationship for replies
            entity.HasOne(c => c.ParentComment) // A comment has one parent comment
                 .WithMany(c => c.Replies) // A parent comment can have many replies
                 .HasForeignKey(c => c.ParentCommentId)
                 .IsRequired(false) // ParentCommentId is nullable
                 .OnDelete(DeleteBehavior.Restrict); // Prevent circular delete issues
        });

        // Like
        modelBuilder.Entity<Like>(entity =>
        {
            entity.Property(l => l.LikeDate).IsRequired();
            // Composite unique key to prevent a user from liking the same post multiple times
            entity.HasIndex(l => new { l.UserId, l.PostId }).IsUnique();
        });

        // SavedPost
        modelBuilder.Entity<SavedPost>(entity =>
        {
            entity.Property(sp => sp.SavedDate).IsRequired();
            // Composite unique key to prevent a user from saving the same post multiple times
            entity.HasIndex(sp => new { sp.UserId, sp.PostId }).IsUnique();
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            entity.Property(n => n.Type).IsRequired();
            entity.Property(n => n.IsRead).IsRequired();
            entity.Property(n => n.SentDate).IsRequired();
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(200);
            entity.Property(rt => rt.Expires).IsRequired();
            entity.Property(rt => rt.Created).IsRequired();
            entity.Property(rt => rt.Revoked).IsRequired(false); // Nullable
            entity.Property(rt => rt.ReplacedByToken).HasMaxLength(200);
            entity.HasIndex(rt => rt.Token).IsUnique(); // Ensure refresh tokens are unique
        });

        // --- Join Entity Configurations (Many-to-Many) ---

        // PostCategory (Join entity for Post and Category)
        modelBuilder.Entity<PostCategory>(entity =>
        {
            // Composite primary key
            entity.HasKey(pc => new { pc.PostId, pc.CategoryId }); // [3]

            entity.HasOne(pc => pc.Post)
                 .WithMany(p => p.PostCategories)
                 .HasForeignKey(pc => pc.PostId)
                 .OnDelete(DeleteBehavior.Cascade); // If post is deleted, join entry is deleted

            entity.HasOne(pc => pc.Category)
                 .WithMany(c => c.PostCategories)
                 .HasForeignKey(pc => pc.CategoryId)
                 .OnDelete(DeleteBehavior.Cascade); // If category is deleted, join entry is deleted
        });

        // PostTag (Join entity for Post and Tag)
        modelBuilder.Entity<PostTag>(entity =>
        {
            // Composite primary key
            entity.HasKey(pt => new { pt.PostId, pt.TagId }); // [3]

            entity.HasOne(pt => pt.Post)
                 .WithMany(p => p.PostTags)
                 .HasForeignKey(pt => pt.PostId)
                 .OnDelete(DeleteBehavior.Cascade); // If post is deleted, join entry is deleted

            entity.HasOne(pt => pt.Tag)
                 .WithMany(t => t.PostTags)
                 .HasForeignKey(pt => pt.TagId)
                 .OnDelete(DeleteBehavior.Cascade); // If tag is deleted, join entry is deleted
        });

        // UserFollow (Join entity for ApplicationUser following another ApplicationUser)
        modelBuilder.Entity<UserFollow>(entity =>
        {
            // Composite primary key
            entity.HasKey(uf => new { uf.FollowerId, uf.FollowingId }); // [3]

            // Relationship for the user who is following
            entity.HasOne(uf => uf.Follower)
                 .WithMany(u => u.Following)
                 .HasForeignKey(uf => uf.FollowerId)
                 .OnDelete(DeleteBehavior.Restrict); // Prevent self-referencing cascade delete issues

            // Relationship for the user being followed
            entity.HasOne(uf => uf.Following)
                 .WithMany(u => u.Followers)
                 .HasForeignKey(uf => uf.FollowingId)
                 .OnDelete(DeleteBehavior.Restrict); // Prevent self-referencing cascade delete issues
        });
    }

    // Override SaveChanges and SaveChangesAsync to automatically set audit properties
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    // TODO: Replace "System" with actual user context (e.g., from HttpContextAccessor)
                    entry.Entity.CreatedBy = "System";
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedDate = DateTime.UtcNow;
                    // TODO: Replace "System" with actual user context (e.g., from HttpContextAccessor)
                    entry.Entity.LastModifiedBy = "System";
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    // TODO: Replace "System" with actual user context (e.g., from HttpContextAccessor)
                    entry.Entity.CreatedBy = "System";
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedDate = DateTime.UtcNow;
                    // TODO: Replace "System" with actual user context (e.g., from HttpContextAccessor)
                    entry.Entity.LastModifiedBy = "System";
                    break;
            }
        }
        return base.SaveChanges();
    }
}