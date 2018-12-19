namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the business logic service for authentication related stuff.
	/// </summary>
	public interface IDuftfinderAuthenticationService
	{
		void SignIn(string email);

		void SignOut();
	}
}