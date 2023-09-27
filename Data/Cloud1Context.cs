using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cloud1.Models;

namespace Cloud1.Data
{
    public class Cloud1Context : DbContext
    {
        public Cloud1Context (DbContextOptions<Cloud1Context> options)
            : base(options)
        {
        }

        public DbSet<Cloud1.Models.IceCream> IceCream { get; set; } = default!;

        //public DbSet<Cloud1.Models.Order>? Order { get; set; }

        public DbSet<Cloud1.Models.IceCream1>? IceCream1 { get; set; }

        //public DbSet<Cloud1.Models.Order>? Order { get; set; }

     //   public DbSet<Cloud1.Models.CartItem>? CartItem { get; set; }

        //public DbSet<Cloud1.Models.Order>? Order { get; set; }

     //   public DbSet<Cloud1.Models.ShoppingCart>? ShoppingCart { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<CartItem>()
        //        .HasOne(ci => ci.ShoppingCart)
        //        .WithMany(sc => sc.shoppingCartItems)
        //        .HasForeignKey(ci => ci.ShoppingCartId);
        //}
    }
}
