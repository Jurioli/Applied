using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static partial class Properties
    {
        private interface INecessary
        {
            void UpdateCount(int count);
        }
        private interface IMatchesNecessary<TSource>
        {
            void LoadProperties(IEnumerable<TSource> items);
        }
        private abstract class NecessaryBase<TValue> : INecessary
        {
            protected int Count { get; private set; }
            public TValue Value { get; protected set; }
            protected NecessaryBase(int count)
            {
                this.Count = count;
            }
            public void UpdateCount(int count)
            {
                this.Count = Math.Max(this.Count, count);
            }
        }
        private abstract class Necessary<TSource, TValue> : NecessaryBase<TValue>
        {
            public Necessary() : base(0)
            {

            }
            protected abstract TValue GetReady(IEnumerable<TSource> items);
            public IEnumerable<TSource> Each(IEnumerable<TSource> items)
            {
                if (this.Count == 0)
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
                        if (queue.Count >= this.Count)
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
        private abstract class NecessaryFirst<TValue> : NecessaryBase<TValue>
        {
            public NecessaryFirst() : base(1)
            {

            }
            protected abstract void First(object first);
            protected abstract TValue GetReady(IEnumerable items);
            public IEnumerable Each(IEnumerable items)
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
                        if (queue.Count == 0)
                        {
                            this.First(item);
                        }
                        queue.Enqueue(item);
                        if (queue.Count >= this.Count)
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
        private abstract class NecessaryFirst<TSource, TValue> : NecessaryBase<TValue>
        {
            public NecessaryFirst() : base(1)
            {

            }
            protected abstract void First(TSource first);
            protected abstract TValue GetReady(IEnumerable<TSource> items);
            public IEnumerable<TSource> Each(IEnumerable<TSource> items)
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
                        if (queue.Count == 0)
                        {
                            this.First(item);
                        }
                        queue.Enqueue(item);
                        if (queue.Count >= this.Count)
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
    }
}
