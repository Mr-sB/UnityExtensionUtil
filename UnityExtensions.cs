using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
        
        public static float Random(this Vector2Int vector2)
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

        #region AnimationCurve
        public static string AnimationCurveToJson(this UnityEngine.AnimationCurve curve, bool prettyPrint = false)
        {
            return JsonUtility.ToJson(new AnimationCurve(curve), prettyPrint);
        }
        
        public static string AnimationCurveArrayToJson(this UnityEngine.AnimationCurve[] curves, bool prettyPrint = false)
        {
            return JsonUtilityExtensions.ToJson(curves.AnimationCurveArray2ExAnimationCurveArray(), prettyPrint);
        }

        public static UnityEngine.AnimationCurve AnimationCurveFromJson(string json)
        {
            return JsonUtilityExtensions.FromJson<AnimationCurve>(json);
        }
        
        public static UnityEngine.AnimationCurve[] AnimationCurveArrayFromJson(string json)
        {
            return JsonUtilityExtensions.FromJson<AnimationCurve[]>(json).ExAnimationCurveArray2AnimationCurveArray();
        }
        
        public static UnityEngine.AnimationCurve[] ExAnimationCurveArray2AnimationCurveArray(this AnimationCurve[] exCurves)
        {
            if (exCurves == null) return null;
            int length = exCurves.Length;
            UnityEngine.AnimationCurve[] curves = new UnityEngine.AnimationCurve[length];
            for (int i = 0; i < length; i++)
                curves[i] = exCurves[i];
            return curves;
        }

        public static AnimationCurve[] AnimationCurveArray2ExAnimationCurveArray(this UnityEngine.AnimationCurve[] curves)
        {
            if (curves == null) return null;
            int length = curves.Length;
            AnimationCurve[] exCurves = new AnimationCurve[length];
            for (int i = 0; i < length; i++)
                exCurves[i] = curves[i];
            return exCurves;
        }
        #endregion

        #region Keyframe
        public static string KeyframeToJson(this UnityEngine.Keyframe keyframe, bool prettyPrint = false)
        {
            return JsonUtility.ToJson(new Keyframe(keyframe), prettyPrint);
        }
        
        public static string KeyframeArrayToJson(this UnityEngine.Keyframe[] keyframes, bool prettyPrint = false)
        {
            return JsonUtilityExtensions.ToJson(keyframes.KeyframeArray2ExKeyframeArray(), prettyPrint);
        }

        public static UnityEngine.Keyframe KeyframeFromJson(string json)
        {
            return JsonUtilityExtensions.FromJson<Keyframe>(json);
        }
        
        public static UnityEngine.Keyframe[] KeyframeArrayFromJson(string json)
        {
            return JsonUtilityExtensions.FromJson<Keyframe[]>(json).ExKeyframeArray2KeyframeArray();
        }

        public static UnityEngine.Keyframe[] ExKeyframeArray2KeyframeArray(this Keyframe[] exKeyframes)
        {
            if (exKeyframes == null) return null;
            int length = exKeyframes.Length;
            UnityEngine.Keyframe[] keyframes = new UnityEngine.Keyframe[length];
            for (int i = 0; i < length; i++)
                keyframes[i] = exKeyframes[i];
            return keyframes;
        }

        public static Keyframe[] KeyframeArray2ExKeyframeArray(this UnityEngine.Keyframe[] keyframes)
        {
            if (keyframes == null) return null;
            int length = keyframes.Length;
            Keyframe[] exKeyframes = new Keyframe[length];
            for (int i = 0; i < length; i++)
                exKeyframes[i] = keyframes[i];
            return exKeyframes;
        }
        #endregion
    }
}