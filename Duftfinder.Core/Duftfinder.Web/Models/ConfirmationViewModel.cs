namespace Duftfinder.Web.Models
{
	/// <summary>
	///     Represents the model for confirmation dialogs that is used for the view.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class ConfirmationViewModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string DialogTitle { get; set; }

		public string DialogText { get; set; }

		/// <summary>
		///     The HttpPost action that will be called in the JS-file.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string Action { get; set; }
	}
}