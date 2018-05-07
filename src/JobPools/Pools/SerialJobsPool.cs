using JobPools.Abstractions;
using JobPools.Enumerations;
using JobPools.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JobPools.Pools
{
    public abstract class SerialJobsPool<TKey, TRequest> : ConcurrentDictionary<TKey, IJob<TKey>>, IJobsPool<TRequest, TKey, IJob<TKey>>        
        where TRequest : class
    {
        public SerialJobsPool(IJobWorker<IJob<TKey>> jobWorker)
        {          
            JobWorker = jobWorker;
            JobWorker.StatusChanged += JobWorkerStatusChanged;
        }

        ~SerialJobsPool()
        {
            if (JobWorker != null)
                JobWorker.StatusChanged -= JobWorkerStatusChanged;
        }

        protected IJobWorker<IJob<TKey>> JobWorker { get; }     

        public abstract IJob<TKey> AddJob(TRequest request); 
        
        protected virtual IJob<TKey> InnerAddJob(IJob<TKey> job)
        {
            if (!this.TryAdd(job.ExternalID, job))
                throw new Exception($"Cannot add {nameof(IJob<TKey>)} in {this.GetType().Name} of {nameof(job.ExternalID)} {job.ExternalID}, another {nameof(IJob<TKey>)} Job for same Element is in progress");
            else            
                JobWorker.Enqueue(job);      
            return job;
        }

        protected virtual void Validate(TRequest request)
        {            
        }      

        private void JobWorkerStatusChanged(StatusChangedJobEventArgs<IJob<TKey>> e)
        {            
            switch (e.Job.Status)
            {
                case JobStatus.Completed:
                case JobStatus.Aborted:
                    IJob<TKey> removedJob;
                    this.TryRemove(e.Job.ExternalID, out removedJob);                        
                    break;
            }
        }
    }
}
