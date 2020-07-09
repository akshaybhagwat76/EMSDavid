using Abp.Dependency;
using GraphQL;
using GraphQL.Types;
using Ems.Queries.Container;

namespace Ems.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IDependencyResolver resolver) :
            base(resolver)
        {
            Query = resolver.Resolve<QueryContainer>();
        }
    }
}