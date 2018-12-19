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
	///     Represents the model for users that is used for the view.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class UserViewModel : RegisterViewModel
	{
		private readonly User _user;

		private RoleValue? _roleValue;

		public UserViewModel()
		{
			_user = new User();
		}

		public UserViewModel(User user, IList<Role> roles)
		{
			if (user == null)
				_user = new User();
			else
				_user = user;

			Roles = roles;
		}

		public string Id
		{
			get => _user.Id;
			set => _user.Id = value;
		}

		[Required(ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InputRequired")]
		public new string FirstName
		{
			get => _user.FirstName;
			set => _user.FirstName = value;
		}

		[Required(ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InputRequired")]
		public new string LastName
		{
			get => _user.LastName;
			set => _user.LastName = value;
		}

		[Required(ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InvalidEmail")]
		[EmailAddress(ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InvalidEmail")]
		[StringLength(40, ErrorMessageResourceType = typeof(Resources.Resources),
			ErrorMessageResourceName = "Error_InvalidEmailLength")]
		public new string Email
		{
			get => _user.Email;
			set => _user.Email = value;
		}

		public bool IsAccountVerified
		{
			get => _user.IsAccountVerified;
			set => _user.IsAccountVerified = value;
		}

		public bool IsConfirmed
		{
			get => _user.IsConfirmed;
			set => _user.IsConfirmed = value;
		}

		public bool IsInactive
		{
			get => _user.IsInactive;
			set => _user.IsInactive = value;
		}

		public bool IsSystemAdmin
		{
			get => _user.IsSystemAdmin;
			set => _user.IsSystemAdmin = value;
		}

		public string RoleId
		{
			get => _user.RoleIdString;
			set => _user.RoleIdString = value;
		}

		/// <summary>
		///     List of roles.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<Role> Roles { get; set; }

		/// <summary>
		///     Creates a list item for each role for dropdown.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<SelectListItem> RoleListItems
		{
			get
			{
				IList<SelectListItem> roleList = new List<SelectListItem>();

				foreach (var role in Roles)
				{
					RoleValueString = role.Name;

					var roleListIem = new SelectListItem();
					if (RoleValueDisplayName == RoleValue.Friend.ToString())
						roleListIem = new SelectListItem
							{Text = RoleValueDisplayName, Value = role.Id, Selected = true};
					else
						roleListIem = new SelectListItem {Text = RoleValueDisplayName, Value = role.Id};

					roleList.Add(roleListIem);
				}

				return roleList;
			}
		}

		/// <summary>
		///     Displays name of role enum.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string RoleValueDisplayName => _roleValue?.Display();

		/// <summary>
		///     Parses enum to string in order to display the appropriate name of the role.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string RoleValueString
		{
			get => _roleValue.ToString();
			set
			{
				RoleValue r;
				_roleValue = Enum.TryParse(value, true, out r) ? (RoleValue?) r : null;
			}
		}

		/// <summary>
		///     Map values from View to Entity.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="user"></param>
		public new void MapViewModelToEntity(User user)
		{
			user.Id = Id;
			user.FirstName = FirstName;
			user.LastName = LastName;
			user.Email = Email;
			user.IsAccountVerified = IsAccountVerified;
			user.IsConfirmed = IsConfirmed;
			user.IsInactive = IsInactive;
			user.RoleIdString = RoleId;
		}
	}
}