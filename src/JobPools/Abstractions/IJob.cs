using JobPools.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobPools.Abstractions
{
    public interface IJob<TKey>
    {
        Guid ID { get; }
        TKey ExternalID { get; }
        JobStatus Status { get; }
    }
}
