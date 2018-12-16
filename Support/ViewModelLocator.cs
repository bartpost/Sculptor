using Sculptor.ViewModels;

namespace Sculptor
{
    public static class MainViewModelLocator
    {
        private static MainViewModel mainVM = null;
        public static MainViewModel GetMainVM()
        {
            if (mainVM == null)
                mainVM = new MainViewModel();

            return mainVM;
        }

        public static bool IsLoaded()
        {
            return mainVM != null;
        }
    }

    public static class ObjectViewModelLocator
    {
        private static ObjectViewModel objectVM = null;
        public static ObjectViewModel GetObjectVM()
        {
            if (objectVM == null)
                objectVM = new ObjectViewModel();

            return objectVM;
        }

        public static bool IsLoaded()
        {
            return objectVM != null;
        }
    }

    public static class TemplateViewModelLocator
    {
        private static TemplateViewModel templateVM = null;
        public static TemplateViewModel GetTemplateVM()
        {
            if (templateVM == null)
                templateVM = new TemplateViewModel();

            return templateVM;
        }

        public static bool IsLoaded()
        {
            return templateVM.Templates != null;
        }
    }

    public static class PropertyViewModelLocator
    {
        private static PropertyViewModel propertyVM = null;
        public static PropertyViewModel GetPropertyVM()
        {
            if (propertyVM == null)
                propertyVM = new PropertyViewModel();

            return propertyVM;
        }

        public static bool IsLoaded()
        {
            return propertyVM.Properties != null;
        }
    }

    public static class AspectViewModelLocator
    {
        private static AspectViewModel aspectVM = null;
        public static AspectViewModel GetAspectVM()
        {
            if (aspectVM == null)
                aspectVM = new AspectViewModel();

            return aspectVM;
        }

        public static bool IsLoaded()
        {
            return aspectVM != null;
        }
    }

    public static class AttributeViewModelLocator
    {
        private static AttributeViewModel attributeVM = null;
        public static AttributeViewModel GetAttributeVM()
        {
            if (attributeVM == null)
                attributeVM = new AttributeViewModel();

            return attributeVM;
        }

        public static bool IsLoaded()
        {
            return attributeVM != null;
        }
    }

    public static class ObjectAssociationViewModelLocator
    {
        private static ObjectAssociationViewModel objectAssociationVM = null;
        public static ObjectAssociationViewModel GetObjectAssociationVM()
        {
            if (objectAssociationVM == null)
                objectAssociationVM = new ObjectAssociationViewModel();

            return objectAssociationVM;
        }

        public static bool IsLoaded()
        {
            return objectAssociationVM != null;
        }
    }

    public static class ObjectRequirementViewModelLocator
    {
        private static ObjectRequirementViewModel objectRequirementVM = null;
        public static ObjectRequirementViewModel GetObjectRequirementVM()
        {
            if (objectRequirementVM == null)
                objectRequirementVM = new ObjectRequirementViewModel();

            return objectRequirementVM;
        }

        public static bool IsLoaded()
        {
            return objectRequirementVM != null;
        }
    }

    public static class TemplateAssociationViewModelLocator
    {
        private static TemplateAssociationViewModel templateAssociationVM = null;
        public static TemplateAssociationViewModel GetTemplateAssociationVM()
        {
            if (templateAssociationVM == null)
                templateAssociationVM = new TemplateAssociationViewModel();

            return templateAssociationVM;
        }

        public static bool IsLoaded()
        {
            return templateAssociationVM != null;
        }
    }

    public static class TemplateRequirementViewModelLocator
    {
        private static TemplateRequirementViewModel templateRequirementVM = null;
        public static TemplateRequirementViewModel GetTemplateRequirementVM()
        {
            if (templateRequirementVM == null)
                templateRequirementVM = new TemplateRequirementViewModel();

            return templateRequirementVM;
        }

        public static bool IsLoaded()
        {
            return templateRequirementVM != null;
        }
    }

    public static class RequirementViewModelLocator
    {
        private static RequirementViewModel requirementVM = null;
        public static RequirementViewModel GetRequirementVM()
        {
            if (requirementVM == null)
                requirementVM = new RequirementViewModel();

            return requirementVM;
        }

        public static bool IsLoaded()
        {
            return requirementVM != null;
        }
    }
}
