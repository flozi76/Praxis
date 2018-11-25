using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for "Moleküle zuweisen" that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class AssignMoleculeViewModel
    {
        private readonly EssentialOil _essentialOil;

        private readonly Effect _effect;

        private SubstanceValue? _substanceValue;

        public AssignMoleculeViewModel()
        {
            _essentialOil = new EssentialOil();
            _effect = new Effect();
        }

        public AssignMoleculeViewModel(EssentialOil essentialOil, Effect effect, IList<AssignValueViewModel> moleculeViewModels, IList<Substance> substances)
        {
            _essentialOil = essentialOil;
            _effect = effect;
            AssignMolecules = moleculeViewModels;
            Substances = substances;
        }

        public string EssentialOilId
        {
            get { return _essentialOil.Id; }
            set { _essentialOil.Id = value; }
        }

        public string EssentialOilName
        {
            get { return _essentialOil.Name; }
            set { _essentialOil.Name = value; }
        }

        public string EffectId
        {
            get { return _effect.Id; }
            set { _effect.Id = value; }
        }

        public string EffectName
        {
            get { return _effect.Name; }
            set { _effect.Name = value; }
        }

        /// <summary>
        /// Molecules that are assignable to essential oil or effect.
        /// </summary>
        /// <author>Anna Krebs</author>
        public IList<AssignValueViewModel> AssignMolecules { get; set; }

        public IList<Substance> Substances { get; set; }

        /// <summary>
        /// Creates a list of displayable substance names.
        /// </summary>
        /// <author>Anna Krebs</author>
        public IList<KeyValuePair<string, string>> SubstanceDisplayItems
        {
            get
            {
                IList<KeyValuePair<string, string>> substanceList = new List<KeyValuePair<string, string>>();

                foreach (Substance substance in Substances)
                {
                    SubstanceValue = substance.Name;
                    substanceList.Add(new KeyValuePair<string, string>(substance.Id, SubstanceValueDisplayName));
                }
                return substanceList;
            }
        }

        /// <summary>
        /// Displays name of substance enum.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string SubstanceValueDisplayName
        {
            get { return _substanceValue?.Display(); }
        }

        /// <summary>
        /// Parses enum to string in order to display the appropriate name of the substance.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string SubstanceValue
        {
            get { return _substanceValue.ToString(); }
            set
            {
                SubstanceValue c;
                _substanceValue = Enum.TryParse(value, true, out c) ? (SubstanceValue?)c : null;
            }
        }
    }
}

