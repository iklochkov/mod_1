using System;
using System.Collections.Generic;
using System.Linq;
using MyCRM.Domain.Entities;

namespace MyCRM.Application.Interfaces
{
    public interface IContactRepository
    {
        Task<bool> AddContactInRepoAsync(Contact contact, CancellationToken token = default);
        Task<List<Contact>> GetContactByUserIdAsync(int clientId, CancellationToken token = default);
    }
}