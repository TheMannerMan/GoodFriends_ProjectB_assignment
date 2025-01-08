using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AppGoodFriendsMVC.Models
{
    public class FriendViewModel
    {
        public IFriend Friend { get; set; }

        [BindProperty]
        public Guid FriendId { get; set; }
    }
}