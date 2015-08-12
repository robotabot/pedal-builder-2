using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedalBuilder
{
    public class OrderItem
    {
        public Boolean Ordered { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public string Url { get; set; }
        public decimal? Price { get; set; }
    }
}
