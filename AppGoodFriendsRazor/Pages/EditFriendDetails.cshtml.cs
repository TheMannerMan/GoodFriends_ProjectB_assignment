using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using Models.DTO;
using Services;

namespace MyApp.Namespace
{
    public class EditFriendDetailsModel : PageModel
    {
        public readonly IFriendsService _service;

        [BindProperty]
        public FriendIM FriendInputModel { get; set; }

        // public string PageHeader { get; set; } // Implement in future if add is implemented
        public string ErrorMessage { get; set; } = null; // Not implemented at the moment.

        //For Server Side Validation set by IsValid()
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }

        public EditFriendDetailsModel(IFriendsService service)
        {
            _service = service;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Read a QueryParameter
                if (Guid.TryParse(Request.Query["friendId"], out Guid _id))
                {
                    //Use the Service and populate the InputModel
                    FriendInputModel = new FriendIM(await _service.ReadFriendAsync(_id, false));

                    // PageHeader = "Edit details of a friend";
                }
                else
                {
                    // Use if add a friend is implemented in future 
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return Page();
        }
        public async Task<IActionResult> OnPostUndo()
        {
            //Use the Service and populate the InputModel
            FriendInputModel = new FriendIM(await _service.ReadFriendAsync(FriendInputModel.FriendId, false));
            // PageHeader = "Edit details of a quote"; // Not implemented at current version
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {

            //PageHeader = (FriendInputModel.StatusIM == StatusIM.Inserted) ? "Create a new quote" : "Edit details of a quote"; // Not implemented at current version

            if (!IsValid())
            {
                //The page is not valid
                return Page();
            }

            //Use the Service and populate the InputModel
            if (FriendInputModel.StatusIM == StatusIM.Inserted)
            {
                // Implement in future if Add a friend is implemented.
            }
            else
            {

                IFriend model = await _service.ReadFriendAsync(FriendInputModel.FriendId, false);
                model = FriendInputModel.UpdateModel(model);


                IAddress relevantAddress = await UpdateAddress(model.Address); // kan man skicka in id enbart?
                var dtoFriend = new FriendCUdto(model);
                dtoFriend.AddressId = relevantAddress.AddressId;
                try
                {
                    model = await _service.UpdateFriendAsync(dtoFriend);
                }
                catch (Exception e)
                {
                    ErrorMessage = e.Message;
                }
            }

            return RedirectToPage("/FriendDetails", new { id = FriendInputModel.FriendId });

        }

        public async Task<IAddress> UpdateAddress(IAddress address)
        {
            IAddress test;
            var tempAddress = new AddressIM(address);

            // If no changes has been done to the address, then they are equal. Return address.
            bool isEqual = tempAddress.Equals(FriendInputModel.AddressInputModel);
            if (isEqual)
            {
                return address;
            }

            int _pageSize = 1000;
            var resultOfFilteredAddresses = await _service.ReadAddressesAsync(true, false, FriendInputModel.AddressInputModel.City, 0, _pageSize);

            // If pagesize is smaller the DbItemsCount, it means that not all addresses were loaded from the service. We fix it with a call on the service with the exact amount of objects.
            if (resultOfFilteredAddresses.DbItemsCount > _pageSize)
            {
                _pageSize = resultOfFilteredAddresses.DbItemsCount;
                resultOfFilteredAddresses = await _service.ReadAddressesAsync(true, false, FriendInputModel.AddressInputModel.City, _pageSize, 0);
            }

            // If the entered address exist in database, return it. Otherwise create a new address.
            var returnedItem = resultOfFilteredAddresses.PageItems.FirstOrDefault
            (a => a.Country == FriendInputModel.AddressInputModel.Country && a.ZipCode == FriendInputModel.AddressInputModel.ZipCode && a.StreetAddress == FriendInputModel.AddressInputModel.StreetAddress);

            if (returnedItem != null)
            {
                return returnedItem; // vad händer här, verkar inte sätta rätt adress.
            }

            else // Create a new address and add it to the database.
            {

                IAddress newAddress = FriendInputModel.AddressInputModel.CreateNewAddress(address);
                var dtoAddress = new AddressCUdto(newAddress);
                dtoAddress.AddressId = null;
                IAddress testtest = await _service.CreateAddressAsync(dtoAddress);
                return testtest;

            }
        }

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public enum StatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        public class AddressIM
        {
            public StatusIM StatusIM { get; set; }
            public Guid AddressId { get; init; } // Guid.NewGuid();
            [Required(ErrorMessage = "Street address is required")]
            public string StreetAddress { get; set; }

            [Required(ErrorMessage = "Zipcode is required")]
            [RegularExpression(@"^\d+$", ErrorMessage = "Zip code must contain only numbers")]
            public int ZipCode { get; set; }

            [Required(ErrorMessage = "City is required")]
            public string City { get; set; }

            [Required(ErrorMessage = "Country is required")]
            public string Country { get; set; }

            public bool Equals(AddressIM other) => (other != null) && ((this.StreetAddress, this.ZipCode, this.City, this.Country) ==
        (other.StreetAddress, other.ZipCode, other.City, other.Country));

            public AddressIM()
            {

            }

            public AddressIM(AddressIM original)
            {
                StatusIM = original.StatusIM;

                AddressId = original.AddressId;
                StreetAddress = original.StreetAddress;
                ZipCode = original.ZipCode;
                City = original.City;
                Country = original.Country;
            }

            public AddressIM(IAddress original)
            {
                StatusIM = StatusIM.Unchanged;
                AddressId = original.AddressId;
                StreetAddress = original.StreetAddress;
                ZipCode = original.ZipCode;
                City = original.City;
                Country = original.Country;
            }
            public IAddress UpdateModel(IAddress model)
            {
                model.AddressId = AddressId;
                model.StreetAddress = StreetAddress;
                model.ZipCode = ZipCode;
                model.City = City;
                model.Country = Country;

                return model;
            }

            public IAddress CreateNewAddress(IAddress model)
            {
                //model.AddressId = Guid.NewGuid();
                model.StreetAddress = StreetAddress;
                model.ZipCode = ZipCode;
                model.City = City;
                model.Country = Country;

                return model;
            }

        }

        public class FriendIM
        {
            //Status of InputModel
            public StatusIM StatusIM { get; set; }

            //Properties from Model which is to be edited in the <form>
            public Guid FriendId { get; init; } = Guid.NewGuid();
            [Required(ErrorMessage = "First name is required")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "Last name is required")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "Email is required")]
            public string Email { get; set; }
            public DateTime? Birthday { get; set; }
            public AddressIM AddressInputModel { get; set; }

            #region constructors and model update
            public FriendIM() { StatusIM = StatusIM.Unchanged; }

            //Copy constructor
            public FriendIM(FriendIM original)
            {
                StatusIM = original.StatusIM;

                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;
                Email = original.Email;
                Birthday = original.Birthday;
            }

            //Model => InputModel constructor
            public FriendIM(IFriend original)
            {
                StatusIM = StatusIM.Unchanged;
                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;
                Email = original.Email;
                Birthday = original.Birthday;
                AddressInputModel = new AddressIM(original.Address);
            }

            //InputModel => Model
            public IFriend UpdateModel(IFriend model)
            {
                model.FriendId = FriendId;
                model.FirstName = FirstName;
                model.LastName = LastName;
                model.Email = Email;
                model.Birthday = Birthday;

                return model;
            }
            #endregion

        }
        #endregion

        #region Server Side Validation
        private bool IsValid(string[] validateOnlyKeys = null)
        {
            InvalidKeys = ModelState
               .Where(s => s.Value.ValidationState == ModelValidationState.Invalid);

            if (validateOnlyKeys != null)
            {
                InvalidKeys = InvalidKeys.Where(s => validateOnlyKeys.Any(vk => vk == s.Key));
            }

            ValidationErrorMsgs = InvalidKeys.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
            HasValidationErrors = InvalidKeys.Any();

            return !HasValidationErrors;
        }
        #endregion

    }

}

