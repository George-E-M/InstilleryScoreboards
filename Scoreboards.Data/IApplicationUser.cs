﻿using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboards.Data
{
    public interface IApplicationUser
    {
        ApplicationUser GetById(string userId);
        IEnumerable<ApplicationUser> GetAll();
        IEnumerable<ApplicationUser> GetAllActive();
        Task SetProfileImageAsync(string userId, Uri uri);
        Task SetMottoAsync(string userId, string motto);
        IEnumerable<ApplicationUser> GetByRole(string userRole);
        Task DeleteUserProfileAsync(ApplicationUser user);
    }
}
