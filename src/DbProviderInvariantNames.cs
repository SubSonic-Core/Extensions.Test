using System;
using System.Collections.Generic;
using System.Text;

namespace SubSonic.Extensions.Test
{
    using MockDbClient;

    public static class DbProviderInvariantNames
    {
        public static string MockDbProviderInvariantName => typeof(MockDbClientFactory).Namespace;
        public static string SqlServiceDbProviderInvariantName => "Microsoft.Data.SqlClient";
    }
}
