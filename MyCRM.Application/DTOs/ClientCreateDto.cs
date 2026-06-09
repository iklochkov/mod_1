using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Application.DTOs
{
    public class ClientCreateDto
    {

        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set;}

        public ClientCreateDto(string name, string surname, int age)
        {
            Name = name;
            Surname = surname;
            Age = age;
        }
    } 

}
   

