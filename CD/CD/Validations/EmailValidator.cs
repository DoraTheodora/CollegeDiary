using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CD.Validations;

namespace CD.Validations
{
    public class EmailValidator: IValidationRule<string>
    {
        const string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
        public string Description => "Please enter a valid email.";

        public bool Validate(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(value);
        }
    }
}
