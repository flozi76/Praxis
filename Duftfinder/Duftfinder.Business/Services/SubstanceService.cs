using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Database.Repositories;
using log4net;

namespace Duftfinder.Business.Services
{
    /// <summary>
    /// Contains business logic for "Stoffklasse".
    /// Basic functionality is done in Service.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class SubstanceService : Service<Substance, SubstanceFilter, ISubstanceRepository>, ISubstanceService
    {
        private readonly ISubstanceRepository _substanceRepository;

        public SubstanceService(ISubstanceRepository substanceRepository) 
            : base(substanceRepository)
        {
            _substanceRepository = substanceRepository;
        }
    }
}
