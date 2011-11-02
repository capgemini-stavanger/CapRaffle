using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CapRaffle.Models;

namespace CapRaffle.UnitTests
{
    [TestFixture]
    class ViewModelTests
    {
        SaveRuleViewModel ruleX;
        SaveRuleViewModel ruleY;
        RuleComparer comparer;

        [SetUp]
        public void Setup()
        {
            ruleX = new SaveRuleViewModel { Param = 1, RuleId = 1 };
            ruleY = new SaveRuleViewModel { Param = 1, RuleId = 1 };
            comparer = new RuleComparer();
        }

        [Test]
        public void RuleComparer_returns_true_on_equal_ruleId()
        {
            var result = comparer.Equals(ruleX, ruleY);
            Assert.IsTrue(result);
        }

        [Test]
        public void RuleComparer_returns_true_on_equal_object()
        {
            var result = comparer.Equals(ruleX, ruleX);
            Assert.IsTrue(result);
        }

        [Test]
        public void RuleComparer_returns_false_when_either_of_the_rules_are_null()
        {
            var result1 = comparer.Equals(ruleX, null);
            var result2 = comparer.Equals(null, ruleY);

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [Test]
        public void rulecomparer_HashCode_returns_0_on_empty_object()
        {
            var haschode = comparer.GetHashCode(null);
            Assert.AreEqual(0, haschode);
        }
    }
}
