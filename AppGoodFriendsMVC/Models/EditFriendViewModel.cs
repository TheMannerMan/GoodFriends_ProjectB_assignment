using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AppGoodFriendsMVC.Models
{
    public class EditFriendViewModel
    {
        [BindProperty]
        public FriendIM FriendInputModel { get; set; }

        //For Server Side Validation set by IsValid()
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string>? ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>>? InvalidKeys { get; set; }
    }
}