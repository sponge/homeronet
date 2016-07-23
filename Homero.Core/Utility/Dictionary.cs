using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Homero.Core.Utility
{
    public static class Dictionary
    {
        public static bool DictionaryEqual<TKey, TValue>(
            this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            return first.DictionaryEqual(second, null);
        }

        public static bool DictionaryEqual<TKey, TValue>(
            this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second,
            IEqualityComparer<TValue> valueComparer)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;

            valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            foreach (var kvp in first)
            {
                TValue secondValue;
                if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
                if (!valueComparer.Equals(kvp.Value, secondValue)) return false;
            }
            return true;
        }


        public static object GetObject(this Dictionary<string, object> dict, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop == null) continue;

                object value = kv.Value;
                if (value is Dictionary<string, object>)
                {
                    value = GetObject((Dictionary<string, object>) value, prop.PropertyType);
                }
                if (value is object[])
                {
                    if (typeof(IList).IsAssignableFrom(prop.PropertyType))
                    {
                        var listInstance = Activator.CreateInstance(prop.PropertyType);
                        Type listType = prop.PropertyType.GenericTypeArguments[0];
                        MethodInfo addMethod = listInstance.GetType().GetMethods().First(m => m.Name == "Add");

                        // They want a list, great.
                        foreach (object entry in (object[]) value)
                        {
                            if (entry is Dictionary<string, object>)
                            {
                                addMethod.Invoke(listInstance,
                                    new[] {((Dictionary<string, object>) entry).GetObject(listType)});
                            }
                            else
                            {
                                addMethod.Invoke(listInstance, new[] {Convert.ChangeType(entry, listType)});
                            }
                        }
                        prop.SetValue(obj, listInstance, null);
                        continue;
                    }
                }

                prop.SetValue(obj, value, null);
            }
            return obj;
        }

        public static T GetObject<T>(this Dictionary<string, object> dict)
        {
            return (T) GetObject(dict, typeof(T));
        }
    }
}