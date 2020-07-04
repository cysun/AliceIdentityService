using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ConsoleManager
{
    partial class ConsoleManager
    {
        private async Task CheckAdminRoleAsync()
        {
            if (!await roleManager.RoleExistsAsync(AdminRoleName))
                await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
        }
    }
}
