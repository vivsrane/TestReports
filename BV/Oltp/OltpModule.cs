using Autofac;
using VB.Common.Core.Membership;

namespace VB.DomainModel.Oltp
{
    public class OltpModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<HttpMemberContext>().As<IMemberContext>();
        }
    }
}