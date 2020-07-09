using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Ems.Configure;
using Ems.Startup;
using Ems.Test.Base;

namespace Ems.GraphQL.Tests
{
    [DependsOn(
        typeof(EmsGraphQLModule),
        typeof(EmsTestBaseModule))]
    public class EmsGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EmsGraphQLTestModule).GetAssembly());
        }
    }
}