using MyCRM.Application.Interfaces;
using MyCRM.Domain.Entities;
using System.Collections.Concurrent;

namespace MyCRM.Infrastructure.Cache;

public class HotClientsCacheImpl : IHotClientsCache
{
    private readonly ConcurrentDictionary<int, Client> _cache = new();
    
    public Client? GetClient(int id)
    {
        _cache.TryGetValue(id, out var client);
        return client;
    }
    
    public void AddOrUpdate(Client client)
    {
        if (client != null)
        {
            _cache[client.Id] = client;
        }
    }
    
    public void Refresh()
    {
        _cache.Clear();
    }
}
