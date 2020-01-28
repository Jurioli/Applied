using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static partial class Properties
    {
        private interface INecessary
        {
            void UpdateCount(int count);
        }
        private abstract class Necessary<TValue> : INecessary
        {
            private int _queueCount;
            public TValue Value { get; private set; }
            public void UpdateCount(int count)
            {
                _queueCount = Math.Max(_queueCount, count);
            }
            protected abstract TValue GetReady(IEnumerable items);
            public IEnumerable Each(IEnumerable items)
            {
                if (_queueCount == 0)
                {
                    this.Value = this.GetReady(new object[0]);
                    return items;
                }
                else
                {
                    return this.EachQueue(items);
                }
            }
            private IEnumerable EachQueue(IEnumerable items)
            {
                bool ready = false;
                Queue queue = new Queue();
                this.Value = default;
                foreach (object item in items)
                {
                    if (ready)
                    {
                        yield return item;
                    }
                    else
                    {
                        queue.Enqueue(item);
                        if (queue.Count >= _queueCount)
                        {
                            this.Value = this.GetReady(queue);
                            ready = true;
                            while (queue.Count > 0)
                            {
                                yield return queue.Dequeue();
                            }
                        }
                    }
                }
                if (!ready && queue.Count > 0)
                {
                    this.Value = this.GetReady(queue);
                    while (queue.Count > 0)
                    {
                        yield return queue.Dequeue();
                    }
                }
            }
        }
        private abstract class Necessary<TSource, TValue> : INecessary
        {
            private int _queueCount;
            public TValue Value { get; private set; }
            public void UpdateCount(int count)
            {
                _queueCount = Math.Max(_queueCount, count);
            }
            protected abstract TValue GetReady(IEnumerable<TSource> items);
            public IEnumerable<TSource> Each(IEnumerable<TSource> items)
            {
                if (_queueCount == 0)
                {
                    this.Value = this.GetReady(Enumerable.Empty<TSource>());
                    return items;
                }
                else
                {
                    return this.EachQueue(items);
                }
            }
            private IEnumerable<TSource> EachQueue(IEnumerable<TSource> items)
            {
                bool ready = false;
                Queue<TSource> queue = new Queue<TSource>();
                this.Value = default;
                foreach (TSource item in items)
                {
                    if (ready)
                    {
                        yield return item;
                    }
                    else
                    {
                        queue.Enqueue(item);
                        if (queue.Count >= _queueCount)
                        {
                            this.Value = this.GetReady(queue);
                            ready = true;
                            while (queue.Count > 0)
                            {
                                yield return queue.Dequeue();
                            }
                        }
                    }
                }
                if (!ready && queue.Count > 0)
                {
                    this.Value = this.GetReady(queue);
                    while (queue.Count > 0)
                    {
                        yield return queue.Dequeue();
                    }
                }
            }
        }
        private abstract class MatchesNecessary<TSource> : Necessary<TSource, MatchProperty[]>
        {
            public abstract void LoadProperties(IEnumerable<TSource> items);
        }
    }
}
