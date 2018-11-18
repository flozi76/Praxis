using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Duftfinder.Business.Services;
using Duftfinder.Database.Repositories;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;
using Unity;

namespace Duftfinder.Ioc.Helpers
{
    /// <summary>
    /// Creates a container of dependency objects.
    /// Registers the interfaces and it's implementation in the container.
    /// Is called in Global.asax of web project.
    /// </summary>
    /// <author>Anna Krebs</author>
    /// <seealso> href="http://www.c-sharpcorner.com/UploadFile/dacca2/implement-ioc-using-unity-in-mvc-5/">c-sharpcorner</seealso>  
    public static class ContainerConfig
    {
        public static IUnityContainer Initialize()
        {
            IUnityContainer container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            return container;
        }

        /// <summary>
        /// Registers dependencies of the components.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso> href="http://www.c-sharpcorner.com/UploadFile/dacca2/implement-ioc-using-unity-in-mvc-5/">c-sharpcorner</seealso> 
        /// <returns></returns>
        private static IUnityContainer BuildUnityContainer()
        {
            IUnityContainer container = new UnityContainer();

            // Register all components with the container here. 
            container.RegisterType<ISettingsRepository, SettingsRepository>();
            container.RegisterType<ISettingsService, SettingsService>();

            container.RegisterType<IConfigurationRepository, ConfigurationRepository>();
            container.RegisterType<IConfigurationService, ConfigurationService>();

            container.RegisterType<IAuthenticationService, AuthenticationService>();

            container.RegisterType<ISmtpEmailService, SmtpEmailService>();

            container.RegisterType<IEmailService, EmailService>();

            container.RegisterType<ICryptoService, CryptoService>();

            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUserService, UserService>();

            container.RegisterType<IRoleRepository, RoleRepository>();
            
            container.RegisterType<IEssentialOilRepository, EssentialOilRepository>();
            container.RegisterType<IEssentialOilService, EssentialOilService>();

            container.RegisterType<IEffectRepository, EffectRepository>();
            container.RegisterType<IEffectService, EffectService>();

            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<ICategoryService, CategoryService>();

            container.RegisterType<ISubstanceRepository, SubstanceRepository>();
            container.RegisterType<ISubstanceService, SubstanceService>();

            container.RegisterType<IMoleculeRepository, MoleculeRepository>();
            container.RegisterType<IMoleculeService, MoleculeService>();

            container.RegisterType<IEssentialOilMoleculeRepository, EssentialOilMoleculeRepository>();
            container.RegisterType<IEssentialOilMoleculeService, EssentialOilMoleculeService>();

            container.RegisterType<IEssentialOilEffectRepository, EssentialOilEffectRepository>();
            container.RegisterType<IEssentialOilEffectService, EssentialOilEffectService>();

            container.RegisterType<IEffectMoleculeRepository, EffectMoleculeRepository>();
            container.RegisterType<IEffectMoleculeService, EffectMoleculeService>();

            RegisterTypes(container);
            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {

        }
    }
}
