using Autofac;
using VB.Common.Core.Cache;
using VB.Common.Core.Data;
using VB.Common.Core.Logging;

namespace VB.Common.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new DotnetMemoryCacheWrapper()).As(typeof(ICache), typeof(IMemoryCache)).SingleInstance();
            builder.Register(c => new LoggerFactory()).As<ILoggerFactory>().SingleInstance();
            builder.RegisterType<DbConnectionFactory>().As<IDbConnectionFactory>().SingleInstance();
        }
    }
}