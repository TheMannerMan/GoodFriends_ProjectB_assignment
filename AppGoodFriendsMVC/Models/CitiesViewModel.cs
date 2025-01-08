using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppGoodFriendsMVC.Models
{
    public class CitiesViewModel
    {
        public Dictionary<string, (int FriendsCount, int PetsCount)> StatsPerCity { get; set; }

        public string Country { get; set; }
    }
}