using System;
using System.Collections.Generic;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for "Wirkung" that is used for the Index view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EffectViewModelIndex
    {
        private CategoryValue? _categoryValue;

        public EffectViewModelIndex(IList<EffectViewModel> effectViewModels, IList<Category> categories)
        {
            EffectViewModels = effectViewModels;
            Categories = categories;
        }

        public IList<EffectViewModel> EffectViewModels { get; set; }

        public IList<Category> Categories { get; set; }


        /// <summary>
        /// Creates a list of displayable category names.
        /// </summary>
        /// <author>Anna Krebs</author>
        public IList<KeyValuePair<string, string>> CategoryDisplayItems
        {
            get
            {
                IList<KeyValuePair<string, string>> categoriesList = new List<KeyValuePair<string, string>>();

                foreach (Category category in Categories)
                {
                    CategoryValue = category.Name;
                    categoriesList.Add(new KeyValuePair<string, string>(category.Id, CategoryValueDisplayName));
                }
                return categoriesList;
            }
        }

        /// <summary>
        /// Displays name of category enum.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string CategoryValueDisplayName
        {
            get { return _categoryValue?.Display(); }
        }

        /// <summary>
        /// Parses enum to string in order to display the appropriate name of the category.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string CategoryValue
        {
            get { return _categoryValue.ToString(); }
            set
            {
                CategoryValue c;
                _categoryValue = Enum.TryParse(value, true, out c) ? (CategoryValue?)c : null;
            }
        }

        /// <summary>
        /// Effect, that was created or edited before returned to Index page.
        /// Is used in order to scroll to the last created or edited item in the list.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string LastEditedEffectId { get; set; }
    }

}

