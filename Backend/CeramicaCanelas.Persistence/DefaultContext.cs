using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CeramicaCanelas.Domain.Entities;
using CeramicaCanelas.Domain.Entities.Almoxarifado;
using CeramicaCanelas.Domain.Entities.Financial;

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

    // ---PARA O LIVRO CAIXA ---
    public DbSet<Launch> Launches { get; set; } = null!;
    public DbSet<LaunchCategory> LaunchCategories { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;

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

        // --- INÍCIO DAS NOVAS CONFIGURAÇÕES ---

        // Launch
        builder.Entity<Launch>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Id).HasDefaultValueSql("uuid_generate_v4()");

            // Relacionamento com LaunchCategory
            // Se a categoria for "deletada", o campo CategoryId no lançamento vira nulo
            entity.HasOne(l => l.Category)
                  .WithMany() // Categoria não tem uma lista de Lançamentos
                  .HasForeignKey(l => l.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Relacionamento com Customer
            // Se o cliente for "deletado", o campo CustomerId no lançamento vira nulo
            entity.HasOne(l => l.Customer)
                  .WithMany(c => c.Launches)
                  .HasForeignKey(l => l.CustomerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // LaunchCategory
        builder.Entity<LaunchCategory>(entity =>
        {
            entity.HasKey(lc => lc.Id);
            entity.Property(lc => lc.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        // Customer
        builder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        // --- FIM DAS NOVAS CONFIGURAÇÕES ---



        base.OnModelCreating(builder);
    }
}
