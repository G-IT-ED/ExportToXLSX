using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RetroToFileExporter.Core
{
    class MultyThreadQueue
    {
        private readonly int _countThread;

        public MultyThreadQueue(int countThread)
        {
            _countThread = countThread;
        }

        private ConcurrentDictionary<Thread, int> _listThreadReceiver = new ConcurrentDictionary<Thread, int>();

        public void Enqueue(Thread thread)
        {
            _listThreadReceiver[thread] = 0;
        }
        

        public Thread GetFirst()
        {
            if (_listThreadReceiver.IsEmpty) return null;
            
            var started = _listThreadReceiver.Where(x => x.Key.ThreadState == ThreadState.Running || 
                x.Key.ThreadState==ThreadState.WaitSleepJoin);
            if (started.Count() > _countThread)
                return null;

            var stopped = _listThreadReceiver.Where(x => x.Key.ThreadState == ThreadState.Aborted
                                                      || x.Key.ThreadState == ThreadState.Stopped).ToList();

            foreach (var thread in stopped)
            {
                var t = thread.Key;
                int i;
                _listThreadReceiver.TryRemove(t, out i);
            }

            var result = _listThreadReceiver.Where(x => x.Key.ThreadState == ThreadState.Unstarted);
            if (result.Any())
                return result.First().Key;
            return null;
        }

        public int Count()
        {
            return _listThreadReceiver.Count;
        }

        public ICollection<Thread> GetAll()
        {
            return _listThreadReceiver.Keys;
        }
    }
}