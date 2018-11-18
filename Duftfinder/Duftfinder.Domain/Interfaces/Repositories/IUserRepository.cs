﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Represents the interface of the store for Users.
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface IUserRepository : IRepository<User, UserFilter>
    {
    }
}
