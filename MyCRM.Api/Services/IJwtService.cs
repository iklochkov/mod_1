using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyCRM.Api.Services
{
    public interface IJwtService
    {
        public string GenerateToken(IdentityUser user, IList<string> roles);
    }
}