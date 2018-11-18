using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Unity;
using Unity.Exceptions;

namespace Duftfinder.Ioc.Helpers
{
    /// <summary>
    /// Resolves dependencies using Unity.
    /// </summary>
    /// <author>Anna Krebs</author>
    /// <seealso> href="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection">docs.microsoft</seealso>  
    public class UnityDependencyResolver : IDependencyResolver
    {
        protected IUnityContainer Container;

        public UnityDependencyResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            this.Container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return Container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return Container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }
    }
}
