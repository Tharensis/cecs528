using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Priority
{
    public class PriorityQueue<T> where T:IComparable<T>
    {
        public List<T> _internal;

        public PriorityQueue()
        {
            _internal = new List<T>();
        }

        public void Enqueue(T item)
        {
            _internal.Add(item);
            int ci = _internal.Count - 1;
            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (_internal[ci].CompareTo(_internal[pi]) > 0)
                    break;
                T tmp = _internal[ci];
                _internal[ci] = _internal[pi];
                _internal[pi] = tmp;
                ci = pi;
            }
        }

        public T Dequeue()
        {
            // Assumes pq isn't empty
            int lastNode = _internal.Count - 1;
            T frontItem = _internal.FirstOrDefault();
            if (frontItem == null) return default(T);
            _internal[0] = _internal[lastNode];
            _internal.RemoveAt(lastNode);

            --lastNode;
            int parentNode = 0;
            while (true)
            {
                int childNode = parentNode * 2 + 1;
                if (childNode > lastNode) break;
                int rc = childNode + 1;
                if (rc <= lastNode && _internal[rc].CompareTo(_internal[childNode]) >= 0)
                    childNode = rc;
                if (_internal[parentNode].CompareTo(_internal[childNode]) < 0) break;
                T tmp = _internal[parentNode]; _internal[parentNode] = _internal[childNode]; _internal[childNode] = tmp;
                parentNode = childNode;
            }
            return frontItem;
        }

        public T Peek()
        {
            return _internal.FirstOrDefault();
        }

        public int Count
        {
            get { return _internal.Count; }
        }

        public bool Contains(T item)
        {
            return _internal.Contains(item);
        }
    }
}
