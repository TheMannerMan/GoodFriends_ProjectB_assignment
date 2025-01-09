using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace MyApp.Namespace
{
    public class CountriesModel : PageModel
    {
        readonly IFriendsService _service;

        public Dictionary<string, int> FriendsPerCountry { get; set; } = [];
        public List<string> Countries { get; set;} = new List<string>();


        public async Task<IActionResult> OnGet()
        {
            var dbInfo = await _service.InfoAsync;

            FriendsPerCountry = dbInfo.Friends
                .Select(f => f.Country)
                .Where(country => !string.IsNullOrEmpty(country))
                .Distinct()
                .ToDictionary(country => country, _ => 0);

            Countries = dbInfo.Friends.Select(f => f.Country)
                .Where(country => !string.IsNullOrEmpty(country))
                .Distinct()
                .ToList();


            foreach (var friend in dbInfo.Friends)
            {
                if (!string.IsNullOrEmpty(friend.Country) && FriendsPerCountry.ContainsKey(friend.Country))
                {
                    FriendsPerCountry[friend.Country] += friend.NrFriends;
                }
            }

            return Page();
        }

        public CountriesModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
