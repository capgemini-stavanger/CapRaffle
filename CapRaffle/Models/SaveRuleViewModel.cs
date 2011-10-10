using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapRaffle.Models
{
    public class SaveRuleViewModel
    {
        public int RuleId { get; set; }
        public int Param { get; set; }

    }

    public class RuleComparer : IEqualityComparer<SaveRuleViewModel>
    {
        public bool Equals(SaveRuleViewModel x, SaveRuleViewModel y)
        {
            if(object.ReferenceEquals(x,y)) return true;
            if(object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;
            return x.RuleId == y.RuleId;
        }

        public int GetHashCode(SaveRuleViewModel rule)
        {
            if(object.ReferenceEquals(rule, null)) return 0;
            return rule.RuleId == 0 ? 0 : rule.RuleId.GetHashCode();
        }
    }
}