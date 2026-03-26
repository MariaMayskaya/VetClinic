using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vc.Models
{
    public class Medicine
    {


        public int Id { get; set;}
        public string Name { get; set;}


        public string Description { get; set;}
        public int Quantity { get; set;}
        public decimal Price { get; set;}
        public DateTime ExpiryDate { get; set;}


        public bool IsLowStock => Quantity < 10;

        // вычисляемое свойство. возвращает true, если количество лекарства меньше 10 (низкий запас)
    }
}
