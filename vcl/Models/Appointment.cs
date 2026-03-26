using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vc.Models
{
    public class Appointment
    {

        public int Id { get; set;}
        public int PetId {get; set;}

        public string PetName {get; set;}
        public int OwnerId { get; set; }

        public string OwnerName { get; set; }

        public DateTime AppointmentDate { get; set;}

        public string Reason {get; set;}
        public string Status {get; set;}



        public DateTime CreatedAt {get; set;}
        public string PetSpecies { get; set;}
    }
}
