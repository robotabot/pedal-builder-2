//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PedalBuilder
{
    using System;
    using System.Collections.Generic;
    
    public partial class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Pedal_PedalId { get; set; }
        public int Component_ComponentId { get; set; }
    
        public virtual Component Component { get; set; }
        public virtual Pedal Pedal { get; set; }
    }
}
