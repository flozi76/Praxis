using Duftfinder.Domain.Entities;

namespace Duftfinder.Web.Models
{
	/// <summary>
	///     Represents the model for configurations that is used for the view.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class ConfigurationViewModel
	{
		private readonly Configuration _configuration;


		public ConfigurationViewModel()
		{
			_configuration = new Configuration();
		}

		public ConfigurationViewModel(Configuration configuration)
		{
			if (configuration == null)
				_configuration = new Configuration();
			else
				_configuration = configuration;
		}

		public string Id
		{
			get => _configuration.Id;
			set => _configuration.Id = value;
		}

		public string Key
		{
			get => _configuration.Key;
			set => _configuration.Key = value;
		}

		public string Value
		{
			get => _configuration.Value;
			set => _configuration.Value = value;
		}

		public string Description
		{
			get => _configuration.Description;
			set => _configuration.Description = value;
		}

		/// <summary>
		///     Map values from View to Entity.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="configuration"></param>
		public void MapViewModelToEntity(Configuration configuration)
		{
			configuration.Id = Id;
			configuration.Key = Key;
			configuration.Value = Value;
			configuration.Description = Description;
		}
	}
}