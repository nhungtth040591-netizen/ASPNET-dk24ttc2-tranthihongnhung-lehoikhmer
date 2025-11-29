using Microsoft.EntityFrameworkCore;

namespace KhmerFestival.Web.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ===== DbSet =====
        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Festival> Festivals { get; set; }
        // Đã xóa DbSet<FestivalMedia>

        public DbSet<Category> Categories { get; set; }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Role =====
            modelBuilder.Entity<Role>(b =>
            {
                b.ToTable("Roles");
                b.HasKey(r => r.RoleId);
            });

            // ===== Account =====
            modelBuilder.Entity<Account>(b =>
            {
                b.ToTable("Accounts");
                b.HasKey(a => a.AccountId);

                b.HasIndex(a => a.Email)
                  .IsUnique();
            });

            // ===== AccountRole (many-to-many Account - Role) =====
            modelBuilder.Entity<AccountRole>(b =>
            {
                b.ToTable("AccountRoles");

                b.HasKey(ar => new { ar.AccountId, ar.RoleId });

                b.HasOne(ar => ar.Account)
                  .WithMany(a => a.AccountRoles)
                  .HasForeignKey(ar => ar.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(ar => ar.Role)
                  .WithMany(r => r.AccountRoles)
                  .HasForeignKey(ar => ar.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== SystemConfig =====
            modelBuilder.Entity<SystemConfig>(b =>
            {
                b.ToTable("SystemConfigs");
                b.HasKey(sc => sc.ConfigKey);
            });

            // ===== Location (self reference) =====
            modelBuilder.Entity<Location>(b =>
            {
                b.ToTable("Locations");
                b.HasKey(l => l.LocationId);

                b.HasOne(l => l.Parent)
                  .WithMany(p => p.Children)
                  .HasForeignKey(l => l.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Festival =====
            modelBuilder.Entity<Festival>(b =>
            {
                b.ToTable("Festivals");
                b.HasKey(f => f.FestivalId);

                b.HasIndex(f => f.Slug)
                  .IsUnique();

                b.HasOne(f => f.Location)
                  .WithMany(l => l.Festivals)
                  .HasForeignKey(f => f.LocationId)
                  .OnDelete(DeleteBehavior.SetNull);

                // Đã xóa quan hệ HasMany(Media) tại đây

                b.HasMany(f => f.Articles)
                  .WithOne(a => a.Festival)
                  .HasForeignKey(a => a.FestivalId)
                  .OnDelete(DeleteBehavior.SetNull);
            });

            // Đã xóa block cấu hình FestivalMedia

            // ===== Category (self reference) =====
            modelBuilder.Entity<Category>(b =>
            {
                b.ToTable("Categories");
                b.HasKey(c => c.CategoryId);

                b.HasIndex(c => c.Slug)
                  .IsUnique();

                b.HasOne(c => c.ParentCategory)
                  .WithMany(p => p.Children)
                  .HasForeignKey(c => c.ParentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Article =====
            modelBuilder.Entity<Article>(b =>
            {
                b.ToTable("Articles");
                b.HasKey(a => a.ArticleId);

                b.HasIndex(a => a.Slug)
                  .IsUnique();

                b.HasOne(a => a.Category)
                  .WithMany(c => c.Articles)
                  .HasForeignKey(a => a.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(a => a.Festival)
                  .WithMany(f => f.Articles)
                  .HasForeignKey(a => a.FestivalId)
                  .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(a => a.Author)
                  .WithMany()
                  .HasForeignKey(a => a.AuthorId)
                  .OnDelete(DeleteBehavior.SetNull);

                b.HasMany(a => a.Comments)
                  .WithOne(c => c.Article)
                  .HasForeignKey(c => c.ArticleId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== Comment =====
            modelBuilder.Entity<Comment>(b =>
            {
                b.ToTable("Comments");
                b.HasKey(c => c.CommentId);

                b.HasOne(c => c.Article)
                  .WithMany(a => a.Comments)
                  .HasForeignKey(c => c.ArticleId)
                  .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(c => c.Account)
                  .WithMany()
                  .HasForeignKey(c => c.AccountId)
                  .OnDelete(DeleteBehavior.SetNull);
            });

            // ===== Contact =====
            modelBuilder.Entity<Contact>(b =>
            {
                b.ToTable("Contacts");
                b.HasKey(c => c.ContactId);

                b.HasIndex(c => c.Status);
            });
        }
    }
}