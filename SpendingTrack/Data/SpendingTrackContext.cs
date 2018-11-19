using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SpendingTrack.Models
{
    public class SpendingTrackContext : DbContext
    {
        public SpendingTrackContext (DbContextOptions<SpendingTrackContext> options)
            : base(options)
        {
        }

        public DbSet<SpendingTrack.Models.SpendingItem> SpendingItem { get; set; }
    }
}
