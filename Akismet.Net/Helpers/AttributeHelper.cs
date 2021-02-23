using Akismet.Net.Attributes;
using RestSharp.Extensions;
using System.Collections.Generic;
using System.Reflection;

namespace Akismet.Net.Helpers
{
    internal static class AttributeHelper
    {
        public static List<KeyValuePair<string, string>> GetAttributes(object model)
        {
            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            string honeypotName = "";
            foreach (var property in model.GetType().GetRuntimeProperties())
            {
                if (property.GetValue(model) != null)
                {
                    if (property.Name == nameof(AkismetComment.HoneypotFieldName))
                        honeypotName = property.GetValue(model).ToString();

                    if (property.Name == nameof(AkismetComment.HoneypotFieldValue))
                    {
                        l.Add(new KeyValuePair<string, string>(honeypotName, property.GetValue(model).ToString()));
                    }
                    else
                    {
                        if (property.GetAttribute<AkismetNameAttribute>() is AkismetNameAttribute attribute)
                            l.Add(new KeyValuePair<string, string>(attribute.AkismetName, property.GetValue(model).ToString()));
                        else
                            l.Add(new KeyValuePair<string, string>(property.Name, property.GetValue(model).ToString()));
                    }
                }
            }

            return l;
        }
    }
}
