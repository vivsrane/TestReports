namespace VB.Common.Core.Registry
{
    public interface IRegistry : IResolver
    {
        IRegistry Register<TContract, TImplementation>(ImplementationScope implementationScope)
            where TImplementation : TContract, new();

        IResolver CreateScope();

        IRegistry Register<TModule>() where TModule : class, IModule, new();

        IRegistry Clone();
    }
}