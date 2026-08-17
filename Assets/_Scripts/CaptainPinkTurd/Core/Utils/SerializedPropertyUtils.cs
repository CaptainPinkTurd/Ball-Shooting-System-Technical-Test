using System;
using System.Reflection;

namespace CaptainPinkTurd.Core.Utils
{
    public class SerializedPropertyUtils
    {
        public static object Walk(object obj, string path)
        {
            if (obj == null) return null;

            if (path.Contains("["))
            {
                string fieldName = path.Substring(0, path.IndexOf("["));
                int index = Convert.ToInt32(path.Substring(path.IndexOf("[")).Replace("[", "").Replace("]", ""));

                return GetIndexedValue(obj, fieldName, index);
            }

            return GetFieldOrProp(obj, path);
        }

        public static object GetIndexedValue(object source, string name, int index)
        {
            if (source == null) return null;

            var field = source.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null) return null;

            var list = field.GetValue(source) as System.Collections.IList;
            if (list == null || index < 0 || index >= list.Count) return null;

            return list[index];
        }

        public static object GetFieldOrProp(object source, string name)
        {
            var t = source.GetType();

            var field = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null) return field.GetValue(source);

            var prop = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null) return prop.GetValue(source);

            return null;
        }
    }
}