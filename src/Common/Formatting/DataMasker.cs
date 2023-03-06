using System;
using System.Text.RegularExpressions;

namespace Trainline.PromocodeService.Common.Formatting
{
    public static class DataMasker
    {
        private static string Pattern = @"(?<=[\w]{1})[\w-\._\+%\\]*(?=[\w]{1}@)|(?<=@[\w]{1})[\w-_\+%]*(?=\.)";

        public static string MaskEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return email;
            }
            if (!email.Contains("@"))
                return new String('*', email.Length);
            if (email.Split('@')[0].Length < 4)
                return @"*@*.*";
            return Regex.Replace(email, Pattern, m => new string('*', m.Length));
        }
    }
}
