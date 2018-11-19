using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SpendingTrack.Models
{
    public class ReceiptImageItem
    {
        public string Title { get; set; }
        public int TripID { get; set; }
        public int ItemID { get; set; }
        public IFormFile Image { get; set; }
    }
}
