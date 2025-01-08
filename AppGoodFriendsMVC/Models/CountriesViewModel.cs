using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppGoodFriendsMVC.Models
{
   public class CountriesViewModel
    {
        public Dictionary<string, int> FriendsPerCountry { get; set; }
        public List<string> Countries { get; set; }
    }
}