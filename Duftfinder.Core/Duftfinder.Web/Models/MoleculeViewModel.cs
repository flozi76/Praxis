using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Duftfinder.Web.Models
{
	/// <summary>
	///     Represents the model for "Molekül" that is used for the view.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class MoleculeViewModel
	{
		private readonly Molecule _molecule;

		private SubstanceValue? _substanceValue;

		public MoleculeViewModel()
		{
			_molecule = new Molecule();
		}

		public MoleculeViewModel(Molecule molecule, IList<Substance> substances)
		{
			if (molecule == null)
				_molecule = new Molecule();
			else
				_molecule = molecule;

			Substances = substances;
		}

		public string Id
		{
			get => _molecule.Id;
			set => _molecule.Id = value;
		}

		[Required(ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InputRequired")]
		public string Name
		{
			get
			{
				// Show e.g. "Aromatische Aldehyde allgemein" if is general molecule. 
				if (IsGeneral) return $"{SubstanceValueDisplayName} {Resources.Resources.Molecule_General}";
				return _molecule.Name;
			}
			set => _molecule.Name = value;
		}

		public bool IsGeneral
		{
			get => _molecule.IsGeneral;
			set => _molecule.IsGeneral = value;
		}

		public string SubstanceId
		{
			get => _molecule.SubstanceIdString;
			set => _molecule.SubstanceIdString = value;
		}

		/// <summary>
		///     List of substances of a molecule.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<Substance> Substances { get; set; }

		/// <summary>
		///     Creates a list item for each substance for drop down.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<SelectListItem> SubstanceListItems
		{
			get
			{
				IList<SelectListItem> substancesList = new List<SelectListItem>();

				foreach (var substance in Substances)
				{
					SubstanceValue = substance.Name;
					substancesList.Add(new SelectListItem {Text = SubstanceValueDisplayName, Value = substance.Id});
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
		///     Map values from View to Entity.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="molecule"></param>
		public void MapViewModelToEntity(Molecule molecule)
		{
			molecule.Id = Id;
			molecule.Name = Name;
			molecule.IsGeneral = IsGeneral;
			molecule.SubstanceIdString = SubstanceId;
		}
	}
}