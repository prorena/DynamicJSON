///////////////////////////////////////
// Part of Prorena dynamic JSON; an  //
// open-source converter for JObjects//
// into real dynamic ExpandoObjects. //
//                                   //
// Copyright © 2024 by Prorena GmbH. //
// Author: Florian Weihmann          //
// Licensed under GPL v.3            //
///////////////////////////////////////

using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Text.Json.Nodes;

namespace DynamicJSON
{
    /// <summary>
    /// Converts JSONs into dynamic ExpandoObjects
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts a string containing a JSON into a dynamic ExpandoObject
        /// </summary>
        /// <param name="json">The input JSON-string</param>
        /// <returns>A dynamic ExpandoObject representing the JSON strcture</returns>
        public static dynamic? ToDynamicJson(this string json)
        {
            return JObject.Parse(json).ToDynamic();
        }

#if NET8_0
        /// <summary>
        /// Converts JsonNode into a dynamic ExpandoObject
        /// </summary>
        /// <param name="json">The input JSON</param>
        /// <returns>A dynamic ExpandoObject representing the JSON strcture</returns>
        /// <see cref="System.Text.Json.Nodes.JsonNode"/>
        /// <see cref="System.Text.Json.Nodes.JsonObject"/>
        /// <see cref="System.Text.Json.Nodes.JsonValue"/>
        public static dynamic? ToDynamic(this JsonNode? json)
        {
            if (json == null)
                return null;

            if (json.GetType() == typeof(JsonArray))
            {
                var output = new List<object>();
                foreach (var item in json as JsonArray)
                    output.Add(item.ToDynamic());
                return output;
            }
            else if (json.GetType() == typeof(JsonObject))
            {
                var output = new ExpandoObject() as IDictionary<string, object?>;
                foreach (var item in json as JsonObject)
                    output.Add(item.Key, item.Value.ToDynamic());
                return output;
            }
            else
                switch (json.GetValueKind())
                {
                    case System.Text.Json.JsonValueKind.Number:
                        try { return json.GetValue<byte>(); } catch (Exception) { }
                        try { return json.GetValue<int>(); } catch (Exception) { }
                        return json.GetValue<double>();

                    case System.Text.Json.JsonValueKind.True:
                        return true;
                    case System.Text.Json.JsonValueKind.False:
                        return false;
                    case System.Text.Json.JsonValueKind.Undefined:
                    case System.Text.Json.JsonValueKind.Null:
                        return null;
                    default:
                    case System.Text.Json.JsonValueKind.String:
                        return json.ToString();

                }
        }
#endif

        /// <summary>
        /// Converts JObject / JToken into a dynamic ExpandoObject
        /// </summary>
        /// <param name="json">The input JSON</param>
        /// <returns>A dynamic ExpandoObject representing the JSON strcture</returns>
        /// <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// <see cref="Newtonsoft.Json.Linq.JObject"/>
        /// <see cref="Newtonsoft.Json.Linq.JProperty"/>
        public static dynamic? ToDynamic(this JToken? json)
        {
            if (json == null)
                return null;

            switch (json.Type)
            {
                case JTokenType.Object:
                    {
                        var output = new ExpandoObject() as IDictionary<string, object?>;
                        foreach (var item in (JObject)json)
                            output.Add(item.Key, item.Value.ToDynamic());
                        return output;
                    }

                case JTokenType.Array:
                    {
                        var output = new List<object>();
                        foreach (var item in json)
                            output.Add(item.ToDynamic());
                        return output;
                    }

                case JTokenType.Constructor:
                    {
                        var jConstructor = (JConstructor)json;
                        switch (jConstructor.Count)
                        {
                            case 0:
                                return null;

                            case 1:
                                return jConstructor.First.ToDynamic();

                            default:
                                if (jConstructor.First!.Type == JTokenType.String)
                                {
                                    var result = new ExpandoObject() as IDictionary<string, object?>;

                                    if (jConstructor.Count == 2)
                                        result.Add((string)jConstructor.First!, jConstructor.Last.ToDynamic());
                                    else
                                    {
                                        result.Add((string)jConstructor.First!, new List<object>());

                                        foreach (var item in jConstructor.Skip(1))
                                            ((List<object>)result[(string)jConstructor.First!]!).Add(item.ToDynamic());
                                    }
                                    return result;
                                }
                                else
                                {
                                    var output = new List<object>();
                                    foreach (var item in json)
                                        output.Add(item.ToDynamic());
                                    return output;
                                }
                        }
                    }

                case JTokenType.Property:
                    {
                        var result = new ExpandoObject() as IDictionary<string, object?>;
                        result.Add(((JProperty)json).Name, ((JProperty)json).Value.ToDynamic());
                        return result;
                    }
                case JTokenType.Integer:
                    return (int)json;
                case JTokenType.Float:
                    return (double)json;
                case JTokenType.Boolean:
                    return (bool)json;

                case JTokenType.None:
                case JTokenType.Null:
                case JTokenType.Undefined:
                case JTokenType.Comment:
                    return null;

                case JTokenType.Date:
                    return (DateTime)json;
                case JTokenType.Raw:
                    {
                        var result = (JToken?)((JRaw)json!)!.Value;
                        if (result == null)
                            return null;

                        return result.ToDynamic();
                    }
                case JTokenType.Bytes:
                    return (byte[]?)json;
                case JTokenType.Guid:
                    return new Guid(json.ToString());
                case JTokenType.Uri:
                    return new Uri(json.ToString());
                case JTokenType.TimeSpan:
                    return (TimeSpan)json;

                case JTokenType.String:
                default:
                    return json.ToString();
            }
        }
    }
}
