namespace eVision.eCrypt.KeyGenerator.Helpers.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class PropertyInfoExtensions
    {
        public static IEnumerable<T> GetCustomAttributes<T>(this PropertyInfo info, bool inherit = false)
            where T : Attribute
        {
            return info.GetCustomAttributes(typeof (T), inherit).Cast<T>();
        }
    }
}
