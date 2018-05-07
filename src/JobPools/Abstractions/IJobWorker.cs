using JobPools.Workers;

namespace JobPools.Abstractions
{
    public interface IJobWorker<TJob>      
    {
        event StatusChangedJobEventHandler<TJob> StatusChanged;

        bool IsStarted { get; }

        void Enqueue(TJob item);

        void Start();

        void Stop();        
    }
}
