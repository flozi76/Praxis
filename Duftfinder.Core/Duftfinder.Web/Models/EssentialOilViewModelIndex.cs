using System.Collections.Generic;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for "Ätherische Öle" that is used for the Index view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EssentialOilViewModelIndex
    {

        public EssentialOilViewModelIndex(IList<EssentialOilViewModel> essentialOilViewModels)
        {
            EssentialOilViewModels = essentialOilViewModels;
        }

        public IList<EssentialOilViewModel> EssentialOilViewModels { get; set; }
        
        /// <summary>
        /// Creates a list of alphabetical indexes of essential oils. (A-Z).
        /// </summary>
        /// <author>Anna Krebs</author>
        public IList<string> AlphabeticalIndexes {
            get
            {
                IList<string> firstLetterList = new List<string>();
                foreach (EssentialOilViewModel essentialOilViewModel in EssentialOilViewModels)
                {
                    // Add first letter to list, if it doesn't already exist.
                    string firstLetter = essentialOilViewModel.Name.Substring(0, 1).ToUpper();
                    if (!firstLetterList.Contains(firstLetter))
                    {
                        firstLetterList.Add(firstLetter);
                    }
                }
                return firstLetterList;
            }
        }

        /// <summary>
        /// Essential oil, that was created or edited before returned to Index page.
        /// Is used in order to scroll to the last created or edited item in the list.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string LastEditedEssentialOilId { get; set; }
    }
}

