using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Configuration;

namespace CapRaffle.Domain.Model
{
    public class RequiredAndValidEmailAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            var domain = ConfigurationManager.AppSettings["EmailDomain"];
            ErrorMessage = ErrorMessage.Replace("domain", domain);

            if (value == null)
                return false;

            var email = value as string;
            if (!email.ToLower().Contains("@" + domain.ToLower()))
                return false;

            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var domain = ConfigurationManager.AppSettings["EmailDomain"];
            ErrorMessage = ErrorMessage.Replace("domain", domain);
            var rule = new ModelClientValidationRule() { ErrorMessage = this.ErrorMessage, ValidationType = "validemail" };
            yield return rule;
        }
    }
}
