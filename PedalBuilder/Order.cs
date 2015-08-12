using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedalBuilder
{
    public class Order
    {
        public List<Pedal> Pedals { get; set; }
        public List<Component> Components { get; set; }
        public List<OrderItem> Items { get; set; } 

        public Order()
        {
            this.Items = new List<OrderItem>();
            this.Pedals = new List<Pedal>();
            this.Components = new List<Component>();
        }
    }
}
