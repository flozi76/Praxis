using Autofac;
using Duftfinder.Business.Services;
using Duftfinder.Database.Mongo;
using Duftfinder.Database.Repositories;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;

namespace Duftfinder.Ioc
{
	public static class ContainerConfig
	{
		public static void BuildUnityContainer(ContainerBuilder builder)
		{
			// Register all components with the container here. 
			builder.RegisterType<SettingsRepository>().As<ISettingsRepository>();
			builder.RegisterType<SettingsService>().As<ISettingsService>();

			builder.RegisterType<ConfigurationRepository>().As<IConfigurationRepository>();
			builder.RegisterType<ConfigurationService>().As<IConfigurationService>();

			builder.RegisterType<DuftfinderAuthenticationService>().As<IDuftfinderAuthenticationService>();

			builder.RegisterType<SmtpEmailService>().As<ISmtpEmailService>();

			builder.RegisterType<EmailService>().As<IEmailService>();

			builder.RegisterType<CryptoService>().As<ICryptoService>();

			builder.RegisterType<UserRepository>().As<IUserRepository>();
			builder.RegisterType<UserService>().As<IUserService>();

			builder.RegisterType<RoleRepository>().As<IRoleRepository>();

			builder.RegisterType<EssentialOilRepository>().As<IEssentialOilRepository>();
			builder.RegisterType<EssentialOilService>().As<IEssentialOilService>();

			builder.RegisterType<EffectRepository>().As<IEffectRepository>();
			builder.RegisterType<EffectService>().As<IEffectService>();

			builder.RegisterType<CategoryRepository>().As<ICategoryRepository>();
			builder.RegisterType<CategoryService>().As<ICategoryService>();

			builder.RegisterType<SubstanceRepository>().As<ISubstanceRepository>();
			builder.RegisterType<SubstanceService>().As<ISubstanceService>();

			builder.RegisterType<MoleculeRepository>().As<IMoleculeRepository>();
			builder.RegisterType<MoleculeService>().As<IMoleculeService>();

			builder.RegisterType<EssentialOilMoleculeRepository>().As<IEssentialOilMoleculeRepository>();
			builder.RegisterType<EssentialOilMoleculeService>().As<IEssentialOilMoleculeService>();

			builder.RegisterType<EssentialOilEffectRepository>().As<IEssentialOilEffectRepository>();
			builder.RegisterType<EssentialOilEffectService>().As<IEssentialOilEffectService>();

			builder.RegisterType<EffectMoleculeRepository>().As<IEffectMoleculeRepository>();
			builder.RegisterType<EffectMoleculeService>().As<IEffectMoleculeService>();

			builder.RegisterType<MongoContext>().As<MongoContext>();

			//var container = builder.Build();
			//return container;
		}
	}
}