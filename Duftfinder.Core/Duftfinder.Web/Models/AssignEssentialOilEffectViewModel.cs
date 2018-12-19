using System;
using System.Collections.Generic;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Web.Models
{
	/// <summary>
	///     Represents the model for "Wirkung zuweisen" that is used for the view.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class AssignEssentialOilEffectViewModel
	{
		private readonly Effect _effect;
		private readonly EssentialOil _essentialOil;

		private CategoryValue? _categoryValue;

		public AssignEssentialOilEffectViewModel()
		{
			_essentialOil = new EssentialOil();
			_effect = new Effect();
		}

		public AssignEssentialOilEffectViewModel(EssentialOil essentialOil, Effect effect,
			IList<AssignValueViewModel> assignValueViewModel, IList<Category> categories)
		{
			if (essentialOil != null)
			{
				_essentialOil = essentialOil;
				AssignEffects = assignValueViewModel;
				Categories = categories;
			}
			else if (effect != null)
			{
				_effect = effect;
				AssignEssentialOils = assignValueViewModel;
			}
		}

		public string EssentialOilId
		{
			get => _essentialOil.Id;
			set => _essentialOil.Id = value;
		}

		public string EssentialOilName
		{
			get => _essentialOil.Name;
			set => _essentialOil.Name = value;
		}

		public string EffectId
		{
			get => _effect.Id;
			set => _effect.Id = value;
		}

		public string EffectName
		{
			get => _effect.Name;
			set => _effect.Name = value;
		}

		/// <summary>
		///     Essential oils that are assignable to effect.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <author>Anna Krebs</author>
		public IList<AssignValueViewModel> AssignEssentialOils { get; set; }

		/// <summary>
		///     Effects that are assignable to essential oil.
		/// </summary>
		public IList<AssignValueViewModel> AssignEffects { get; set; }

		public IList<Category> Categories { get; set; }

		/// <summary>
		///     Creates a list of displayable category names.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<KeyValuePair<string, string>> CategoryDisplayItems
		{
			get
			{
				IList<KeyValuePair<string, string>> categoryList = new List<KeyValuePair<string, string>>();

				foreach (var category in Categories)
				{
					CategoryValue = category.Name;
					categoryList.Add(new KeyValuePair<string, string>(category.Id, CategoryValueDisplayName));
				}

				return categoryList;
			}
		}

		/// <summary>
		///     Displays name of category enum.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string CategoryValueDisplayName => _categoryValue?.Display();

		/// <summary>
		///     Parses enum to string in order to display the appropriate name of the category.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string CategoryValue
		{
			get => _categoryValue.ToString();
			set
			{
				CategoryValue c;
				_categoryValue = Enum.TryParse(value, true, out c) ? (CategoryValue?) c : null;
			}
		}

		/// <summary>
		///     Creates a list of alphabetical indexes of essential oils. (A-Z).
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<string> AlphabeticalIndexes
		{
			get
			{
				IList<string> firstLetterList = new List<string>();
				foreach (var assignValueViewModel in AssignEssentialOils)
				{
					// Add first letter to list, if it doesn't already exist.
					var firstLetter = assignValueViewModel.AssignedValueName.Substring(0, 1).ToUpper();
					if (!firstLetterList.Contains(firstLetter)) firstLetterList.Add(firstLetter);
				}

				return firstLetterList;
			}
		}
	}
}