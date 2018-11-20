using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpendingTrack.Models
{
    public class TripItem
    {
        public int ID { get; set; } //tripID
        public string Country { get; set; }
        public string Currency { get; set; }
        public string Starts { get; set; }
        public string Ends { get; set; }
        public double Budget { get; set; }
    }
}
