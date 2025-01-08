using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppGoodFriendsMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace AppGoodFriendsMVC.Controllers
{
    
    public class CountriesController : Controller
    {
        private readonly IFriendsService _service;

        public CountriesController(IFriendsService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var dbInfo = await _service.InfoAsync;

            var friendsPerCountry = dbInfo.Friends
                .Select(f => f.Country)
                .Where(country => !string.IsNullOrEmpty(country))
                .Distinct()
                .ToDictionary(country => country, _ => 0);

            foreach (var friend in dbInfo.Friends)
            {
                if (!string.IsNullOrEmpty(friend.Country) && friendsPerCountry.ContainsKey(friend.Country))
                {
                    friendsPerCountry[friend.Country] += friend.NrFriends;
                }
            }

            var model = new CountriesViewModel
            {
                FriendsPerCountry = friendsPerCountry,
                Countries = friendsPerCountry.Keys.ToList()
            };

            return View(model);
        }
    }
}