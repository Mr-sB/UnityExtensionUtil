namespace System.Collections.Generic.PriorityQueue
{
    public abstract class PriorityQueueBase<T>
    {
        protected List<T> nodes;
        public int Count => nodes.Count;
        
        public PriorityQueueBase(int capacity = 0)
        {
            nodes = new List<T>(capacity);
        }
        
        public static int GetChildIndex(int parentIndex, bool isLeft)
        {
            int leftChildIndex = (parentIndex << 1) + 1;
            int childIndex = isLeft ? leftChildIndex : leftChildIndex + 1;
            return childIndex;
        }
        
        public static int GetParentIndex(int childIndex)
        {
            return (childIndex - 1) >> 1;
        }
        
        protected abstract int Compare(T a, T b);

        public void Enqueue(T element)
        {
            //插到最后，然后上滤
            nodes.Add(element);
            UpFilter(Count - 1);
        }

        public T Dequeue()
        {
            int count = Count;
            if (count <= 0) return default;
            
            //移除顶部，把最后的节点放到最前，然后下滤
            int lastIndex = count - 1;
            var element = nodes[0];
            nodes[0] = nodes[lastIndex];
            nodes.RemoveAt(lastIndex);
            DownFilter(0);
            return element;
        }

        /// <summary>
        /// 插入节点优先级高于父节点时上滤。使用挖洞式移动父节点，最后一次性放回插入节点。
        /// </summary>
        protected void UpFilter(int childIndex)
        {
            var child = nodes[childIndex];
            while (childIndex > 0)
            {
                int parentIndex = GetParentIndex(childIndex);
                var parent = nodes[parentIndex];
                // 相等优先级不交换，避免同级节点无意义上浮。
                if (!IsHigherPriority(child, parent)) break;
                
                nodes[childIndex] = parent;
                childIndex = parentIndex;
            }
            nodes[childIndex] = child;
        }

        protected bool IsHigherPriority(T a, T b)
        {
            return Compare(a, b) < 0;
        }

        protected void DownFilter(int parentIndex)
        {
            int count = Count;
            while (parentIndex < count - 1)
            {
                int leftIndex = GetChildIndex(parentIndex, true);
                int rightIndex = leftIndex + 1;
                //没有子节点了
                if (leftIndex >= count)
                    break;

                int nextIndex = CompareExchange(parentIndex, leftIndex, rightIndex);
                if (nextIndex < 0) break;
                
                //继续下滤
                parentIndex = nextIndex;
            }
        }

        /// <summary>
        /// 外部修改内容之后，刷新顺序
        /// </summary>
        public void Refresh()
        {
            //自下而上的下滤
            //弗洛伊德建堆
            //时间复杂度O(n)
            int count = Count;
            int parentIndex = GetParentIndex(count - 1);
            while (parentIndex >= 0)
            {
                DownFilter(parentIndex);
                parentIndex--;
            }
        }

        public T Peek()
        {
            return Count > 0 ? nodes[0] : default;
        }

        public void Clear()
        {
            nodes.Clear();
        }
        
        protected int CompareExchange(int parentIndex, int leftIndex, int rightIndex)
        {
            var parent = nodes[parentIndex];
            var left = nodes[leftIndex];
            if (rightIndex < Count)
            {
                //有右子节点
                var right = nodes[rightIndex];
                if (!IsHigherPriority(left, parent) && !IsHigherPriority(right, parent))
                {
                    //parent最小
                    //已经完整了，不需要下滤了
                    return -1;
                }
                    
                //把小的交换到父节点
                if (IsHigherPriority(left, right))
                {
                    //left最小
                    nodes[parentIndex] = left;
                    nodes[leftIndex] = parent;
                    //继续下滤
                    return leftIndex;
                }
                else
                {
                    //right最小
                    nodes[parentIndex] = right;
                    nodes[rightIndex] = parent;
                    //继续下滤
                    return rightIndex;
                }
            }
            else
            {
                //没有右子节点
                if (!IsHigherPriority(left, parent))
                {
                    //parent最小
                    //已经完整了，不需要下滤了
                    return -1;
                }
                    
                //把小的交换到父节点
                //left最小
                nodes[parentIndex] = left;
                nodes[leftIndex] = parent;
                //继续下滤
                return leftIndex;
            }
        }
    }

    public class PriorityQueue<TElement, TPriority> : PriorityQueueBase<(TElement Element, TPriority Priority)>
    {
        private IComparer<TPriority> comparer;
        
        public PriorityQueue(int capacity = 0, IComparer<TPriority> comparer = null) : base(capacity)
        {
            this.comparer = comparer ?? Comparer<TPriority>.Default;
        }
        
        protected override int Compare((TElement Element, TPriority Priority) a, (TElement Element, TPriority Priority) b)
        {
            return comparer.Compare(a.Priority, b.Priority);
        }
    }

    public class PriorityQueue<T> : PriorityQueueBase<T> where T : IComparable<T>
    {
        private bool bigHeap;
        
        public PriorityQueue(int capacity = 0, bool bigHeap = false) : base(capacity)
        {
            this.bigHeap = bigHeap;
        }

        protected override int Compare(T a, T b)
        {
            return bigHeap ? b.CompareTo(a) : a.CompareTo(b);
        }
    }
}
