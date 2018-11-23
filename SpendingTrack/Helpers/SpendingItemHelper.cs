using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpendingTrack.Helpers
{
    public static class SpendingItemHelper
    {
        public static bool ValidCategory(string category)
        {
            string lowerCategory = category.ToLower();
            if (lowerCategory == "accomodation" || lowerCategory == "attractionfee"
                   || lowerCategory == "flight" || lowerCategory == "food"
                   || lowerCategory == "grocery" || lowerCategory == "localtransport"
                   || lowerCategory == "phonedata" || lowerCategory == "shopping"
                   || lowerCategory == "souvenir" || lowerCategory == "other")
            {
                return true;
            }
            return false;
        }
    }
}
