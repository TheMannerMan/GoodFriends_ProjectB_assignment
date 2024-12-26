using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace MyApp.Namespace
{
    public class FriendDetailsModel : PageModel
    {
        private readonly IFriendsService _service;
        public string? ErrorMessage { get; set; } = null;
        public IFriend Friend { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Read a QueryParameter
                Guid _id = Guid.Parse(Request.Query["id"]);

                //Use the Service
                Friend = await _service.ReadFriendAsync(_id, false);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return Page();
        }
    
    public FriendDetailsModel(IFriendsService service)
        {
            _service = service;
        }
    }
}
