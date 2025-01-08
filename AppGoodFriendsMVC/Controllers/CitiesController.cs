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
    
    public class CitiesController : Controller
    {
        private readonly IFriendsService _service;

        public CitiesController(IFriendsService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string country)
        {

            var dbInfo = await _service.InfoAsync;
            var vm = new CitiesViewModel();
            vm.StatsPerCity = dbInfo.Friends
                .Where(f => f.Country == country && f.City is not null)
                .ToDictionary(
                    f => f.City,
                    f => (
                        FriendsCount: f.NrFriends,
                        PetsCount: dbInfo.Pets
                            .Where(p => p.Country == country && p.City == f.City)
                            .Sum(p => p.NrPets)
                    )
                );
                vm.Country = country;

            //ViewData["Country"] = country;
            return View(vm);
        }
    }
}