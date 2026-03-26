using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vc.Models
{
    public class MedicalRecord
    {


        public int Id { get; set;}
        public int PetId { get; set;}


        public string PetName { get; set;}


        public int? AppointmentId {get; set;}

        public string Diagnosis { get; set;}
        public string Treatment { get; set;}
        public string Prescription { get; set;}

        public DateTime RecordDate { get; set;}
        public int DoctorId { get; set;}
        public string DoctorName { get; set;}
    }
}
