namespace JobPools.Workers
{
    public delegate void StatusChangedJobEventHandler<TJob>(StatusChangedJobEventArgs<TJob> e);        
}
