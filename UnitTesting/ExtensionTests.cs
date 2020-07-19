using FluentAssertions;
using NUnit.Framework;
using SubSonic.Extensions.Test.Models;
using System.Collections.Generic;
using System.Data;

namespace SubSonic.Extensions.Test
{
    public class ExtensionTests
    {
        [SetUp]
        public void Setup()
        {
            new TestSubSonicContext();
        }

        [Test]
        public void CanTransformEnumberableIntoDataTable()
        {
            List<Person> list = new List<Person>();

            DataTable data = list.ToDataTable();

            data.Columns[nameof(Person.ID)].Should().NotBeNull();
        }
    }
}