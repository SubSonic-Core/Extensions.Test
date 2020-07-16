namespace SubSonic.Extensions.Test
{
    using MockDbClient;
    using SubSonic.Extensions.SqlServer;
    using SubSonic.SqlGenerator;

    class MockSqlQueryProvider
        : SqlQueryProvider
    {
        public static readonly MockSqlQueryProvider Instance = new MockSqlQueryProvider();

        public MockSqlQueryProvider()
            : base(new SqlContext<AnsiSqlFragment, MockSqlMethods>())
        {

        }

        public override string ClientName => typeof(MockDbClientFactory).FullName;
    }
}
