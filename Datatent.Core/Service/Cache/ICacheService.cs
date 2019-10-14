namespace Datatent.Core.Service.Cache
{
    public interface ICacheService<T> where T: class
    {
        void Cache(string Id, T document);

        T? Retrieve(string Id);

        bool IsKeyInCache(string Id);
    }
}
