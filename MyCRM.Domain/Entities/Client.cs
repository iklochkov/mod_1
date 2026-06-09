using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyCRM.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }

        public string? CreatedByUserId { get; set; }
        public virtual IdentityUser? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Deal> Deals { get; set; } = new List<Deal>();

        public string? AvatarPath { get; set; }
        private int _viewCount = 0;
        public int ViewCount => _viewCount;

        public Client()
        {
        }

        public Client(string name, String surname, int age)
        {
            this.Name = name;
            this.Surname = surname;
            this.Age = age;
            CreatedAt = DateTime.UtcNow;
            _viewCount = 0;
        }

        public int IncrementViewCount()
        {
            return Interlocked.Increment(ref _viewCount);
        }
    }

}