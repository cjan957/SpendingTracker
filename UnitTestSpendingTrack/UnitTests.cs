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
    public class UnitTests
    {
        public static readonly DbContextOptions<SpendingTrackContext> options = new DbContextOptionsBuilder<SpendingTrackContext>()
      .UseInMemoryDatabase(databaseName: "testDatabase")
      .Options;
        public static IConfiguration configuration = null;
        public static readonly IList<string> headingList = new List<string> { "Lunch at the Opera House", "Flight to Chiang Mai" };
        public static readonly IList<string> categoryList = new List<string> { "food", "localtransport" };
        public static readonly IList<double> costList = new List<double> { 55.89, 3200 };
        public static readonly IList<int> tripIDList = new List<int> { 1, 2 };
        public static readonly IList<string> currencyList = new List<string> { "AU$", "THB" };
        public static readonly IList<string> createdAtList = new List<string> { "", "" };
        public static readonly IList<string> receiptIDList = new List<string> { "", "" };
        public static readonly IList<string> noteList = new List<string> { "it was expensive!", "Booking Reference: BK438J" };


        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new SpendingTrackContext(options))
            {
                SpendingItem spendingItem1 = new SpendingItem()
                {
                    Heading = headingList[0],
                    Category = categoryList[0],
                    Cost = costList[0],
                    TripID = tripIDList[0],
                    Currency = currencyList[0],
                    CreatedAt = createdAtList[0],
                    ReceiptID = receiptIDList[0],
                    Note = noteList[0]
                };

                SpendingItem spendingItem2 = new SpendingItem()
                {
                    Heading = headingList[1],
                    Category = categoryList[1],
                    Cost = costList[1],
                    TripID = tripIDList[1],
                    Currency = currencyList[1],
                    CreatedAt = createdAtList[1],
                    ReceiptID = receiptIDList[1],
                    Note = noteList[1]
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

        //CREATE: test if POSTING works
        [TestMethod]
        public async Task TestAddNewItem()
        {
            using (var context = new SpendingTrackContext(options))
            {
               
                SpendingItem TestItem = new SpendingItem()
                {
                    Heading = "Lunch at Yumekaze",
                    Category = "food",
                    Cost = 65.4,
                    TripID = 3,
                    Currency = "JPY",
                    CreatedAt = "",
                    ReceiptID = "",
                    Note = "Very good sushi",
                };

                SpendingController spendingController = new SpendingController(context, configuration);
                IActionResult result = await spendingController.PostSpendingItem(TestItem) as IActionResult;

                Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));

                var checkQuery = (from m in context.SpendingItem where m.Heading == TestItem.Heading 
                                  && m.Category == TestItem.Category
                                  && m.Cost == TestItem.Cost
                                  && m.TripID == TestItem.TripID
                                  && m.Currency == TestItem.Currency
                                  && m.CreatedAt == TestItem.CreatedAt
                                  && m.ReceiptID == TestItem.ReceiptID
                                  && m.Note == TestItem.Note
                                  select m);

                SpendingItem fromMockDB = context.SpendingItem.Where(x => (x.Heading == TestItem.Heading) 
                && (x.Category == TestItem.Category) && (x.Cost == TestItem.Cost) && (x.TripID == TestItem.TripID)
                && (x.Currency == TestItem.Currency) && (x.CreatedAt == TestItem.CreatedAt) && (x.ReceiptID == TestItem.ReceiptID
                && (x.Note == TestItem.Note))).Single();

            }
        }

        //CREATE: test if api rejects unexpected item category
        [TestMethod]
        public async Task TestAddNewInvalidItem()
        {
            using (var context = new SpendingTrackContext(options))
            {

                SpendingItem TestItem = new SpendingItem()
                {
                    Heading = "Lunch at Yumekaze",
                    //what can be in the category field is restricted, see the helper function in SpendingTrack to see what's
                    //allowed to be in the category field (lunch is not permitted)
                    Category = "lunch", 
                    Cost = 65.4,
                    TripID = 3,
                    Currency = "JPY",
                    CreatedAt = "",
                    ReceiptID = "",
                    Note = "Very good sushi",
                };

                SpendingController spendingController = new SpendingController(context, configuration);
                IActionResult result = await spendingController.PostSpendingItem(TestItem) as IActionResult;

                //should return unprocessableentity
                Assert.IsInstanceOfType(result, typeof(UnprocessableEntityObjectResult));

            }
        }

        //CREATE: test if api rejects entry with 0 as cost
        [TestMethod]
        public async Task TestAddNewZeroCostItem()
        {
            using (var context = new SpendingTrackContext(options))
            {

                SpendingItem TestItem = new SpendingItem()
                {
                    Heading = "Lunch at Yumekaze",
                    //what can be in the category field is restricted, see the helper function in SpendingTrack to see what's
                    //allowed to be in the category field (lunch is not permitted)
                    Category = "food",
                    Cost = 0,
                    TripID = 3,
                    Currency = "JPY",
                    CreatedAt = "",
                    ReceiptID = "",
                    Note = "Very good sushi",
                };

                SpendingController spendingController = new SpendingController(context, configuration);
                IActionResult result = await spendingController.PostSpendingItem(TestItem) as IActionResult;

                //should return unprocessableentity
                Assert.IsInstanceOfType(result, typeof(UnprocessableEntityObjectResult));

            }
        }

        //PUT: test if put is successfull
        [TestMethod]
        public async Task TestPutItem()
        {
            using (var context = new SpendingTrackContext(options))
            {
                // Given
                SpendingItem grabItem = context.SpendingItem.Where(x => x.Heading == headingList[0]).Single();
                grabItem.Heading = "Dinner at the Opera House";
                grabItem.Cost = 145;

                // When
                SpendingController controller = new SpendingController(context, configuration);
                IActionResult result = await controller.PutSpendingItem(grabItem.ID, grabItem) as IActionResult;

                // Then check that item was updated successfully / NOCONTENT response
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NoContentResult));

                //Then check if there's only one entry in db with heading "Dinner at the Opera House"
                grabItem = context.SpendingItem.Where(x => x.Heading == "Dinner at the Opera House").Single();
            }
        }


        //PUT: test if put fails
        [TestMethod]
        public async Task TestPutInvalidItem()
        {
            using (var context = new SpendingTrackContext(options))
            {
                // Given
                SpendingItem grabItem = context.SpendingItem.Where(x => x.Heading == headingList[1]).Single();
                grabItem.Heading = "Flight to Chiang Rai";
                grabItem.Cost = 0; //cost should never be 0

                // When
                SpendingController controller = new SpendingController(context, configuration);
                IActionResult result = await controller.PutSpendingItem(grabItem.ID, grabItem) as IActionResult;

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(UnprocessableEntityResult));

            }
        }

        // READ: read the inital seed
        [TestMethod]
        public async Task TestReadingAlreadyExists()
        {
            using (var context = new SpendingTrackContext(options))
            {
                SpendingController controller = new SpendingController(context, configuration);
                IEnumerable<SpendingItem> result = controller.GetSpendingItem();

                Assert.IsNotNull(result);
                IList<SpendingItem> resultList = result.ToList();

                //Assumes same order
                Assert.IsTrue(resultList[0].Heading == headingList[0]);
                Assert.IsTrue(resultList[0].Currency == currencyList[0]);
                Assert.IsTrue(resultList[0].Cost == costList[0]);
                Assert.IsTrue(resultList[0].CreatedAt == createdAtList[0]);
                Assert.IsTrue(resultList[0].Category == categoryList[0]);
                Assert.IsTrue(resultList[0].ReceiptID == receiptIDList[0]);
                Assert.IsTrue(resultList[0].TripID == tripIDList[0]);

                Assert.IsTrue(resultList[1].Heading == headingList[1]);
                Assert.IsTrue(resultList[1].Currency == currencyList[1]);
                Assert.IsTrue(resultList[1].Cost == costList[1]);
                Assert.IsTrue(resultList[1].CreatedAt == createdAtList[1]);
                Assert.IsTrue(resultList[1].Category == categoryList[1]);
                Assert.IsTrue(resultList[1].ReceiptID == receiptIDList[1]);
                Assert.IsTrue(resultList[1].TripID == tripIDList[1]);
            }
        }

        //DELETE: GET then DELETE an item
        [TestMethod]
        public async Task TestDelete()
        {
            using (var context = new SpendingTrackContext(options))
            {
                SpendingController controller = new SpendingController(context, configuration);
                IEnumerable<SpendingItem> get_result = controller.GetSpendingItem();
                Assert.IsNotNull(get_result);
                IList<SpendingItem> resultList = get_result.ToList();
                
                IActionResult del_result = await controller.DeleteSpendingItem(resultList[0].ID) as IActionResult;
                Assert.IsInstanceOfType(del_result, typeof(OkObjectResult));

                IActionResult result_after = await controller.GetSpendingItem(resultList[0].ID) as IActionResult;
                Assert.IsInstanceOfType(result_after, typeof(NotFoundResult));
            }
        }

        [TestMethod]
        public async Task TestGetAllHeadings()
        {
            using (var context = new SpendingTrackContext(options))
            {
                SpendingController controller = new SpendingController(context, configuration);
                List<string> get_result = await controller.GetAllHeadings();

                Assert.IsTrue(get_result.All(headingList.Contains));

            }

        }

        
        // Add an item then find a sum
        [TestMethod]
        public async Task TestCostByTrip()
        {
            using (var context = new SpendingTrackContext(options))
            {
                SpendingItem TestItem = new SpendingItem()
                {
                    Heading = "Flight to Sydney",
                    Category = "flight",
                    Cost = 400,
                    TripID = 1,
                    Currency = "AU$",
                    CreatedAt = "",
                    ReceiptID = "",
                    Note = "QANTAS Flight",
                };

                SpendingController spendingController = new SpendingController(context, configuration);
                IActionResult result = await spendingController.PostSpendingItem(TestItem) as IActionResult;

                Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));

                double result_2 = await spendingController.CostByTrip(1);
                Assert.IsTrue(result_2 == 455.89);
            }
        }


    }
}
