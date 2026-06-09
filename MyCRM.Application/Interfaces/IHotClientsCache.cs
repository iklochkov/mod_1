using System.Threading;
using MyCRM.Domain.Entities;

namespace MyCRM.Application.Interfaces;

public interface IHotClientsCache
{
    Client? GetClient(int id);
    void AddOrUpdate(Client client);
    void Refresh();
}
