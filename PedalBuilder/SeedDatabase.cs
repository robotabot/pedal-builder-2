﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PedalBuilder
{
    public class SeedDatabase
    {
        private static int added = 0;
        private static PedalContext _context = new PedalContext();
        private static Component resistor;
        private static List<Component> resistorList = new List<Component>();

        /// <summary>
        /// A list of common resistor values.
        /// </summary>
        private static string[] resistorValues = new string[]
        {
            "0",
            "1",
            "2.2",
            "4.7",
            "5.1",
            "6.8",
            "9.1",
            "10",
            "15",
            "20",
            "22",
            "27",
            "30",
            "33",
            "39",
            "47",
            "51",
            "56",
            "62",
            "68",
            "82",
            "91",
            "110",
            "120",
            "150",
            "180",
            "200",
            "220",
            "240",
            "270",
            "300",
            "330",
            "360",
            "470",
            "510",
            "560",
            "680",
            "820",
            "910",
            "1K",
            "1.2K",
            "1.5K",
            "1.8K",
            "2K",
            "2.2K",
            "2.4K",
            "2.7K",
            "3K",
            "3.3K",
            "3.6K",
            "3.9K",
            "4.3K",
            "4.7K",
            "5.1K",
            "5.6K",
            "6.2K",
            "6.8K",
            "7.5K",
            "8.2K",
            "9.1K",
            "10K",
            "11K",
            "12K",
            "13K",
            "14K",
            "15K",
            "16K",
            "18K",
            "20K",
            "22K",
            "24K",
            "27K",
            "30K",
            "33K",
            "36K",
            "39K",
            "43K",
            "47K",
            "51K",
            "56K",
            "62K",
            "68K",
            "75K",
            "82K",
            "91K",
            "100K",
            "110K",
            "120K",
            "130K",
            "140K",
            "150K",
            "160K",
            "180K",
            "200K",
            "220K",
            "240K",
            "270K",
            "300K",
            "330K",
            "360K",
            "390K",
            "430K",
            "470K",
            "510K",
            "560K",
            "620K",
            "680K",
            "750K",
            "820K",
            "910K",
            "1M",
            "1.2M",
            "1.5M",
            "1.8M",
            "2M",
            "2.2M",
            "2.4M",
            "2.7M"
        };

        //TODO fix disposal of context
        /// <summary>
        /// Creates a new component for each resistor.
        /// Adds the component to the list of resistors.
        /// Saves the components to the database.
        /// Disposes the context.
        /// </summary>
        /// <returns> int added</returns>
        public static int SeedResistors()
        {
            foreach (string r in resistorValues)
            {
                resistor = new Component();
                resistor.Type = "Resistor";
                resistor.Price = (decimal?)0.00;
                resistor.Value = r;
                resistorList.Add(resistor);
            }
            _context.Components.AddRange(resistorList);
            added = _context.SaveChanges();

            if (added > 0)
            {
                _context.Dispose();
            }

            return added;
        }

    }
}
