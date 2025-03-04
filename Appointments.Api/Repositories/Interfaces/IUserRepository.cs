﻿using Appointments.Api.Models;

namespace Appointments.Api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
    }
}
