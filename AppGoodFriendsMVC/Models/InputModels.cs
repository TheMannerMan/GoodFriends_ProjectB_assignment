using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace AppGoodFriendsMVC.Models
{
    public enum StatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

    public class AddressIM
    {
        public StatusIM StatusIM { get; set; }
        public Guid AddressId { get; init; } // Guid.NewGuid();
        public string StreetAddress { get; set; }
        
        [RegularExpression(@"^\d+$", ErrorMessage = "Zip code must contain only numbers")]
        public int ZipCode { get; set; }
        public string City { get; set; }
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
    }
}
#endregion