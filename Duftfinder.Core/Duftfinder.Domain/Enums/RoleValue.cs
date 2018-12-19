using System.ComponentModel.DataAnnotations;

namespace Duftfinder.Domain.Enums
{
	/// <summary>
	///     Enum of Roles.
	/// </summary>
	/// <author>Anna Krebs</author>
	public enum RoleValue
	{
		[Display(Name = nameof(Resources.Resources.RoleValue_Admin), ResourceType = typeof(Resources.Resources))]
		Admin,

		[Display(Name = nameof(Resources.Resources.RoleValue_Friend), ResourceType = typeof(Resources.Resources))]
		Friend
	}
}