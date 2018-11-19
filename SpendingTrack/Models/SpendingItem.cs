using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpendingTrack.Models
{
    public class SpendingItem
    {
        public int ID { get; set; }
        public int TripID { get; set; }
        public string Category { get; set; }
        public string Heading { get; set; }
        public int Cost { get; set; }
        public string Currency { get; set; }
        public string Note { get; set; }
    }
}
