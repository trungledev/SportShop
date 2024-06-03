using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SportShop.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProducerProduct> Producers { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<StatusProduct> Statuses { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Relation of User
        // User 1 - Many Address
        builder.Entity<ApplicationUser>()
            .HasMany<Address>(p=>p.Address)
            .WithOne (s => s.ApplicationUser);
        //Ralation 1 Producer - 1 Address 
        builder.Entity<ProducerProduct>()
           .HasMany<Address>(p => p.Address)
           .WithOne (s => s.ProducerProduct);
        //Relation of Product
        // Producer 1 - Many Product
        builder.Entity<ProducerProduct>()
            .HasMany(p => p.Products)
            .WithOne(i => i.Producer);

        //Type 1 - Many Product
        builder.Entity<Category>()
            .HasMany(p => p.Products)
            .WithOne(i => i.Category);

        //Status 1 - Many Product
        builder.Entity<StatusProduct>()
            .HasMany(p => p.Products)
            .WithOne(i => i.Status);
        
        //User Many - Cart - Many Product
        builder.Entity<Cart>()
            .HasKey(t=> new{ t.UserId, t.ProductId});

        //User Many - Review - Many Product
        builder.Entity<Review>()
            .HasKey(t=> new{ t.UserId, t.ProductId});
    }
}
