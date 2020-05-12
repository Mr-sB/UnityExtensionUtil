using System;
using System.Collections.Generic;

namespace GameUtil.Extensions
{
    public static class IListExtensions
    {
        #region Sort
        private const int QuickSortDepthThreshold = 32;//快排的深度限制
        
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
            return list.Sort(0, list.Count, comparison);
        }

        public static IList<T> Sort<T>(this IList<T> list, int startIndex, int length, Comparison<T> comparison)
        {
            int endIndex = startIndex + length - 1;
            if (list == null || list.Count <= 1 || comparison == null || startIndex < 0 || endIndex >= list.Count || startIndex >= endIndex) return list;
            list.DepthLimitedQuickSort(startIndex, endIndex, comparison, QuickSortDepthThreshold);
            return list;
        }
        
        //.NET DepthLimitedQuickSort source code
        //深度限制快排
        private static void DepthLimitedQuickSort<T>(this IList<T> keys, int left, int right, Comparison<T> comparer, int depthLimit)
        {
            do
            {
                //超过深度限制就使用堆排序
                if (depthLimit == 0)
                {
                    HeapSort(keys, left, right, comparer);
                    return;
                }
                
                int i = left;
                int j = right;
 
                // pre-sort the low, middle (pivot), and high values in place.
                // this improves performance in the face of already sorted data, or 
                // data that is made up of multiple sorted runs appended together.
                int middle = i + ((j - i) >> 1);
                SwapIfGreater(keys, comparer, i, middle);  // swap the low with the mid point
                SwapIfGreater(keys, comparer, i, j);   // swap the low with the high
                SwapIfGreater(keys, comparer, middle, j); // swap the middle with the high
 
                T x = keys[middle];
                do
                {
                    while (comparer(keys[i], x) < 0) i++;
                    while (comparer(x, keys[j]) < 0) j--;
                    // Contract.Assert(i >= left && j <= right, "(i>=left && j<=right)  Sort failed - Is your IComparer bogus?");
                    if (i > j) break;
                    if (i < j)
                    {
                        T key = keys[i];
                        keys[i] = keys[j];
                        keys[j] = key;
                    }
                    i++;
                    j--;
                } while (i <= j);
 
                // The next iteration of the while loop is to "recursively" sort the larger half of the array and the
                // following calls recrusively sort the smaller half.  So we subtrack one from depthLimit here so
                // both sorts see the new value.
                depthLimit--;
 
                if (j - left <= right - i)
                {
                    if (left < j) DepthLimitedQuickSort(keys, left, j, comparer, depthLimit);
                    left = i;
                }
                else
                {
                    if (i < right) DepthLimitedQuickSort(keys, i, right, comparer, depthLimit);
                    right = j;
                }
            } while (left < right);
        }
         
         private static void SwapIfGreater<T>(IList<T> keys, Comparison<T> comparer, int a, int b)
         {
             if (a == b) return;
             if (comparer(keys[a], keys[b]) <= 0) return;
             T key = keys[a];
             keys[a] = keys[b];
             keys[b] = key;
         }
         
         private static void HeapSort<T>(this IList<T> keys, int lo, int hi, Comparison<T> comparer)
         {
             int n = hi - lo + 1;
             for (int i = n / 2; i >= 1; i = i - 1)
             {
                 DownHeap(keys, i, n, lo, comparer);
             }
             for (int i = n; i > 1; i = i - 1)
             {
                 Swap(keys, lo, lo + i - 1);
                 DownHeap(keys, 1, i - 1, lo, comparer);
             }
         }

         private static void DownHeap<T>(this IList<T> keys, int i, int n, int lo, Comparison<T> comparer)
         {
             T d = keys[lo + i - 1];
             int child;
             while (i <= n / 2)
             {
                 child = 2 * i;
                 if (child < n && comparer(keys[lo + child - 1], keys[lo + child]) < 0)
                 {
                     child++;
                 }
                 if (!(comparer(d, keys[lo + child - 1]) < 0))
                     break;
                 keys[lo + i - 1] = keys[lo + child - 1];
                 i = child;
             }
             keys[lo + i - 1] = d;
         }
         
         private static void Swap<T>(IList<T> a, int i, int j)
         {
             if (i == j) return;
             T t = a[i];
             a[i] = a[j];
             a[j] = t;
         }

        // private static void SortInternal<T>(this IList<T> list, int startIndex, int endIndex, Comparison<T> comparison)
        // {
        //     //无需错误检查，因为在调用SortInternal之前已经经过检查了，避免递归时重复检查消耗性能
        //     while (true)
        //     {
        //         //结束条件
        //         if (startIndex >= endIndex) return;
        //         int leftIndex = startIndex;
        //         int rightIndex = endIndex;
        //         //取最左边的为基准数
        //         T tmp = list[startIndex];
        //         while (leftIndex < rightIndex)
        //         {
        //             //先从右开始遍历比较
        //             while (comparison(list[rightIndex], tmp) >= 0 && leftIndex < rightIndex) rightIndex--;
        //             //再从左开始遍历比较
        //             while (comparison(list[leftIndex], tmp) <= 0 && leftIndex < rightIndex) leftIndex++;
        //             //交换
        //             if (leftIndex < rightIndex)
        //             {
        //                 var t = list[leftIndex];
        //                 list[leftIndex] = list[rightIndex];
        //                 list[rightIndex] = t;
        //             }
        //         }
        //
        //         //基准数归位
        //         list[startIndex] = list[leftIndex];
        //         list[leftIndex] = tmp;
        //         //递归左
        //         list.SortInternal(startIndex, leftIndex - 1, comparison);
        //         //递归右
        //         // list.SortInternal(leftIndex + 1, endIndex, comparison);
        //         // 手动将尾递归优化掉
        //         startIndex = leftIndex + 1;
        //     }
        // }
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