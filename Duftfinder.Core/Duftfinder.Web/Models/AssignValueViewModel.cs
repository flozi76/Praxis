using System;
using System.ComponentModel.DataAnnotations;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Web.Models
{
	/// <summary>
	///     Represents the model for "Moleküle" or "Wirkungen" that is used to
	///     assign molecules or an effects to an essential oil.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class AssignValueViewModel
	{
		private SubstanceValue? _substanceValue;

		public AssignValueViewModel()
		{
		}

		public AssignValueViewModel(Molecule molecule, Effect effect, EssentialOil essentialOil)
		{
			// Assign value depending on whether molecule, effect or essential oil was assigned.
			if (molecule != null)
			{
				AssignedValueId = molecule.Id;
				SubstanceIdString = molecule.SubstanceIdString;
				SubstanceValue = molecule.Name;

				// Show e.g. "Aromatische Aldehyde allgemein" if is general molecule.
				if (molecule.IsGeneral)
					AssignedValueName = $"{SubstanceValueDisplayName} {Resources.Resources.Molecule_General}";
				else
					AssignedValueName = molecule.Name;
			}
			else if (effect != null)
			{
				AssignedValueId = effect.Id;
				AssignedValueName = effect.Name;
				CategoryIdString = effect.CategoryIdString;
			}
			else if (essentialOil != null)
			{
				AssignedValueId = essentialOil.Id;
				AssignedValueName = essentialOil.Name;
				NameLatin = essentialOil.NameLatin;
				FirstLetterOfEssentialOilName = essentialOil.Name.Substring(0, 1).ToUpper();
			}
		}

		public string AssignedValueId { get; set; }

		public string AssignedValueName { get; set; }

		[Range(0, 100, ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InputRangeMin0Max100")]
		public double? MoleculePercentage { get; set; }

		public string SubstanceIdString { get; set; }

		[Range(0, 4, ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InputRangeMin0Max4")]
		public int EffectDegree { get; set; }

		public string NameLatin { get; set; }

		/// <summary>
		///     The first letter of the essential oil name.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string FirstLetterOfEssentialOilName { get; set; }

		public string CategoryIdString { get; set; }

		/// <summary>
		///     Displays name of substance enum.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string SubstanceValueDisplayName => _substanceValue?.Display();

		/// <summary>
		///     Parses enum to string in order to display the appropriate name of the substance.
		/// </summary>
		public string SubstanceValue
		{
			get => _substanceValue.ToString();
			set
			{
				SubstanceValue s;
				_substanceValue = Enum.TryParse(value, true, out s) ? (SubstanceValue?) s : null;
			}
		}
	}
}