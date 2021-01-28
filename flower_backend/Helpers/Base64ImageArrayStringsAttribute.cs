using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace flower.Helpers
{
    public class Base64ImageArrayStringsAttribute : ValidationAttribute
    {
        public string GetErrorMessage() =>
            $"Image invalid. Supported extension: gif|png|jpeg|bmp|jpg.";

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value != null)
            {
                string[] images = value as string[];
                foreach (string image in images)
                {
                    string ext = image.TrimStart(',').Split(',')[0];
                    if (!Regex.IsMatch(ext, @"data:image\/(?:gif|png|jpeg|bmp|jpg);base64") && !Regex.IsMatch(ext, @"^\d+$"))
                        return new ValidationResult(GetErrorMessage());
                }
            }
            return ValidationResult.Success;
        }
    }
}
