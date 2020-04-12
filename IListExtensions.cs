using System;
using System.Collections.Generic;

namespace GameUtil.Extensions
{
    public static class IListExtensions
    {
        #region Sort
        public static T[] Sort<T>(this T[] array, IComparer<T> comparer)
        {
            if (array == null || array.Length <= 1 || comparer == null) return array;
            Array.Sort(array, comparer);
            return array;
        }
        
        public static T[] Sort<T>(this T[] array, Comparison<T> comparison)
        {
            if (array == null || array.Length <= 1 || comparison == null) return array;
            Array.Sort(array, comparison);
            return array;
        }
        
        public static IList<T> Sort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list == null || list.Count <= 1 || comparison == null) return list;
            list.Sort(0, list.Count - 1, comparison);
            return list;
        }

        public static IList<T> Sort<T>(this IList<T> list, int startIndex, int endIndex, Comparison<T> comparison)
        {
            if (list == null || list.Count <= 1 || comparison == null || startIndex < 0 || endIndex >= list.Count || startIndex >= endIndex) return list;
            list.SortInternal(startIndex, endIndex, comparison);
            return list;
        }
        
        private static void SortInternal<T>(this IList<T> list, int startIndex, int endIndex, Comparison<T> comparison)
        {
            //无需错误检查，因为在调用SortInternal之前已经经过检查了，避免递归时重复检查消耗性能
            //结束条件
            if(startIndex >= endIndex) return;
            int leftIndex = startIndex;
            int rightIndex = endIndex;
            //取最左边的为基准数
            T tmp = list[startIndex];
            while (leftIndex < rightIndex)
            {
                //先从右开始遍历比较
                while (comparison(list[rightIndex], tmp) >= 0 && leftIndex < rightIndex)
                    rightIndex--;
                //再从左开始遍历比较
                while (comparison(list[leftIndex], tmp) <= 0 && leftIndex < rightIndex)
                    leftIndex++;
                //交换
                if (leftIndex < rightIndex)
                {
                    var t = list[leftIndex];
                    list[leftIndex] = list[rightIndex];
                    list[rightIndex] = t;
                }
            }
            //基准数归位
            list[startIndex] = list[leftIndex];
            list[leftIndex] = tmp;
            //递归左
            list.SortInternal(startIndex, leftIndex - 1, comparison);
            //递归右
            list.SortInternal(leftIndex + 1, endIndex, comparison);
        }
        #endregion

        #region Find
        public static int FindIndex<T>(this IList<T> list, Predicate<T> match)
        {
            if (list == null || list.Count <= 0 || match == null) return -1;
            for (int i = 0, count = list.Count; i < count; i++)
                if (match(list[i])) return i;
            return -1;
        }
        
        public static int FindLastIndex<T>(this IList<T> list, Predicate<T> match)
        {
            if (list == null || list.Count <= 0 || match == null) return -1;
            int lastIndex = -1;
            for (int i = 0, count = list.Count; i < count; i++)
                if (match(list[i]))
                    lastIndex = i;
            return lastIndex;
        }
        
        public static int FindMinIndex<T1, T2>(this IList<T1> list, Func<T1, T2> comparer) where T2 : IComparable<T2>
        {
            if (list == null || list.Count <= 0 || comparer == null) return -1;
            int count = list.Count;
            if (count == 1) return 0;
            int minIndex = -1;
            T2 min = default;
            for (int i = 0; i < count; i++)
            {
                var current = comparer(list[i]);
                if (minIndex == -1 || current.CompareTo(min) < 0)
                {
                    min = current;
                    minIndex = i;
                }
            }
            return minIndex;
        }
        
        public static int FindMaxIndex<T1, T2>(this IList<T1> list, Func<T1, T2> comparer) where T2 : IComparable<T2>
        {
            if (list == null || list.Count <= 0 || comparer == null) return -1;
            int count = list.Count;
            if (count == 1) return 0;
            int maxIndex = -1;
            T2 max = default;
            for (int i = 0; i < count; i++)
            {
                var current = comparer(list[i]);
                if (maxIndex == -1 || current.CompareTo(max) > 0)
                {
                    max = current;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }
        
        public static bool TryFind<T>(this IList<T> list, Predicate<T> match, out T output)
        {
            output = default;
            if (list == null || list.Count <= 0 || match == null) return false;
            int index = list.FindIndex(match);
            if (index < 0) return false;
            output = list[index];
            return true;
        }
        
        public static bool TryFindLast<T>(this IList<T> list, Predicate<T> match, out T output)
        {
            output = default;
            if (list == null || list.Count <= 0 || match == null) return false;
            int index = list.FindLastIndex(match);
            if (index < 0) return false;
            output = list[index];
            return true;
        }
        
        public static bool TryFindMin<T1, T2>(this IList<T1> list, Func<T1, T2> comparer, out T1 output) where T2 : IComparable<T2>
        {
            output = default;
            if (list == null || list.Count <= 0 || comparer == null) return false;
            int index = list.FindMinIndex(comparer);
            if (index < 0) return false;
            output = list[index];
            return true;
        }
        
        public static bool TryFindMax<T1, T2>(this IList<T1> list, Func<T1, T2> comparer, out T1 output) where T2 : IComparable<T2>
        {
            output = default;
            if (list == null || list.Count <= 0 || comparer == null) return false;
            int index = list.FindMaxIndex(comparer);
            if (index < 0) return false;
            output = list[index];
            return true;
        }
        
        public static T Find<T>(this IList<T> list, Predicate<T> match)
        {
            T output;
            list.TryFind(match, out output);
            return output;
        }
        
        public static T FindLast<T>(this IList<T> list, Predicate<T> match)
        {
            T output;
            list.TryFindLast(match, out output);
            return output;
        }
        
        public static T1 FindMin<T1, T2>(this IList<T1> list, Func<T1, T2> comparer) where T2 : IComparable<T2>
        {
            T1 output;
            list.TryFindMin(comparer, out output);
            return output;
        }
        
        public static T1 FindMax<T1, T2>(this IList<T1> list, Func<T1, T2> comparer) where T2 : IComparable<T2>
        {
            T1 output;
            list.TryFindMax(comparer, out output);
            return output;
        }
        
        public static List<T> FindAll<T>(this IList<T> list, Predicate<T> match)
        {
            if (list == null || list.Count <= 0 || match == null) return null;
            bool found = false;
            List<T> output = null; 
            foreach (var data in list)
            {
                if (!match(data)) continue;
                if (!found)
                {
                    found = true;
                    output = new List<T>();
                }
                output.Add(data);
            }
            return output;
        }
        
        public static IEnumerable<T> FindAllNonAlloc<T>(this IList<T> list, Predicate<T> match)
        {
            if (list == null || list.Count <= 0 || match == null) yield break;
            foreach (var data in list)
                if (match(data))
                    yield return data;
        }
        #endregion
        
        public static T Random<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 0) return default;
            return list.Count == 1 ? list[0] : list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T Last<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 0) return default;
            return list[list.Count - 1];
        }
        
        public static IList<T> RemoveLast<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 0) return list;
            list.RemoveAt(list.Count - 1);
            return list;
        }
    }
}