using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace MyApp.Namespace
{
    public class FriendDetailsModel : PageModel
    {
        private readonly IFriendsService _service;
        //public string? ErrorMessage { get; set; } = null; //Use in Error handling if implemented in future

        public IFriend Friend { get; set; }

        [BindProperty]
        public Guid FriendId { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Read a QueryParameter
                Guid _id = Guid.Parse(Request.Query["id"]);

                //Use the Service
                Friend = await _service.ReadFriendAsync(_id, false);
                FriendId = Friend.FriendId;
            }
            catch (Exception e)
            {
                // ErrorMessage = e.Message; //Use in Error handling if implemented in future
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeletePet(Guid id)
        {

            //Set the Quote as deleted, it will not be rendered

            try
            {
                await _service.DeletePetAsync(id);
                Friend = await _service.ReadFriendAsync(FriendId, false);
            }
            catch (Exception e)
            {
                //ErrorMessage = e.Message; //Use in Error handling if implemented in future
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteQuote(Guid id)
        {

            //Set the Quote as deleted, it will not be rendered

            try
            {
                await _service.DeleteQuoteAsync(id);
                Friend = await _service.ReadFriendAsync(FriendId, false);
            }
            catch (Exception e)
            {
                // ErrorMessage = e.Message; //Use in Error handling if implemented in future
            }
            return Page();
        }


        public FriendDetailsModel(IFriendsService service)
        {
            _service = service;
        }

    }

}
