using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace MyApp.Namespace
{
    public class CitiesModel : PageModel
    {

        readonly IFriendsService _service;

        public Dictionary<string, (int FriendsCount, int PetsCount)> StatsPerCity { get; set; }

        public string Country { get; set; }
        public async Task<IActionResult> OnGet(string country)
        {
            Country = country;
            var dbInfo = await _service.InfoAsync;

            StatsPerCity = dbInfo.Friends
            .Where(f => f.Country == Country && f.City is not null)
            .ToDictionary(
                f => f.City,
                f => (
                    FriendsCount: f.NrFriends,
                    PetsCount: dbInfo.Pets
                        .Where(p => p.Country == Country && p.City == f.City)
                        .Sum(p => p.NrPets)
                )
            );

            return Page();

        }
        public CitiesModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
