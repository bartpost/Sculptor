//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sculptor.EDBEntityDataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblObjectType
    {
        public int ID { get; set; }
        public int Project_ID { get; set; }
        public string ObjectType { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public Nullable<int> ShowOrder { get; set; }
    }
}
