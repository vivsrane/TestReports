using Autofac;

namespace VB.Common.Core.IOC
{
    public interface IRegistryModule
    {
        void Register(ContainerBuilder builder);
    }
}