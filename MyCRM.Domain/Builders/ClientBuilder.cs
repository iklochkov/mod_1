using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;

namespace MyCRM.Domain.Builders
{
    public class ClientBuilder
    {
        private string _name = string.Empty;
        private string _surname = string.Empty;
        private int _age;
        private Client _client = new Client();

        public ClientBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public ClientBuilder SetCreatedByUserId(string userId)
        {
            _client.CreatedByUserId = userId;
            return this;
        }

        public ClientBuilder SetSurname(string surname)
        {
            _surname = surname;
            return this;
        }

        public ClientBuilder SetAge(int age)
        {
            _age = age;
            return this;
        }

        public Client Build()
        {
            _client.Name = _name;
            _client.Surname = _surname;
            _client.Age = _age;

            return _client;
        }
    }
}