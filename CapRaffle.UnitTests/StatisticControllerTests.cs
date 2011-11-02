using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    public class StatisticControllerTests : Shared
    {
        [Test]
        public void Can_Get_Category_Statistics_View()
        {
            var result = statisticController.Category(1);
            Assert.IsInstanceOf(typeof(ActionResult), result);
        }

        [Test]
        public void Can_Get_User_Statistic_Partial_View()
        {
            var result = statisticController.UserStatisticPartial();
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }
    }
}
