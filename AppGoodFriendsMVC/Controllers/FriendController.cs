using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using AppGoodFriendsMVC.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection.Metadata.Ecma335;
using Models;
using Models.DTO;
using System.Runtime.InteropServices;

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

        [HttpPost]
        public async Task<IActionResult> Undo(EditFriendViewModel vm)
        {
            vm.FriendInputModel = new FriendIM(await _service.ReadFriendAsync(vm.FriendInputModel.FriendId, false));
            return View("EditFriend", vm);
        }
        [HttpPost]
        public async Task<IActionResult> Save(EditFriendViewModel vm)
        {
            if (!IsValid(vm))
            {
                //The page is not valid
                return View("EditFriend", vm);
            }
            else
            {
                IFriend model = await _service.ReadFriendAsync(vm.FriendInputModel.FriendId, false);
                model = vm.FriendInputModel.UpdateModel(model);


                IAddress relevantAddress = await UpdateAddress(vm, model.Address); // kan man skicka in id enbart?
                var dtoFriend = new FriendCUdto(model);
                dtoFriend.AddressId = relevantAddress.AddressId;
                
                model = await _service.UpdateFriendAsync(dtoFriend);
                return RedirectToAction("Index", new {id = model.FriendId});
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeletePet(Guid id, FriendViewModel vm)
        {

            await _service.DeletePetAsync(id);
            vm.Friend = await _service.ReadFriendAsync(vm.FriendId, false);

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuote(Guid id, FriendViewModel vm)
        {


            await _service.DeleteQuoteAsync(id);
            vm.Friend = await _service.ReadFriendAsync(vm.FriendId, false);


            return View("Index", vm);
        }

        private bool IsValid(EditFriendViewModel vm, string[] validateOnlyKeys = null)
        {
            vm.InvalidKeys = ModelState
                .Where(s => s.Value.ValidationState == ModelValidationState.Invalid);

            if (validateOnlyKeys != null)
            {
                vm.InvalidKeys = vm.InvalidKeys.Where(s => validateOnlyKeys.Any(vk => vk == s.Key));
            }

            vm.ValidationErrorMsgs = vm.InvalidKeys.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
            vm.HasValidationErrors = vm.InvalidKeys.Any();

            return !vm.HasValidationErrors;
        }

        public async Task<IAddress> UpdateAddress(EditFriendViewModel vm, IAddress address)
        {
            IAddress test;
            var tempAddress = new AddressIM(address);

            // If no changes have been done, then they are equal.
            bool isEqual = tempAddress.Equals(vm.FriendInputModel.AddressInputModel);
            if (isEqual){
                return address;
            }
            int _pageSize = 1000;
            var resultOfFilteredAddresses = await _service.ReadAddressesAsync(true, false, vm.FriendInputModel.AddressInputModel.City, 0, _pageSize);

            // If pagesize is smaller the DbItemsCount, it means that not all addresses were loaded from the service. We fix it with a call on the service with the exact amount of objects.
            if (resultOfFilteredAddresses.DbItemsCount > _pageSize)
            {
                _pageSize = resultOfFilteredAddresses.DbItemsCount;
                resultOfFilteredAddresses = await _service.ReadAddressesAsync(true, false, vm.FriendInputModel.AddressInputModel.City, _pageSize, 0);
            }

            // If the entered address exist in database, return it. Otherwise create a new address.
            var returnedItem = resultOfFilteredAddresses.PageItems.FirstOrDefault
            (a => a.Country == vm.FriendInputModel.AddressInputModel.Country && a.ZipCode == vm.FriendInputModel.AddressInputModel.ZipCode && a.StreetAddress == vm.FriendInputModel.AddressInputModel.StreetAddress);

            if (returnedItem != null)
            {
                return returnedItem;
            }

            else
            {

                IAddress newAddress = vm.FriendInputModel.AddressInputModel.CreateNewAddress(address);
                var dtoAddress = new AddressCUdto(newAddress);
                dtoAddress.AddressId = null;
                IAddress testtest = await _service.CreateAddressAsync(dtoAddress);
                return testtest;

            }
        }

    }

}