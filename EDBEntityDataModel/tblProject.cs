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
    
    public partial class tblProject
    {
        public int ID { get; set; }
        public string ContractNo { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public Nullable<System.DateTime> LastOpened { get; set; }
        public byte[] Logo { get; set; }
        public string LastOpenedBy { get; set; }
    }
}
