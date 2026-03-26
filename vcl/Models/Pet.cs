using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vc.Models
{
    public class Pet
    {

        public int Id { get; set;}
        public string Name { get; set;}

        public string Species { get; set;}
        public string Breed { get; set;}
        public int Age { get; set;}
        public int OwnerId { get; set;}
        public string OwnerName { get; set;}



        public DateTime CreatedAt { get; set;}
        // дата и время создания записи в бд
    }
}
