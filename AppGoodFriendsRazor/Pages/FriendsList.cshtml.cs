using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Models;
using Services;

namespace MyApp.Namespace
{
    public class FriendsListModel : PageModel
    {
        private readonly IFriendsService _service;

        [BindProperty(SupportsGet = true)]
        public string City { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Country { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TotalNumberOfFriends { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchFilter { get; set; }

        public List<IFriend> AllFriends { get; set; }

        public List<IFriend> FriendsPerPage { get; set; }

        //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 10;

        [BindProperty(SupportsGet = true)]
        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;


        public async Task<IActionResult> OnGet()
        {

            await LoadFriendsAsync();
            UpdatePagination();
            return Page();
        }


        public async Task<IActionResult> OnPostSearch()
        {
            ThisPageNr = 0; // Reset pagination for a new search
            await LoadFriendsAsync();
            UpdatePagination();
            return Page();
        }

        private async Task LoadFriendsAsync()
        {
            var allAddresses = await _service.ReadAddressesAsync(true, false, City, 0, TotalNumberOfFriends);
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
        private void UpdatePagination()
        {
            NrOfPages = (int)Math.Ceiling((double)AllFriends.Count / PageSize);
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(NrOfPages - 1, ThisPageNr + 1);
            //PresentPages = Math.Min(3, NrOfPages);
            FriendsPerPage = AllFriends.Skip(ThisPageNr * PageSize).Take(PageSize).ToList<IFriend>();
        }
        public FriendsListModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
