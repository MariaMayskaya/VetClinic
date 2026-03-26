using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vc.Models
{
    public class AvailableTimeSlot
    {
        public DateTime DateTime {get; set;}
        public bool IsAvailable {get; set;}



        // доп свойства для отображения
        public string DisplayTime => DateTime.ToString("HH:mm");
        public string DisplayDate => DateTime.ToString("dd.MM.yyyy");
        public string DisplayStatus => IsAvailable ? "Доступно" : "Занято";
    }
}
