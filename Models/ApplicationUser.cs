﻿using Microsoft.AspNetCore.Identity;

namespace ProgPoe.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public virtual ICollection<Claim> Claims { get; set; }

    }
}
