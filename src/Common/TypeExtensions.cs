using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trainline.PromocodeService.Common
{
    public static class TypeExtensions
    {
        public static IList<string> GetAllStringFields(this Type me) =>
            me
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(fi => fi.GetValue(null))
                .OfType<string>()
                .ToList();
    }
}
