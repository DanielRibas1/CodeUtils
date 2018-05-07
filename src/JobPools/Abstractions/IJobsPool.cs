namespace JobPools.Abstractions
{ 
    public interface IJobsPool<TRequest, TKey, TJob>      
    {
        TJob AddJob(TRequest request);

        bool TryRemove(TKey key, out TJob value);

        bool TryGetValue(TKey key, out TJob value);        
    }
}
