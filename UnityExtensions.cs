using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameUtil.Extensions
{
    public static class UnityExtensions
    {
        public static T Get<T>(this GameObject go) where T : Component
        {
            if (!go) return null;
            T t = go.GetComponent<T>();
            if (!t)
                t = go.AddComponent<T>();
            return t;
        }
        
        public static T Get<T>(this Component component) where T : Component
        {
            if (!component) return null;
            return Get<T>(component.gameObject);
        }

        public static void Destroy(this Object obj)
        {
            if(!obj) return;
#if UNITY_EDITOR
            if(EditorUtility.IsPersistent(obj)) return;
            if(Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
#else
            Object.Destroy(obj);
#endif
        }
        
        public static void DestroyImmediate(this Object obj)
        {
            if(!obj) return;
#if UNITY_EDITOR
            if(EditorUtility.IsPersistent(obj)) return;
#endif
            Object.DestroyImmediate(obj);
        }

        public static void SetActiveSafe(this GameObject go, bool active)
        {
            if(!go) return;
            go.SetActive(active);
        }
        
        public static void SetActiveSafe(this Component component, bool active)
        {
            if(!component || !component.gameObject) return;
            component.gameObject.SetActive(active);
        }

        public static float Random(this Vector2 vector2)
        {
            return vector2.x < vector2.y ? UnityEngine.Random.Range(vector2.x, vector2.y) : UnityEngine.Random.Range(vector2.y, vector2.x);
        }
        
        public static string GetTransformPath(this Transform transform)
        {
            StringBuilder sb = new StringBuilder(transform.name);
            while (transform.parent)
            {
                transform = transform.parent;
                sb.Insert(0, '/');
                sb.Insert(0, transform.name);
            }
            return sb.ToString();
        }
    }
}