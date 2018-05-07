using JobPools.Enumerations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobPools.Workers
{
    public abstract class ThreadedQueueWorker<TJob> : WithEventJobWorker<TJob> where TJob : class
    {       
        
        private readonly ManualResetEvent _stopEvent;
        private readonly TimeSpan _workWait;
        private readonly Task _backgroundTask;
        private bool _stopping;

        public ThreadedQueueWorker(TimeSpan workWait)
        {                       
            _stopEvent = new ManualResetEvent(false);
            _backgroundTask = new Task(DoWork);
            _workWait = workWait;
        }

        public override void Start()
        {
            _backgroundTask.Start();
        }

        public override void Stop()
        {
            _stopping = true;
            _stopEvent.Set();
        }

        /// <summary>
        /// Delegate pool method to do work as a Thread.
        /// For every loop wait configurated time.
        /// Every job do a call to theese methods: Preparation, Process, PostPorcess, optionally HandleException.
        /// Theese methods are implemented in a inherited classes of this, so theese are only abstractions in this level.
        /// </summary>
        private void DoWork()
        {
            do
            {
                _stopEvent.Reset();
                try
                {
                    InnerProcessJob();
                }
                catch (Exception)
                {                    
                    break;
                }
                _stopEvent.WaitOne(_workWait);
            }
            while (!_stopping);

            _stopping = false;
        }

        internal void InnerProcessJob()
        {
            while (this.TryDequeue(out TJob job))
            {
                var finalState = JobStatus.Unknown;
                try
                {
                    UpdateStatus(job, JobStatus.Running);
                    Preparation(job);
                    Process(job);
                    PostProcess(job);
                    finalState = JobStatus.Completed;
                }
                catch (Exception ex)
                {
                    HandleException(job, ex);
                    finalState = JobStatus.Aborted;
                }
                finally
                {
                    UpdateStatus(job, finalState);
                }
            }
        }

        protected abstract void Preparation(TJob job);

        protected abstract void Process(TJob job);

        protected abstract void PostProcess(TJob job);

        protected abstract void UpdateStatus(TJob job, JobStatus status);

        protected abstract void HandleException(TJob job, Exception ex);        
    }
}
