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
        public void CanTransformEnumberableIntoDataTableForNonEntity()
        {
            List<RenterView> list = new List<RenterView>()
            { 
                new RenterView() { PersonID = 1, FullName = "Test", UnitID = 1}
            };

            DataTable data = list.ToDataTable();

            data.Columns[nameof(RenterView.PersonID)].Should().NotBeNull();

            data.Dispose();
        }

        [Test]
        public void CanTransformEnumberableIntoDataTableForEntity()
        {
            List<Unit> list = new List<Unit>()
            {
                new Unit() { ID = 1, NumberOfBedrooms = 1, RealEstatePropertyID = 1, StatusID = 1 }
            };

            DataTable data = list.ToDataTable();

            data.Columns["Bedrooms"].Should().NotBeNull();

            data.Dispose();
        }
    }
}