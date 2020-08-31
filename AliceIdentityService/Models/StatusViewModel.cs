using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceIdentityService.Models
{
    public class StatusViewModel
    {
        public string Message { get; set; }
        public bool IsError { get; set; } = false;
    }
}
