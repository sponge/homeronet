using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Homero.Core.Utility
{
    public static class Json
    {
        public static Dictionary<string, object> CleanDictionary(Dictionary<string, object> input)
        {
            if (input == null)
            {
                return null;
            }
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> keyValuePair in input)
            {
                if (keyValuePair.Value is JArray)
                {
                    List<object> res = ((JArray)keyValuePair.Value).ToObject<List<object>>();
                    List<object> cleanedArray = new List<object>();
                    foreach (object re in res)
                    {
                        if (re is JObject)
                        {
                            cleanedArray.Add(CleanDictionary(((JObject)re).ToObject<Dictionary<string, object>>()));
                        }
                        else if (re is Dictionary<string, object>)
                        {
                            cleanedArray.Add(CleanDictionary((Dictionary<string, object>)re));
                        }
                        else
                        {
                            cleanedArray.Add(re);
                        }
                    }
                    result.Add(keyValuePair.Key, cleanedArray.ToArray());
                }
                else if (keyValuePair.Value is Dictionary<string, object>)
                {
                    result.Add(keyValuePair.Key, CleanDictionary((Dictionary<string, object>)keyValuePair.Value));
                }
                else if (keyValuePair.Value is JObject)
                {
                    result.Add(keyValuePair.Key, CleanDictionary(((JObject)keyValuePair.Value).ToObject<Dictionary<string, object>>()));
                }
                else if (keyValuePair.Value is List<object>)
                {
                    result.Add(keyValuePair.Key, ((List<object>)keyValuePair.Value).ToList());
                }
                else
                {
                    result.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            return result;
        }
    }
}