using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CodeOwls.PowerShell.Host.Utility
{
    public class SyncQueue<T>
    {
        private readonly Queue<T> _q;
        private readonly ManualResetEvent _qv;

        public SyncQueue()
        {
            _q = new Queue<T>();
            _qv = new ManualResetEvent(false);
        }

        public WaitHandle WaitHandle
        {
            get { return _qv; }
        }

        public void Enqueue(T item)
        {
            lock (_q)
            {
                _q.Enqueue(item);
            }
            _qv.Set();
        }

        public T Dequeue()
        {
            _qv.WaitOne();
            T t = default(T);

            lock (_q)
            {
                t = _q.Dequeue();

                if (0 == _q.Count)
                {
                    _qv.Reset();
                }
            }
            return t;
        }

        public List<T> DequeueAny()
        {
            lock (_q)
            {
                var l = _q.ToList();
                _q.Clear();
                return l;
            }
        }
    }
}