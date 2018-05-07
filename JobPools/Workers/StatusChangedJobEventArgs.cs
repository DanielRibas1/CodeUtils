using System;

namespace JobPools.Workers
{
    public class StatusChangedJobEventArgs<TJob> : EventArgs
    {
        public StatusChangedJobEventArgs(TJob job)
        {
            Job = job;
        }

        public TJob Job { get; set; }       
    }
}
