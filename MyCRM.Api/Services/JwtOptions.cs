using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Api.Services
{
    public class JwtOptions
    {
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationHours { get; set; } = 1;
    }
}