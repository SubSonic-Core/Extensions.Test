using FluentAssertions;
using NUnit.Framework;
using SubSonic.Extensions.Test.Models;
using System.Collections.Generic;
using System.Data;
using UnitTesting.SubSonicHelper.Classes;

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
            List<RenterView> list = new List<RenterView>()
            { 
                new RenterView() { PersonID = 1, FullName = "Test", UnitID = 1}
            };

            DataTable data = list.ToDataTable();

            data.Columns[nameof(RenterView.PersonID)].Should().NotBeNull();

            data.Dispose();
        }
    }
}