using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace MyApp.Namespace
{
    public class FriendsListModel : PageModel
    {
        private readonly IFriendsService _service;
        public string City { get; set; }
        public string Country { get; set; }

        public int TotalNumberOfFriends { get; set; }

        public List<IFriend> AllFriends { get; set; }

        public List<IFriend> FriendsPerPage { get; set; }

        //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 10;

        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;


        public async Task<IActionResult> OnGet(string city, string country, int nrOfFriends)
        {
            if (int.TryParse(Request.Query["pagenr"], out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }


            Country = country;
            City = city;
            TotalNumberOfFriends = nrOfFriends;

            var allAddresses = await _service.ReadAddressesAsync(true, false, City, 0, TotalNumberOfFriends);
            AllFriends = allAddresses.PageItems.SelectMany(a => a.Friends).ToList();

            NrOfPages = (int)Math.Ceiling((double)AllFriends.Count/PageSize);
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(NrOfPages - 1, ThisPageNr + 1);
            PresentPages = Math.Min(3, NrOfPages);

            FriendsPerPage = AllFriends.Skip(ThisPageNr*PageSize).Take(PageSize).ToList<IFriend>();
            

            return Page();
        }
        public FriendsListModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
