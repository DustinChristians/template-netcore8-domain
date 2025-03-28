using System.Linq;
using System.Reflection;

namespace CompanyName.ProjectName.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool AllPropertiesAreNull<T>(this T obj)
        {
            return obj == null || typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .All(property => property.GetValue(obj) == null);
        }

        public static T AllStringsToLower<T>(this T obj)
        {
            if (obj == null)
            {
                return obj;
            }

            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType == typeof(string) && property.CanWrite)
                {
                    var value = property.GetValue(obj) as string;
                    if (value != null)
                    {
                        property.SetValue(obj, value.ToLower());
                    }
                }
            }

            return obj;
        }
    }
}
