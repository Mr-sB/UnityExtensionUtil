using System.Text;
using GameUtil.AnimationCurveExtensions;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public static string AnimationCurveToJson(this AnimationCurve curve, bool prettyPrint = false)
        {
            return JsonUtility.ToJson(new AnimationCurveEx(curve), prettyPrint);
        }
        
        public static string AnimationCurveArrayToJson(this AnimationCurve[] curves, bool prettyPrint = false)
        {
            return JsonUtilityExtensions.ToJson(curves.AnimationCurveArray2ExAnimationCurveArray(), prettyPrint);
        }

        public static AnimationCurve AnimationCurveFromJson(string json)
        {
            return JsonUtilityExtensions.FromJson<AnimationCurveEx>(json);
        }
        
        public static AnimationCurve[] AnimationCurveArrayFromJson(string json)
        {
            return JsonUtilityExtensions.FromJson<AnimationCurveEx[]>(json).ExAnimationCurveArray2AnimationCurveArray();
        }
        
        public static AnimationCurve[] ExAnimationCurveArray2AnimationCurveArray(this AnimationCurveEx[] exCurves)
        {
            if (exCurves == null) return null;
            int length = exCurves.Length;
            AnimationCurve[] curves = new AnimationCurve[length];
            for (int i = 0; i < length; i++)
                curves[i] = exCurves[i];
            return curves;
        }

        public static AnimationCurveEx[] AnimationCurveArray2ExAnimationCurveArray(this AnimationCurve[] curves)
        {
            if (curves == null) return null;
            int length = curves.Length;
            AnimationCurveEx[] exCurves = new AnimationCurveEx[length];
            for (int i = 0; i < length; i++)
                exCurves[i] = curves[i];
            return exCurves;
        }
        #endregion

        #region Keyframe
        public static string KeyframeToJson(this Keyframe keyframe, bool prettyPrint = false)
        {
            return JsonUtility.ToJson(new KeyframeEx(keyframe), prettyPrint);
        }
        
        public static string KeyframeArrayToJson(this Keyframe[] keyframes, bool prettyPrint = false)
        {
            return JsonUtilityExtensions.ToJson(keyframes.KeyframeArray2ExKeyframeArray(), prettyPrint);
        }

        public static Keyframe KeyframeFromJson(string json)
        {
            return JsonUtilityExtensions.FromJson<KeyframeEx>(json);
        }
        
        public static Keyframe[] KeyframeArrayFromJson(string json)
        {
            return JsonUtilityExtensions.FromJson<KeyframeEx[]>(json).ExKeyframeArray2KeyframeArray();
        }

        public static Keyframe[] ExKeyframeArray2KeyframeArray(this KeyframeEx[] exKeyframes)
        {
            if (exKeyframes == null) return null;
            int length = exKeyframes.Length;
            Keyframe[] keyframes = new Keyframe[length];
            for (int i = 0; i < length; i++)
                keyframes[i] = exKeyframes[i];
            return keyframes;
        }

        public static KeyframeEx[] KeyframeArray2ExKeyframeArray(this Keyframe[] keyframes)
        {
            if (keyframes == null) return null;
            int length = keyframes.Length;
            KeyframeEx[] exKeyframes = new KeyframeEx[length];
            for (int i = 0; i < length; i++)
                exKeyframes[i] = keyframes[i];
            return exKeyframes;
        }
        #endregion
    }
}