using Unity.Collections;

namespace Pathfinding2D
{
    public struct NativePriorityQueue
    {
        private NativeList<NodeScore> heap;
        public int Count => heap.Length;

        public NativePriorityQueue(int capacity, Allocator alloc)
        {
            heap = new NativeList<NodeScore>(capacity, alloc);
        }

        public void Push(NodeScore item)
        {
            heap.Add(item);
            int i = heap.Length - 1;
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (heap[i].FCost >= heap[p].FCost) break;
                var tmp = heap[i]; heap[i] = heap[p]; heap[p] = tmp;
                i = p;
            }
        }

        public NodeScore Pop()
        {
            var root = heap[0];
            heap[0] = heap[heap.Length - 1];
            heap.RemoveAtSwapBack(heap.Length - 1);
            int i = 0;
            while (true)
            {
                int l = i * 2 + 1, r = i * 2 + 2, small = i;
                if (l < heap.Length && heap[l].FCost < heap[small].FCost) small = l;
                if (r < heap.Length && heap[r].FCost < heap[small].FCost) small = r;
                if (small == i) break;
                var tmp = heap[i]; heap[i] = heap[small]; heap[small] = tmp;
                i = small;
            }
            return root;
        }

        public void UpdatePriority(int index, int newFCost)
        {
            for (int i = 0; i < heap.Length; i++)
            {
                if (heap[i].Index == index)
                {
                    NodeScore node = heap[i];
                    int oldCost = node.FCost;
                    node.FCost = newFCost;
                    heap[i] = node;

                    if (newFCost < oldCost)
                        BubbleUp(i);
                    else
                        BubbleDown(i);

                    return;
                }
            }
        }

        private void BubbleUp(int i)
        {
            while (i > 0)
            {
                int p = (i - 1) / 2;

                if (heap[i].FCost >= heap[p].FCost)
                    break;

                var tmp = heap[i];
                heap[i] = heap[p];
                heap[p] = tmp;

                i = p;
            }
        }

        private void BubbleDown(int i)
        {
            while (true)
            {
                int l = i * 2 + 1;
                int r = i * 2 + 2;
                int small = i;

                if (l < heap.Length && heap[l].FCost < heap[small].FCost)
                    small = l;

                if (r < heap.Length && heap[r].FCost < heap[small].FCost)
                    small = r;

                if (small == i)
                    break;

                var tmp = heap[i];
                heap[i] = heap[small];
                heap[small] = tmp;

                i = small;
            }
        }

        public void Dispose() => heap.Dispose();
    }
}
