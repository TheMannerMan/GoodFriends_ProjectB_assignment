using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using AppGoodFriendsMVC.Models;

namespace AppGoodFriendsMVC.Controllers
{
    public class FriendsListController : Controller
    {
        readonly IFriendsService _service;

        public FriendsListController(IFriendsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search(int totalNumberOfFriends, string country, string city)
        {
            var vwm = new SearchViewModel()
            {
                TotalNumberOfFriends = totalNumberOfFriends,
                Country = country,
                City = city
            };
            await vwm.LoadFriendsAsync(_service);
            vwm.UpdatePagination();
            return View("Search", vwm);
        }

        [HttpPost]
        public async Task<IActionResult> Find(SearchViewModel vm)
        {
            vm.ThisPageNr = 0;
            await vm.LoadFriendsAsync(_service);
            vm.UpdatePagination();
            return View("Search", vm);
        }

    }
}