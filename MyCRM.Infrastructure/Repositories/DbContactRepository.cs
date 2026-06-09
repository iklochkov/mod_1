using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MyCRM.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;
using System.Collections.Concurrent;
using Task = System.Threading.Tasks.Task;

namespace MyCRM.Infrastructure.Repositories
{
    public class DbContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public DbContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> AddContactInRepoAsync(Contact contact, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (contact == null)
                return false;

            await _context.Contacts.AddAsync(contact, token);
            await _context.SaveChangesAsync(token);

            return true;
        }


        public async Task<List<Contact>> GetContactByUserIdAsync(int clientId, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var contacts = await _context.Contacts
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .ToListAsync(token);

            return contacts;
        }

        public async Task<bool> UpdateClientAsync(Client client, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (client == null)
                return false;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<bool> DeleteClientAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var client = await _context.Clients.FindAsync(id, token);
            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync(token);

            return true;
        }
    }
}