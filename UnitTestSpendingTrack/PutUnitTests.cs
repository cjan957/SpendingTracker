using SpendingTrack.Controllers;
using SpendingTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace UnitTestSpendingTrack
{
    [TestClass]
    public class PutUnitTests
    {
        public static readonly DbContextOptions<SpendingTrackContext> options = new DbContextOptionsBuilder<SpendingTrackContext>()
      .UseInMemoryDatabase(databaseName: "testDatabase")
      .Options;
        public static IConfiguration configuration = null;
        public static readonly IList<string> memeTitles = new List<string> { "dankMeme", "dankerMeme" };

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new SpendingTrackContext(options))
            {
                SpendingItem spendingItem1 = new SpendingItem()
                {
                    Heading = memeTitles[0]
                };

                SpendingItem spendingItem2 = new SpendingItem()
                {
                    Heading = memeTitles[1]
                };

                context.SpendingItem.Add(spendingItem1);
                context.SpendingItem.Add(spendingItem2);
                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new SpendingTrackContext(options))
            {
                context.SpendingItem.RemoveRange(context.SpendingItem);
                context.SaveChanges();
            };
        }
    }
}
