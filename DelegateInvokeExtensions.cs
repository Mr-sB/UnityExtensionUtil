using System;
using UnityEngine;

namespace GameUtil.Extensions
{
    public static class DelegateInvokeExtensions
    {
        #region Action
        public static void SafeInvoke(this Action action)
        {
            if(action == null) return;
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public static void SafeInvoke<T>(this Action<T> action, T arg)
        {
            if(action == null) return;
            try
            {
                action(arg);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if(action == null) return;
            try
            {
                action(arg1, arg2);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if(action == null) return;
            try
            {
                action(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        #endregion

        #region Func
        public static TResult SafeInvoke<TResult>(this Func<TResult> func)
        {
            if(func == null) return default;
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }
        
        public static TResult SafeInvoke<T, TResult>(this Func<T, TResult> func, T arg)
        {
            if(func == null) return default;
            try
            {
                return func(arg);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }
        
        public static TResult SafeInvoke<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            if(func == null) return default;
            try
            {
                return func(arg1, arg2);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }
        
        public static TResult SafeInvoke<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
        {
            if(func == null) return default;
            try
            {
                return func(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }
        #endregion
    }
}