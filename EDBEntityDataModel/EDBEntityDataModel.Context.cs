﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EDBEntities : DbContext
    {
        public EDBEntities()
            : base("name=EDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tblAspect> tblAspects { get; set; }
        public virtual DbSet<tblAttribute> tblAttributes { get; set; }
        public virtual DbSet<tblNativeDataType> tblNativeDataTypes { get; set; }
        public virtual DbSet<tblObjectAssociation> tblObjectAssociations { get; set; }
        public virtual DbSet<tblObjectRequirement> tblObjectRequirements { get; set; }
        public virtual DbSet<tblObject> tblObjects { get; set; }
        public virtual DbSet<tblObjectType> tblObjectTypes { get; set; }
        public virtual DbSet<tblProject> tblProjects { get; set; }
        public virtual DbSet<tblProperty> tblProperties { get; set; }
        public virtual DbSet<tblRequirement> tblRequirements { get; set; }
        public virtual DbSet<tblTemplateRequirement> tblTemplateRequirements { get; set; }
        public virtual DbSet<tblTemplate> tblTemplates { get; set; }
        public virtual DbSet<tblType> tblTypes { get; set; }
        public virtual DbSet<tblTemplateAssociation> tblTemplateAssociations { get; set; }
    }
}
