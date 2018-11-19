using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpendingTrack.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SpendingTrackContext(
                serviceProvider.GetRequiredService<DbContextOptions<SpendingTrackContext>>()))
            {
                // Look for any movies.
                if (context.SpendingItem.Count() > 0)
                {
                    return;   // DB has been seeded
                }

                context.SpendingItem.AddRange(
                    new SpendingItem
                    {
                        TripID = 0,
                        Category = "Transport",
                        Heading = "SEED",
                        Cost = 0.1,
                        Currency = "SEEd",
                        Note = "This is a seed",
                        ReceiptID = "",
                        CreatedAt = ""
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
