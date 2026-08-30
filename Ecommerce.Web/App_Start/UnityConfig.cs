using System;
using Microsoft.Practices.Unity;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Data;
using Ecommerce.Data.Infrastructure;
using Ecommerce.Data.Repositories;
using Ecommerce.Services;

namespace Ecommerce.Web.App_Start
{
    /// <summary>
    /// Unity DI container for the MVC 5 front-end. Repositories and the
    /// DbContext are registered per HTTP request (hierarchical lifetime
    /// manager — Unity.Mvc creates a child container per request and
    /// disposes it at the end of the request).
    /// </summary>
    public static class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> Container =
            new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();
                RegisterTypes(container);
                return container;
            });

        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<EcommerceDbContext>(
    new HierarchicalLifetimeManager(),
    new InjectionConstructor("EcommerceDb"));

            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<ICartRepository, CartRepository>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());

            container.RegisterType<ICatalogService, CatalogService>();
            container.RegisterType<ICartService, CartService>();
            container.RegisterType<ICheckoutService, CheckoutService>();
            container.RegisterType<IAccountService, AccountService>();
        }
    }
}