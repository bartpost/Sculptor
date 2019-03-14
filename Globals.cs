using Sculptor.DataModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sculptor
{
    public enum TreeType { Object, Class, Property, Requirement, ObjectAssociation }

    public class DragModel
    {
        public string Type;
        public ObjectModel ObjectModelSource;
        public TemplateModel TemplateModelSource;
        public PropertyModel PropertyModelSource;
        public RequirementModel RequirementModelSource;
        public ObjectAssociationModel ObjectAssociationModelSource;
        public ObjectRequirementModel ObjectRequirementModelSource;
        public AspectModel AspectModelSource;
        public AttributeModel AttributeModelSource;
    }

    static class Globals
    {
        private static string projectName;
        public static string ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                if (value != projectName)
                {
                    projectName = value;
                }
            }
        }

        private static string contractNo;
        public static string ContractNo
        {
            get
            {
                return contractNo;
            }
            set
            {
                if (value != contractNo)
                {
                    contractNo = value;
                }
            }
        }

        public static int Project_ID { get; set; }
        public static bool ProjectSelected { get; set; }
        public static DragModel DraggedItem { get; set; }
    }

}
