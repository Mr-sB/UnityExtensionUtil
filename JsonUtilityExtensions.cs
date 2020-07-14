using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtil.Extensions
{
    public static class JsonUtilityExtensions
    {
        [Serializable]
        public class Pack<T>
        {
            public T Data;

            public Pack() { }
            public Pack(T data)
            {
                Data = data;
            }
        }
        
        public static string ToJson<T>(T obj, bool prettyPrint = false)
        {
            if (obj == null) return string.Empty;
            return typeof(T).IsArrayOrList() ? JsonUtility.ToJson(new Pack<T>(obj), prettyPrint) : JsonUtility.ToJson(obj, prettyPrint);
        }
        
        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default;
            if (!typeof(T).IsArrayOrList())
                return JsonUtility.FromJson<T>(json);
            var pack = JsonUtility.FromJson<Pack<T>>(json);
            return pack == null ? default : pack.Data;
        }

        private static bool IsArrayOrList(this Type type)
        {
            return type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}