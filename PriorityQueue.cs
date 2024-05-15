namespace System.Collections.Generic.PriorityQueue
{
    public class PriorityQueue<TElement, TPriority>
    {
        private List<(TElement Element, TPriority Priority)> nodes;
        private IComparer<TPriority> comparer;
        public int Count => nodes.Count;
        
        public PriorityQueue(int capacity = 0, IComparer<TPriority> comparer = null)
        {
            nodes = new List<(TElement, TPriority)>(capacity);
            this.comparer = comparer ?? Comparer<TPriority>.Default;
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

        public void Enqueue(TElement element, TPriority priority)
        {
            //插到最后，然后上滤
            nodes.Add((element, priority));
            UpFilter(GetParentIndex(Count - 1));
        }

        public TElement Dequeue()
        {
            int count = Count;
            if (count <= 0) return default;
            
            //移除顶部，把最后的节点放到最前，然后下滤
            int lastIndex = count - 1;
            var element = nodes[0].Element;
            nodes[0] = nodes[lastIndex];
            nodes.RemoveAt(lastIndex);
            DownFilter(0);
            return element;
        }

        private void UpFilter(int parentIndex)
        {
            while (parentIndex >= 0)
            {
                int leftIndex = GetChildIndex(parentIndex, true);
                int rightIndex = leftIndex + 1;
                int nextIndex = CompareExchange(parentIndex, leftIndex, rightIndex);
                if (nextIndex < 0) break;
                
                //继续上滤
                parentIndex = GetParentIndex(parentIndex);
            }
        }

        private void DownFilter(int parentIndex)
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

        public TElement Peek()
        {
            return Count > 0 ? nodes[0].Element : default;
        }

        public void Clear()
        {
            nodes.Clear();
        }

        private int CompareExchange(int parentIndex, int leftIndex, int rightIndex)
        {
            var parent = nodes[parentIndex];
            var left = nodes[leftIndex];
            if (rightIndex < Count)
            {
                //有右子节点
                var right = nodes[rightIndex];
                if (comparer.Compare(parent.Priority, left.Priority) <= 0 && comparer.Compare(parent.Priority, right.Priority) <= 0)
                {
                    //parent最小
                    //已经完整了，不需要下滤了
                    return -1;
                }
                    
                //把小的交换到父节点
                if (comparer.Compare(left.Priority, right.Priority) <= 0)
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
                if (comparer.Compare(parent.Priority, left.Priority) <= 0)
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

    public class PriorityQueue<T> where T : IComparable<T>
    {
        public List<T> nodes;
        public int Count => nodes.Count;
        private bool bigHeap;
        
        public PriorityQueue(int capacity = 0, bool bigHeap = false)
        {
            nodes = new List<T>(capacity);
            this.bigHeap = bigHeap;
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

        public void Enqueue(T element)
        {
            //插到最后，然后上滤
            nodes.Add(element);
            UpFilter(GetParentIndex(Count - 1));
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

        private void UpFilter(int parentIndex)
        {
            while (parentIndex >= 0)
            {
                int leftIndex = GetChildIndex(parentIndex, true);
                int rightIndex = leftIndex + 1;
                int nextIndex = CompareExchange(parentIndex, leftIndex, rightIndex);
                if (nextIndex < 0) break;
                
                //继续上滤
                parentIndex = GetParentIndex(parentIndex);
            }
        }

        private void DownFilter(int parentIndex)
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
            int count = Count;
            int parentIndex = GetParentIndex(count - 1);
            while (parentIndex >= 0)
            {
                DownFilter(parentIndex);
                int leftIndex = GetChildIndex(parentIndex, true);
                parentIndex = GetParentIndex(leftIndex - 1);
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
        
        private int CompareExchange(int parentIndex, int leftIndex, int rightIndex)
        {
            var parent = nodes[parentIndex];
            var left = nodes[leftIndex];
            if (rightIndex < Count)
            {
                //有右子节点
                var right = nodes[rightIndex];
                if (Compare(parent, left) && Compare(parent, right))
                {
                    //parent最小
                    //已经完整了，不需要下滤了
                    return -1;
                }
                    
                //把小的交换到父节点
                if (Compare(left, right))
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
                if (Compare(parent, left))
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

        private bool Compare(T a, T b)
        {
            int value = a.CompareTo(b);
            return bigHeap ? value >= 0 : value <= 0;
        }
    }
}
