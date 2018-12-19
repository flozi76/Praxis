using System;
using System.Collections.Generic;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Web.Models
{
	/// <summary>
	///     Represents the model for "Molekül" that is used for the Index view.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class MoleculeViewModelIndex
	{
		private SubstanceValue? _substanceValue;

		public MoleculeViewModelIndex(IList<MoleculeViewModel> moleculeViewModels, IList<Substance> substances)
		{
			MoleculeViewModels = moleculeViewModels;
			Substances = substances;
		}

		public IList<MoleculeViewModel> MoleculeViewModels { get; set; }

		public IList<Substance> Substances { get; set; }


		/// <summary>
		///     Creates a list of displayable substance names.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<KeyValuePair<string, string>> SubstanceDisplayItems
		{
			get
			{
				IList<KeyValuePair<string, string>> substancesList = new List<KeyValuePair<string, string>>();

				foreach (var substance in Substances)
				{
					SubstanceValue = substance.Name;
					substancesList.Add(new KeyValuePair<string, string>(substance.Id, SubstanceValueDisplayName));
				}

				return substancesList;
			}
		}

		/// <summary>
		///     Displays name of substance enum.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string SubstanceValueDisplayName => _substanceValue?.Display();

		/// <summary>
		///     Parses enum to string in order to display the appropriate name of the substance.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string SubstanceValue
		{
			get => _substanceValue.ToString();
			set
			{
				SubstanceValue s;
				_substanceValue = Enum.TryParse(value, true, out s) ? (SubstanceValue?) s : null;
			}
		}

		/// <summary>
		///     Molecule, that was created or edited before returned to Index page.
		///     Is used in order to scroll to the last created or edited item in the list.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string LastEditedMoleculeId { get; set; }
	}
}