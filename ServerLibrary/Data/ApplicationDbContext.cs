using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Client> Clients { get; set; }
        
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleMedia> ArticleMedias { get; set; }
        
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        
        public DbSet<QuoteRequest> QuoteRequests { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<SavedSearch> SavedSearches { get; set; }

        // New entities for contact and review system
        public DbSet<ContactRequest> ContactRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration pour s'assurer que les prix utilisent bien le type decimal
            modelBuilder.Entity<SubscriptionPlan>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<Article>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

             // Relation One-to-One User <-> Supplier
             modelBuilder.Entity<User>()
                .HasOne(u => u.Supplier)
                .WithOne(s => s.User)
                .HasForeignKey<Supplier>(s => s.UserId)
                .IsRequired(false) // Un user peut ne pas être un supplier
                .OnDelete(DeleteBehavior.Restrict);

             // Relation One-to-One User <-> Client
             modelBuilder.Entity<User>()
                .HasOne(u => u.Client)
                .WithOne(c => c.User)
                .HasForeignKey<Client>(c => c.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // ContactRequest relationships
            modelBuilder.Entity<ContactRequest>()
                .HasOne(cr => cr.Client)
                .WithMany(c => c.ContactRequests)
                .HasForeignKey(cr => cr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContactRequest>()
                .HasOne(cr => cr.Supplier)
                .WithMany(s => s.ContactRequests)
                .HasForeignKey(cr => cr.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContactRequest>()
                .HasOne(cr => cr.Article)
                .WithMany()
                .HasForeignKey(cr => cr.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.ContactRequest)
                .WithMany(cr => cr.Reviews)
                .HasForeignKey(r => r.ContactRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ReviewerUser)
                .WithMany()
                .HasForeignKey(r => r.ReviewerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ReviewedUser)
                .WithMany()
                .HasForeignKey(r => r.ReviewedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration globale des index pour PublicId (Sécurité)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(nameof(BaseEntity.PublicId))
                        .IsUnique();
                }
            }
        }
    }
}
