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

        public Order()
        {
            this.Pedals = new List<Pedal>();
            this.Components = new List<Component>();
        }
    }
}
