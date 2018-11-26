using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;

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
