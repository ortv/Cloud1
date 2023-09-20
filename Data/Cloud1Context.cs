﻿using System;
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

        public DbSet<Cloud1.Models.Order>? Order { get; set; }
    }
}
