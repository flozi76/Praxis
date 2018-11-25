using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Filters
{
    /// <summary>
    /// User filter, for filtering for specific properties, sorting etc.
    /// Important: Properties have to be nullable, in order to filter properly.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class UserFilter : Filter
    {
        public string Email { get; set; }

        /// <summary>
        /// Dictionary object for sorting entity.
        /// Key: Name of field in DB for sorting.
        /// Value: Sort direction. (Ascending or Descending)
        /// <author>Anna Krebs</author>
        /// </summary>
        public Dictionary<string, string> SortValues { get; set; } = new Dictionary<string, string>
        {
            { Constants.IsSystemAdmin, Constants.Descending },
            { Constants.LastName, Constants.Ascending },
            { Constants.FirstName, Constants.Ascending },
            { Constants.Email, Constants.Ascending },
        };
    }
}
