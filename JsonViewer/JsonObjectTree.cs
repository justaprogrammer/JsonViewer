using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace EPocalipse.Json.Viewer
{
    public enum JsonType { Object, Array, Value };

    internal class JsonParseError : ApplicationException
    {
        public JsonParseError() : base() { }
        public JsonParseError(string message) : base(message) { }
        protected JsonParseError(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public JsonParseError(string message, Exception innerException) : base(message, innerException) { }

    }

    public class JsonObjectTree
    {
        private JsonObject _root;
        private static Regex dateRegex = new Regex("^/Date\\(([0-9]*)([+-][0-9]{4}){0,1}\\)/$");

        public static JsonObjectTree Parse(string json)
        {
            //Parse the JSON string
            object jsonObject;
            try
            {
                jsonObject = JavaScriptConvert.DeserializeObject(json);
            }
            catch (Exception e)
            {
                throw new JsonParseError(e.Message, e);
            }
            //Parse completed, build the tree
            return new JsonObjectTree(jsonObject);
        }

        public JsonObjectTree(object rootObject)
        {
            _root = ConvertToObject("JSON", rootObject);
        }

        private static JsonObject ConvertToObject(string id, object jsonObject)
        {
            JsonObject obj = CreateJsonObject(jsonObject);
            obj.Id = id;
            AddChildren(jsonObject, obj);
            return obj;
        }

        private static void AddChildren(object jsonObject, JsonObject obj)
        {
            JavaScriptObject javaScriptObject = jsonObject as JavaScriptObject;
            if (javaScriptObject != null)
            {
                foreach (KeyValuePair<string, object> pair in javaScriptObject)
                {
                    obj.Fields.Add(ConvertToObject(pair.Key, pair.Value));
                }
            }
            else
            {
                JavaScriptArray javaScriptArray = jsonObject as JavaScriptArray;
                if (javaScriptArray != null)
                {
                    for (int i = 0; i < javaScriptArray.Count; i++)
                    {
                        obj.Fields.Add(ConvertToObject("[" + i.ToString() + "]", javaScriptArray[i]));
                    }
                }
            }
        }

        private static JsonObject CreateJsonObject(object jsonObject)
        {
            JsonObject obj = new JsonObject();
            if (jsonObject is JavaScriptArray)
                obj.JsonType = JsonType.Array;
            else if (jsonObject is JavaScriptObject)
                obj.JsonType = JsonType.Object;
            else
            {
            	if (jsonObject is string) {
            		Match match = dateRegex.Match(jsonObject as string);
        			if (match.Success) {
            			// I'm not sure why this is match.Groups[1] and not match.Groups[0]
            			// we need to convert milliseconds to windows ticks (one tick is one hundred nanoseconds (e-9))
            			Int64 ticksSinceEpoch = Int64.Parse(match.Groups[1].Value) * (Int64)10e3;
            			jsonObject = DateTime.SpecifyKind(new DateTime(1970, 1, 1).Add(new TimeSpan(ticksSinceEpoch)), DateTimeKind.Utc);
            			// Take care of the timezone offset
            			if (!string.IsNullOrEmpty(match.Groups[2].Value)) {
            				Int64 timeZoneOffset = Int64.Parse(match.Groups[2].Value);
            				jsonObject = ((DateTime)jsonObject).AddHours(timeZoneOffset/100);
            				// Some timezones like India Tehran and Nepal have fractional offsets from GMT
            				jsonObject = ((DateTime)jsonObject).AddMinutes(timeZoneOffset%100);
            			}
            		}
            	}
                obj.JsonType = JsonType.Value;
                obj.Value = jsonObject;
            }
            return obj;
        }

        public JsonObject Root
        {
            get
            {
                return _root;
            }
        }

    }
}
