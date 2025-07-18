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

        // Configure the primary key for the User entity
        builder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        // Configure the primary key and relationships for the Products entity
        builder.Entity<Products>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasDefaultValueSql("uuid_generate_v4()");
            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure the primary key for the Categories entity
        builder.Entity<Categories>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        // Configure the primary key for the Employees entity
        builder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        // Configure ProductEntry relationships
        builder.Entity<ProductEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");

            entity.HasOne(e => e.Product)
                  .WithMany() // ou .WithMany(p => p.Entries) se você tiver essa navegação
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany() // ou .WithMany(u => u.ProductEntries)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.ProductEntries)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure ProductExit relationships
        builder.Entity<ProductExit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");

            entity.HasOne(e => e.Product)
                  .WithMany() // ou .WithMany(p => p.Exits)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Employee)
                  .WithMany() // ou .WithMany(emp => emp.Exits)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                    .WithMany() // ou .WithMany(u => u.ProductExits)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);


        });

        builder.Entity<Supplier>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
        });

        base.OnModelCreating(builder);
    }

}