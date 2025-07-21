using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CeramicaCanelas.Domain.Entities;

namespace CeramicaCanelas.Persistence;

public class DefaultContext : IdentityDbContext<User>
{
    public DefaultContext() { }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options) { }

    public DbSet<Products> Products { get; set; } = null!;
    public DbSet<Categories> Categories { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<ProductExit> ProductExits { get; set; } = null!;
    public DbSet<ProductEntry> ProductEntries { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("uuid-ossp");
        builder.ApplyConfigurationsFromAssembly(typeof(DefaultContext).Assembly);

        // User
        builder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        // Products
        builder.Entity<Products>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasDefaultValueSql("uuid_generate_v4()");
            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull); // mantém produtos mesmo sem categoria
        });

        // Categories
        builder.Entity<Categories>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        // Employees
        builder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        // ProductEntry
        builder.Entity<ProductEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.SetNull); // mantém entrada mesmo se produto for deletado

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull); // mantém entrada mesmo se user for deletado

            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.ProductEntries)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ProductExit
        builder.Entity<ProductExit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.SetNull); // mantém saída mesmo sem produto

            entity.HasOne(e => e.Employee)
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.SetNull); // mantém saída mesmo sem funcionário

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Supplier
        builder.Entity<Supplier>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
        });

        base.OnModelCreating(builder);
    }
}
