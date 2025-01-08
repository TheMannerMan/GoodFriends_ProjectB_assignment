using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace AppGoodFriendsMVC.Models
{
    public class SearchViewModel
    {
        [BindProperty(SupportsGet = true)]
        public string City { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Country { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TotalNumberOfFriends { get; set; }

        public List<IFriend> AllFriends { get; set; }
        public List<IFriend> FriendsPerPage { get; set; }

        //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 10;

        [BindProperty]
        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;

        [BindProperty]
        public string SearchFilter { get; set; }

        public void UpdatePagination()
        {
            NrOfPages = (int)Math.Ceiling((double)AllFriends.Count / PageSize);
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(NrOfPages - 1, ThisPageNr + 1);
            //PresentPages = Math.Min(3, NrOfPages);
            FriendsPerPage = AllFriends.Skip(ThisPageNr * PageSize).Take(PageSize).ToList<IFriend>();
        }

        public async Task LoadFriendsAsync(IFriendsService _service)
        {
            global::Models.DTO.ResponsePageDto<IAddress> allAddresses;

            if (TotalNumberOfFriends == 0) // The default parameter value for TotalNumberOfFriends is 0. To catch all addresses the following logic is used.
            {
                allAddresses = await _service.ReadAddressesAsync(true, false, City, 0, 100);

                if (allAddresses.DbItemsCount > 100)
                {
                    allAddresses = await _service.ReadAddressesAsync(true, false, City, 0, allAddresses.DbItemsCount);
                }
            }

            else
            {
                allAddresses = await _service.ReadAddressesAsync(true, false, City, 0, TotalNumberOfFriends);
            }


            AllFriends = allAddresses.PageItems.SelectMany(a => a.Friends).ToList();

            if (!string.IsNullOrWhiteSpace(SearchFilter))
            {
                AllFriends = AllFriends
                    .Where(f => f.FirstName.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase) ||
                                f.LastName.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase) ||
                                (f.Address != null &&
                                    (f.Address.StreetAddress.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase) ||
                                     f.Address.ZipCode.ToString().Contains(SearchFilter, StringComparison.OrdinalIgnoreCase))))
                    .ToList();
            }
        }

        public SearchViewModel()
        {

        }
    }
}