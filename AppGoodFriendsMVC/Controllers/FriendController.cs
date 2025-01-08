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
    public class FriendController : Controller
    {
        readonly IFriendsService _service;

        public FriendController(IFriendsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {
            var vm = new FriendViewModel();
            vm.Friend = await _service.ReadFriendAsync(id, false);
            vm.FriendId = id;
            
            return View(vm);
        }

        public async Task<IActionResult> EditFriend(Guid id)
        {
            var vm = new EditFriendViewModel();
            vm.FriendInputModel = new FriendIM(await _service.ReadFriendAsync(id, false));
            
            
            return View("EditFriend", vm);
        }

        public async Task<IActionResult> Undo(EditFriendViewModel vm)
        {
            vm.FriendInputModel = new FriendIM(await _service.ReadFriendAsync(vm.FriendInputModel.FriendId, false));
            return View("EditFriend", vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePet(Guid id, FriendViewModel vm)
        {

                await _service.DeletePetAsync(id);
                vm.Friend = await _service.ReadFriendAsync(vm.FriendId, false);
            
            return View("Index",vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuote(Guid id, FriendViewModel vm)
        {


                await _service.DeleteQuoteAsync(id);
                vm.Friend = await _service.ReadFriendAsync(vm.FriendId, false);


            return View("Index",vm);
        }

    }

}