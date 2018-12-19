namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the encryption and decryption.
	/// </summary>
	/// <seealso>adesso SzkB.Ehypo project</seealso>
	public interface ICryptoService
	{
		string GeneratePasswordHash(string password);

		bool ValidatePassword(string password, string correctHash);
	}
}