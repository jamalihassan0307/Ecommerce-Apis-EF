// using Microsoft.EntityFrameworkCore;
// using Ecommerce_Apis.UserModule.Models;

// namespace Ecommerce_Apis.Data
// {
//     public class ApplicationDbContext : DbContext
//     {
//         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//             : base(options)
//         {
//         }

//         public DbSet<User> Users { get; set; }

//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             modelBuilder.Entity<User>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
                
//                 entity.HasIndex(e => e.Email)
//                     .IsUnique();

//                 entity.Property(e => e.FullName)
//                     .IsRequired()
//                     .HasMaxLength(100);

//                 entity.Property(e => e.Email)
//                     .IsRequired()
//                     .HasMaxLength(100);

//                 entity.Property(e => e.PhoneNumber)
//                     .HasMaxLength(20);

//                 entity.Property(e => e.Image)
//                     .HasMaxLength(255);

//                 entity.Property(e => e.RoleId)
//                     .IsRequired();
//             });
//         }
//     }
// } 