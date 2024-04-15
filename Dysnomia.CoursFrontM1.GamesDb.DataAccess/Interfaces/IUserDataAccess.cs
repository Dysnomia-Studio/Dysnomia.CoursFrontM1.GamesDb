﻿
using Dysnomia.CoursFrontM1.GamesDb.Common.Dao;
using Dysnomia.CoursFrontM1.GamesDb.Common.Requests;

namespace Dysnomia.CoursFrontM1.GamesDb.DataAccess.Interfaces {
    public interface IUserDataAccess {
        Task<UserDao?> GetByUsername(string? username);
        Task Register(UserRegistrationRequest registrationRequest);
    }
}
