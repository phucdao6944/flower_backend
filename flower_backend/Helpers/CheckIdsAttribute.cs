using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Helpers
{
    public class CheckIdsAttribute : ValidationAttribute
    {
        public CheckIdsAttribute(string name)
        {
            this.name = name;
        }
        public string name { get; }
        public string GetErrorMessage() =>
            $"Invalid {name}.";

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            try
            {
                string categoryIds = value as string;
                int[] ids = Array.ConvertAll(categoryIds.Split(','), p => int.Parse(p.Trim()));
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(GetErrorMessage());
            }
        }
    }
}
