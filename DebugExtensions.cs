using System.Collections.Generic;
using UnityEngine;

namespace GameUtil.Extensions
{
    public static class DebugExtensions
    {
        public static string GetString(params object[] objs)
        {
            if (objs == null || objs.Length <= 0) return "";
            string str = objs[0] == null ? "null" : objs[0].ToString();
            for (int i = 1, len = objs.Length; i < len; i++)
                str += '\t' + (objs[i] == null ? "null" : objs[i].ToString());
            return str;
        }
        
        public static string GetString<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 0) return "";
            string str = list[0] == null ? "null" : list[0].ToString();
            for (int i = 1, len = list.Count; i < len; i++)
                str += '\t' + (list[i] == null ? "null" : list[i].ToString());
            return str;
        }
        
        public static string ToFullString(this Vector3 vector3)
        {
            return string.Format("({0},{1},{2})", vector3.x, vector3.y, vector3.z);
        }
        
        public static string ToFullString(this Vector2 vector2)
        {
            return string.Format("({0},{1})", vector2.x, vector2.y);
        }
    }
}