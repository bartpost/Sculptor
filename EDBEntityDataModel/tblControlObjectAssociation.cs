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
    
    public partial class tblControlObjectAssociation
    {
        public System.Guid ID { get; set; }
        public System.Guid ControlObject_ID { get; set; }
        public Nullable<System.Guid> ControlProperty_ID { get; set; }
        public Nullable<System.Guid> Association_ID { get; set; }
        public string AssociationType { get; set; }
        public int Project_ID { get; set; }
    }
}
