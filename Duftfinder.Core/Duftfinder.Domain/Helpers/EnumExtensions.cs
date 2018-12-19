using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Duftfinder.Domain.Helpers
{
	/// <summary>
	///     Extension to display names of enums.
	/// </summary>
	/// <seealso>adesso DEXViewer project</seealso>
	public static class EnumExtensions
	{
		/// <summary>
		///     Returns the display name of the given enum value
		///     or value.ToString() if the DisplayAttribute is not defined for the given enum value.
		/// </summary>
		/// <seealso>adesso DEXViewer project</seealso>
		public static string Display(this Enum value)
		{
			var displayName = value.GetType()
				                  .GetField(value.ToString())
				                  .GetCustomAttributes(typeof(DisplayAttribute), false)
				                  .Cast<DisplayAttribute>()
				                  .FirstOrDefault() ?? new DisplayAttribute {Name = value.ToString()};

			return displayName.GetName();
		}

		/// <summary>
		///     Returns the short display name of the given enum value
		///     or value.ToString() if the DisplayShortNameAttribute is not defined for the given enum value.
		/// </summary>
		/// <seealso>adesso DEXViewer project</seealso>
		public static string DisplayShort(this Enum value)
		{
			var displayName = value.GetType()
				                  .GetField(value.ToString())
				                  .GetCustomAttributes(typeof(DisplayAttribute), false)
				                  .Cast<DisplayAttribute>()
				                  .FirstOrDefault() ?? new DisplayAttribute {ShortName = value.ToString()};

			return displayName.GetShortName();
		}

		/// <summary>
		///     Gets the description of the given enum value
		///     or value.ToString() if the description attribute is not defined for the given enum value.
		/// </summary>
		/// <seealso>adesso DEXViewer project</seealso>
		public static string Description(this Enum value)
		{
			var description = value.GetType()
				                  .GetField(value.ToString())
				                  .GetCustomAttributes(typeof(DescriptionAttribute), false)
				                  .Cast<DescriptionAttribute>()
				                  .FirstOrDefault() ?? new DescriptionAttribute(value.ToString());

			var result = description.Description;

			if (string.IsNullOrEmpty(result)) return value.ToString();

			return result;
		}

		public static T GetEnumValueFromDescription<T>(string description)
		{
			if (!typeof(T).IsEnum)
				throw new NotSupportedException(string.Format("{0} is not of type Enum.", typeof(T)));

			var fis = typeof(T).GetFields();

			foreach (var fi in fis)
			{
				var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attributes.Length > 0 && attributes[0].Description == description)
					return (T) Enum.Parse(typeof(T), fi.Name);
			}

			throw new ArgumentNullException("Not found");
		}
	}
}