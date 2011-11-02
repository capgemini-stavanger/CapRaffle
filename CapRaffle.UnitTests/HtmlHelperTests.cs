using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Moq;
using System.Web;
using CapRaffle.HtmlHelpers;
using CapRaffle.Models;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    class HtmlHelperTests
    {
        HtmlHelper helper;

        public void setup()
        {
            var vc = new ViewContext();
            var httpContext = new Mock<HttpContextBase>();
            var viewdata = new Mock<IViewDataContainer>();
            vc.HttpContext = httpContext.Object;
            helper = new HtmlHelper(vc, viewdata.Object);
        }


        [Test]
        public void Can_return_correct_pagelinks_when_total_pages_are_less_than_6()
        {
            
            PagingInfo pi = new PagingInfo
            {
                CurrentPage = 1,
                Archive = true,
                ItemsPerPage = 2,
                TotalItems = 5
            };


            var result = PagingHelpers.PageLinks(helper, pi, x => "index?page=" + x);
            var expected = "<a class=\"selected\" href=\"?page=1&amp;Archive=true\">1</a>";
            expected += "<a href=\"?page=2&amp;Archive=true\">2</a>";
            expected += "<a href=\"?page=3&amp;Archive=true\">3</a>";
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        public void Can_return_correct_pagelinks_when_total_pages_are_more_than_6()
        {
            PagingInfo pi = new PagingInfo
            {
                CurrentPage = 1,
                Archive = true,
                ItemsPerPage = 2,
                TotalItems = 15
            };

            var result = PagingHelpers.PageLinks(helper, pi, x => "index?page=" + x);
            var expected = "<a class=\"selected\" href=\"?page=1&amp;Archive=true\">1</a>";
            expected += "<a href=\"?page=2&amp;Archive=true\">2</a>";
            expected += "<a href=\"?page=3&amp;Archive=true\">3</a>..";
            expected += "<a href=\"?page=8&amp;Archive=true\">8</a>";

            Assert.AreEqual(expected, result.ToString());
            Assert.IsTrue(result.ToString().Contains(".."));
        }

        [Test]
        public void Can_return_correct_pagelinks_when_total_pages_are_more_than_6_and_currentpage_is_larger_than_4()
        {
            PagingInfo pi = new PagingInfo
            {
                CurrentPage = 5,
                Archive = true,
                ItemsPerPage = 2,
                TotalItems = 15
            };

            var result = PagingHelpers.PageLinks(helper, pi, x => "index?page=" + x);
            var expected = "<a href=\"?page=1&amp;Archive=true\">1</a>..";
            expected += "<a href=\"?page=3&amp;Archive=true\">3</a>";
            expected += "<a href=\"?page=4&amp;Archive=true\">4</a>";
            expected += "<a class=\"selected\" href=\"?page=5&amp;Archive=true\">5</a>";
            expected += "<a href=\"?page=6&amp;Archive=true\">6</a>";
            expected += "<a href=\"?page=7&amp;Archive=true\">7</a>";
            expected += "<a href=\"?page=8&amp;Archive=true\">8</a>";

            Assert.AreEqual(expected, result.ToString());
            Assert.IsTrue(result.ToString().Contains(".."));
        }
    }
}
