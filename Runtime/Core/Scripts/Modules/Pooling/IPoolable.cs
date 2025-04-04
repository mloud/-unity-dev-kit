namespace OneDay.Core.Modules.Pooling
{
    public interface IPoolable
    {
        string Key { get; set; }
        void OnGetFromPool();
        void OnReturnToPool();
    }
}