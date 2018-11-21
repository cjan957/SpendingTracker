using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendingTrack.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using SpendingTrack.Models;

namespace SpendingTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpendingController : ControllerBase
    {
        private readonly SpendingTrackContext _context;
        private IConfiguration _configuration;


        public SpendingController(SpendingTrackContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Spending
        [HttpGet]
        public IEnumerable<SpendingItem> GetSpendingItem()
        {
            return _context.SpendingItem;
        }

        // GET: api/Spending/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpendingItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var spendingItem = await _context.SpendingItem.FindAsync(id);

            if (spendingItem == null)
            {
                return NotFound();
            }

            return Ok(spendingItem);
        }

        [HttpGet("costbytrip/{id}")]
        public async Task<double> CostByTrip([FromRoute] int id)
        {
            var allSpendings = (from m in _context.SpendingItem where m.TripID == id select m.Cost);
            if (allSpendings == null)
            {
                return -1;
            }

            var listOfSpendings = await allSpendings.ToListAsync();
            return listOfSpendings.Sum();
        }

        [HttpGet("spendinglistbytrip/{id}")]
        public async Task<IActionResult> GetSpendingListByTrip([FromRoute] int id)
        {
            var allSpendings = (from m in _context.SpendingItem where m.TripID == id select m);
            if (allSpendings == null)
            {
                return NotFound();
            }

            var listOfSpendings = await allSpendings.ToListAsync();
            return Ok(listOfSpendings);
        }

        // PUT: api/Spending/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpendingItem([FromRoute] int id, [FromBody] SpendingItem spendingItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != spendingItem.ID)
            {
                return BadRequest();
            }


            if (!SpendingItemHelper.ValidCategory(spendingItem.Category) || spendingItem.Heading == null
               || spendingItem.Cost == 0 || spendingItem.Currency == null)
            {
                return UnprocessableEntity();
            }

            _context.Entry(spendingItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpendingItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Spending
        [HttpPost]
        public async Task<IActionResult> PostSpendingItem([FromBody] SpendingItem spendingItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Do sanity check before posting
            if (SpendingItemHelper.ValidCategory(spendingItem.Category) && spendingItem.Heading != null
                && spendingItem.Cost != 0 && spendingItem.Currency != null)
            {
                _context.SpendingItem.Add(spendingItem);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetSpendingItem", new { id = spendingItem.ID }, spendingItem);
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }



        // DELETE: api/Spending/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpendingItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var spendingItem = await _context.SpendingItem.FindAsync(id);
            if (spendingItem == null)
            {
                return NotFound();
            }

            _context.SpendingItem.Remove(spendingItem);
            await _context.SaveChangesAsync();

            return Ok(spendingItem);
        }

        private bool SpendingItemExists(int id)
        {
            return _context.SpendingItem.Any(e => e.ID == id);
        }

        // GET: api/Spending/Headings
        [Route("headings")]
        [HttpGet]
        public async Task<List<string>> GetAllHeadings()
        {
            var spending = (from m in _context.SpendingItem select m.Heading).Distinct();
            var list = await spending.ToListAsync();

            return list;
        }

        [HttpPost, Route("upload")]
        public async Task<IActionResult> UploadFile([FromForm] ReceiptImageItem receipt)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a  multipart request, but got {Request.ContentType}");
            }
            try
            {
                using (var stream = receipt.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(receipt.Image.FileName, null, stream);
                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file, please try again");

                    }
                    SpendingItem spendingItem = new SpendingItem();
                    spendingItem.Heading = receipt.Title;
                    spendingItem.TripID = receipt.TripID;
                    spendingItem.ID = receipt.ItemID;

                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

                    _context.SpendingItem.Add(spendingItem); //add to db
                    await _context.SaveChangesAsync();

                    return Ok($"File: {receipt.Title} has successfully uploaded");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }
        }

        private async Task<CloudBlockBlob> UploadToBlob(string filename, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {
            var accountName = _configuration["AzureBlob:name"];
            var accountKey = _configuration["AzureBlob:key"]; ;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("images");

            string storageConnectionString = _configuration["AzureBlob:connectionString"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Generate a new filename for every new blob
                    var fileName = Guid.NewGuid().ToString();
                    fileName += GetFileExtention(filename);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = imagesContainer.GetBlockBlobReference(fileName);

                    if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return new CloudBlockBlob(new Uri(""));
                    }

                    return cloudBlockBlob;
                }
                catch (StorageException ex)
                {
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                return new CloudBlockBlob(new Uri(""));
            }
        }

        private string GetFileExtention(string fileName)
        {
            if (!fileName.Contains("."))
                return ""; //no extension
            else
            {
                var extentionList = fileName.Split('.');
                return "." + extentionList.Last(); //assumes last item is the extension 
            }
        }
    }
}
