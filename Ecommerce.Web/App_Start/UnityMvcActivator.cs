using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Ecommerce.Web.App_Start.UnityMvcActivator), "Start")]

namespace Ecommerce.Web.App_Start
{
    /// <summary>
    /// Wires Unity as MVC's dependency resolver and filter provider. Runs
    /// before Application_Start via WebActivatorEx.
    /// </summary>
    public static class UnityMvcActivator
    {
        public static void Start()
        {
            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}