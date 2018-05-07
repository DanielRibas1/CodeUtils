using System;
using System.Collections.Generic;
using System.Text;

namespace JobPools.Enumerations
{
    public enum JobStatus
    {
        Unknown,
        Ready,
        Running,
        Paused,
        Completed,
        Aborted        
    }
}
