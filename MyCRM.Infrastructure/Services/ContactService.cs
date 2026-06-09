using MyCRM.Application.Interfaces;
using MyCRM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;
using MyCRM.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Infrastructure.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IClientRepository _clientRepository;

        public ContactService(IContactRepository contactRepository, IClientRepository clientRepository)
        {
            _contactRepository = contactRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Contact?> AddContactAsync(string phone, string email, int clientId, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Телефон не может быть пустым");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email не может быть пустым");

            var client = await _clientRepository.GetUserByIdAsync(clientId, token);
            if (client == null)
                throw new ArgumentException($"Клиент с ID {clientId} не найден");

            var contact = new Contact(phone, email, clientId);
            var result = await _contactRepository.AddContactInRepoAsync(contact, token);

            return result ? contact : null;
        }

        public async Task<List<Contact>> GetContactsByClientIdAsync(int clientId, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _contactRepository.GetContactByUserIdAsync(clientId, token);
        }
    }
}
