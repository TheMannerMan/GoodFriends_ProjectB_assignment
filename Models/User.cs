using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

using Newtonsoft.Json;
using System.Linq;

using Configuration;

namespace Models
{
	//https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identityuser?view=aspnetcore-7.0
	public class User : IdentityUser<Guid>
	{
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

