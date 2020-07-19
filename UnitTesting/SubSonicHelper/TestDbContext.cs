using Microsoft.Extensions.Logging;
using SubSonic.Collections;
using SubSonic.Extensions;
using SubSonic.Extensions.Test;
using SubSonic.Extensions.Test.NUnit;
using SubSonic;

namespace SubSonic.Extensions.Test
{
    public class TestSubSonicContext
        : SubSonic.SubSonicContext
    {
        public TestSubSonicContext()
            : base()
        {

        }

        public ISubSonicSetCollection<Models.Unit> Units { get; private set; }

        protected override void OnDbConfiguring(DbContextOptionsBuilder config)
        {
            config
                .ConfigureServiceCollection()
                .AddLogging((_config) => _config.AddNUnitLogger<TestSubSonicContext>(LogLevel.Trace))
                .UseMockDbClient((builder, options) =>
                {
                    builder
                        .SetDatasource("localhost")
                        .SetInitialCatalog("test")
                        .SetIntegratedSecurity(true);
                });
        }

        protected override void OnDbModeling(DbModelBuilder builder)
        {
            builder.AddEntityModel<Models.Unit>();
        }
    }
}
