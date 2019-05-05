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
	///     Represents the model for "Wirkung" that is used for the view.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EffectViewModel
	{
		private readonly Effect _effect;

		private CategoryValue? _categoryValue;

		public EffectViewModel()
		{
			_effect = new Effect();
		}

		public EffectViewModel(Effect effect, IList<Category> categories)
		{
			if (effect == null)
				_effect = new Effect();
			else
				_effect = effect;

			Categories = categories;
		}

		public string Id
		{
			get => _effect.Id;
			set => _effect.Id = value;
		}

		[Required(ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InputRequired")]
		public string Name
		{
			get => _effect.Name;
			set => _effect.Name = value;
		}

		public string Details
		{
			get => _effect.Details;
			set => _effect.Details = value;
		}

		public string CategoryId
		{
			get => _effect.CategoryIdString;
			set => _effect.CategoryIdString = value;
		}

		/// <summary>
		///     List of categories of a effect.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<Category> Categories { get; set; }

		/// <summary>
		///     Creates a list item for each category for drop down.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<SelectListItem> CategoryListItems
		{
			get
			{
				IList<SelectListItem> categoriesList = new List<SelectListItem>();

				if (Categories != null)
				{
					foreach (var category in Categories)
					{
						CategoryValue = category.Name;
						categoriesList.Add(new SelectListItem { Text = CategoryValueDisplayName, Value = category.Id });
					}
				}

				return categoriesList;
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
		///     Map values from View to Entity.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="effect"></param>
		public void MapViewModelToEntity(Effect effect)
		{
			effect.Id = Id;
			effect.Name = Name;
			effect.Details = Details;
			effect.CategoryIdString = CategoryId;
		}
	}
}