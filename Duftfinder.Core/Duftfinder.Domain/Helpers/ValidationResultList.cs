using System.Collections.Generic;

namespace Duftfinder.Domain.Helpers
{
	/// <summary>
	///     Is Dictionary of Errors.
	/// </summary>
	/// <seealso>adesso SzkB.Ehypo project</seealso>
	public class ValidationResultList
	{
		/// <summary>
		///     Dictionary of errors with key & error message.
		/// </summary>
		/// <seealso>adesso SzkB.Ehypo project</seealso>
		public IDictionary<string, string> Errors { get; } = new Dictionary<string, string>();

		/// <summary>
		///     Defines whether the ValidationResult holds errors.
		/// </summary>
		/// <seealso>adesso SzkB.Ehypo project</seealso>
		public bool HasErrors => Errors.Count > 0;
	}
}