using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendingTrack.Models;

namespace SpendingTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpendingController : ControllerBase
    {
        private readonly SpendingTrackContext _context;

        public SpendingController(SpendingTrackContext context)
        {
            _context = context;
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

            _context.SpendingItem.Add(spendingItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpendingItem", new { id = spendingItem.ID }, spendingItem);
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
    }
}