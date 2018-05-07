using JobPools.Abstractions;
using System.Collections.Concurrent;

namespace JobPools.Workers
{
    public abstract class WithEventJobWorker<TJob> : ConcurrentQueue<TJob>, IJobWorker<TJob> where TJob : class
    {
        public event StatusChangedJobEventHandler<TJob> StatusChanged;

        public bool IsStarted { get; protected set; }

        public abstract void Start();

        public abstract void Stop();        

        protected virtual void OnStatusChanged(StatusChangedJobEventArgs<TJob> e)
        {
            StatusChanged?.Invoke(e);
        }      
    }
}
