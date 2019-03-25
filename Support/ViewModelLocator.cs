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

        public static void DisposeObjectVM()
        {
            objectVM.Dispose();
            objectVM = null;
        }

        public static bool IsLoaded()
        {
            return objectVM.IsLoaded;
        }
    }

    public static class ControlObjectViewModelLocator
    {
        private static ControlObjectViewModel controlObjectVM = null;
        public static ControlObjectViewModel GetControlObjectVM()
        {
            if (controlObjectVM == null)
                controlObjectVM = new ControlObjectViewModel();

            return controlObjectVM;
        }

        public static void DisposeObjectVM()
        {
            controlObjectVM.Dispose();
            controlObjectVM = null;
        }

        public static bool IsLoaded()
        {
            return controlObjectVM.IsLoaded;
        }
    }

    public static class TypeViewModelLocator
    {
        private static TypeViewModel typeVM = null;
        public static TypeViewModel GetTypeVM()
        {
            if (typeVM == null)
                typeVM = new TypeViewModel();

            return typeVM;
        }

        public static bool IsLoaded()
        {
            return false;
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
            return templateVM.IsLoaded;
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
            return propertyVM.IsLoaded;
        }
    }

    public static class HardIOViewModelLocator
    {
        private static HardIOViewModel hardIOVM = null;
        public static HardIOViewModel GetHardIOVM()
        {
            if (hardIOVM == null)
                hardIOVM = new HardIOViewModel();

            return hardIOVM;
        }

        public static bool IsLoaded()
        {
            return hardIOVM.IsLoaded;
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
            return aspectVM.IsLoaaded;
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
            return attributeVM.IsLoaded;
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
            return requirementVM.IsLoaded;
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

    public static class ControlObjectAssociationViewModelLocator
    {
        private static ControlObjectAssociationViewModel controlObjectAssociationVM = null;
        public static ControlObjectAssociationViewModel GetControlObjectAssociationVM()
        {
            if (controlObjectAssociationVM == null)
                controlObjectAssociationVM = new ControlObjectAssociationViewModel();

            return controlObjectAssociationVM;
        }

        public static bool IsLoaded()
        {
            return controlObjectAssociationVM != null;
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

}
