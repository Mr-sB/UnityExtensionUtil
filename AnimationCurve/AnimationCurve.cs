using UnityEngine;

namespace GameUtil.Extensions
{
    public class AnimationCurve
    {
        /// <summary>
        ///   <para>All keys defined in the animation curve.</para>
        /// </summary>
        public Keyframe[] keys;

        /// <summary>
        ///   <para>The behaviour of the animation before the first keyframe.</para>
        /// </summary>
        public WrapMode preWrapMode;

        /// <summary>
        ///   <para>The behaviour of the animation after the last keyframe.</para>
        /// </summary>
        public WrapMode postWrapMode;
        
        public AnimationCurve()
        {
            this.keys = null;
        }

        /// <summary>
        ///   <para>Creates an animation curve from an arbitrary number of keyframes.</para>
        /// </summary>
        /// <param name="keys">An array of Keyframes used to define the curve.</param>
        public AnimationCurve(params Keyframe[] keys)
        {
            this.keys = keys;
        }
        
        private AnimationCurve(params UnityEngine.Keyframe[] keys)
        {
            this.keys = keys.KeyframeArray2ExKeyframeArray();
        }
        
        public AnimationCurve(AnimationCurve curve) : this(curve.keys)
        {
            preWrapMode = curve.preWrapMode;
            postWrapMode = curve.postWrapMode;
        }

        public AnimationCurve(UnityEngine.AnimationCurve curve) : this(curve.keys)
        {
            preWrapMode = curve.preWrapMode;
            postWrapMode = curve.postWrapMode;
        }

        public Keyframe this[int index]
        {
            get { return keys[index]; }
        }

        /// <summary>
        ///   <para>The number of keys in the curve. (Read Only)</para>
        /// </summary>
        public int length
        {
            get { return keys.Length; }
        }

        /// <summary>
        ///   <para>Creates a constant "curve" starting at timeStart, ending at timeEnd and with the value value.</para>
        /// </summary>
        /// <param name="timeStart">The start time for the constant curve.</param>
        /// <param name="timeEnd">The start time for the constant curve.</param>
        /// <param name="value">The value for the constant curve.</param>
        /// <returns>
        ///   <para>The constant curve created from the specified values.</para>
        /// </returns>
        public static AnimationCurve Constant(
            float timeStart,
            float timeEnd,
            float value)
        {
            return Linear(timeStart, value, timeEnd, value);
        }

        /// <summary>
        ///   <para>A straight Line starting at timeStart, valueStart and ending at timeEnd, valueEnd.</para>
        /// </summary>
        /// <param name="timeStart">The start time for the linear curve.</param>
        /// <param name="valueStart">The start value for the linear curve.</param>
        /// <param name="timeEnd">The end time for the linear curve.</param>
        /// <param name="valueEnd">The end value for the linear curve.</param>
        /// <returns>
        ///   <para>The linear curve created from the specified values.</para>
        /// </returns>
        public static AnimationCurve Linear(
            float timeStart,
            float valueStart,
            float timeEnd,
            float valueEnd)
        {
            if (timeStart == timeEnd)
                return new AnimationCurve(new Keyframe(timeStart, valueStart));
            float num = (valueEnd - valueStart) / (timeEnd - timeStart);
            return new AnimationCurve(new Keyframe(timeStart, valueStart, 0.0f, num), new Keyframe(timeEnd, valueEnd, num, 0.0f));
        }

        /// <summary>
        ///   <para>Creates an ease-in and out curve starting at timeStart, valueStart and ending at timeEnd, valueEnd.</para>
        /// </summary>
        /// <param name="timeStart">The start time for the ease curve.</param>
        /// <param name="valueStart">The start value for the ease curve.</param>
        /// <param name="timeEnd">The end time for the ease curve.</param>
        /// <param name="valueEnd">The end value for the ease curve.</param>
        /// <returns>
        ///   <para>The ease-in and out curve generated from the specified values.</para>
        /// </returns>
        public static AnimationCurve EaseInOut(
            float timeStart,
            float valueStart,
            float timeEnd,
            float valueEnd)
        {
            return (double) timeStart == (double) timeEnd
                ? new AnimationCurve(new Keyframe(timeStart, valueStart))
                : new AnimationCurve(new Keyframe(timeStart, valueStart, 0.0f, 0.0f), new Keyframe(timeEnd, valueEnd, 0.0f, 0.0f));
        }

        public static implicit operator AnimationCurve(UnityEngine.AnimationCurve curve)
        {
            return new AnimationCurve(curve);
        }
        
        public static implicit operator UnityEngine.AnimationCurve(AnimationCurve curve)
        {
            return new UnityEngine.AnimationCurve(curve.keys.ExKeyframeArray2KeyframeArray())
            {
                preWrapMode = curve.preWrapMode,
                postWrapMode = curve.postWrapMode
            };
        }
    }
}