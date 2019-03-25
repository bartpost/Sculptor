using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;
using Sculptor.DataModels;
using Sculptor.ViewModels;

namespace Sculptor
{
    public class TreeListViewDragDropBehavior
    {
        private object originalSource = null;
        private IList sourceCollection = null;

        private RadTreeListView _associatedObject;

        public IList SourceCollection { get; set; } = null;


        /// <summary>
        /// AssociatedObject Property
        /// </summary>
        public RadTreeListView AssociatedObject
        {
            get
            {
                return _associatedObject;
            }
            set
            {
                _associatedObject = value;
            }
        }

        private static Dictionary<RadTreeListView, TreeListViewDragDropBehavior> instances;

        static TreeListViewDragDropBehavior()
        {
            instances = new Dictionary<RadTreeListView, TreeListViewDragDropBehavior>();
        }

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            TreeListViewDragDropBehavior behavior = GetAttachedBehavior(obj as RadTreeListView);

            behavior.AssociatedObject = obj as RadTreeListView;

            if (value)
            {
                behavior.Initialize();
            }
            else
            {
                behavior.CleanUp();
            }
            obj.SetValue(IsEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(TreeListViewDragDropBehavior),
                new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SetIsEnabled(dependencyObject, (bool)e.NewValue);
        }

        public static TreeListViewDragDropBehavior GetAttachedBehavior(RadTreeListView gridview)
        {
            if (!instances.ContainsKey(gridview))
            {
                instances[gridview] = new TreeListViewDragDropBehavior();
                instances[gridview].AssociatedObject = gridview;
            }

            return instances[gridview];
        }

        protected virtual void Initialize()
        {
            DragDropManager.AddDragInitializeHandler(this.AssociatedObject, OnDragInitialize, true);
            DragDropManager.AddDropHandler(this.AssociatedObject, OnDrop, true);
            DragDropManager.AddDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted, true);
            DragDropManager.AddDragOverHandler(this.AssociatedObject, OnDragOver, true);

            this.AssociatedObject.DataLoaded += RadTreeListView1_DataLoaded;
        }

        protected virtual void CleanUp()
        {
            DragDropManager.RemoveDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
            DragDropManager.RemoveDropHandler(this.AssociatedObject, OnDrop);
            DragDropManager.RemoveDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
            DragDropManager.RemoveDragOverHandler(this.AssociatedObject, OnDragOver);

            this.AssociatedObject.DataLoaded -= RadTreeListView1_DataLoaded;
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            var sourceRow = (e.OriginalSource as TreeListViewRow) ?? (e.OriginalSource as FrameworkElement).ParentOfType<TreeListViewRow>();

            if (sourceRow != null)
            {
                var dataObject = DragDropPayloadManager.GeneratePayload(null);

                var draggedItem = sourceRow.Item;

                DragDropPayloadManager.SetData(dataObject, "DragData", new Collection<object>() { draggedItem });
                e.Data = dataObject;

                // TODO: Cleanup. The "if" statement is to patch a problem with the ObjectAssociation collection. Quick and Dirty
                if ((sender as RadTreeListView).Name != "ObjectAssociationTreeListView")
                {
                    var screenshotVisualProvider = new ScreenshotDragVisualProvider();
                    e.DragVisual = screenshotVisualProvider.CreateDragVisual(new DragVisualProviderState(e.RelativeStartPoint, new List<object>() { draggedItem }, new List<DependencyObject>() { sourceRow }, sender as FrameworkElement));
                    e.DragVisualOffset = new Point(0, 0);
                    this.originalSource = sourceRow.Item;
                    this.sourceCollection = sourceRow.ParentRow != null ? (IList)sourceRow.ParentRow.Items.SourceCollection : (IList)sourceRow.GridViewDataControl.ItemsSource;
                }
                // In case the item is dragged to a different tree, populate the DraggedModel object with the dragged data
                // so it can be picked up by the destination tree
                if (Globals.DraggedItem == null) Globals.DraggedItem = new DragModel();
                switch ((sender as RadTreeListView).Name)
                {
                    case "ObjectTreeListView":
                        Globals.DraggedItem.Type = "Object";
                        Globals.DraggedItem.ObjectModelSource = draggedItem as ObjectModel;
                        break;
                    case "ControlObjectTreeListView":
                        Globals.DraggedItem.Type = "ControlObject";
                        Globals.DraggedItem.ControlObjectModelSource = draggedItem as ControlObjectModel;
                        break;
                    case "TemplateTreeListView":
                        Globals.DraggedItem.Type = "Template";
                        Globals.DraggedItem.TemplateModelSource = draggedItem as TemplateModel;
                        break;
                    case "PropertyTreeListView":
                        Globals.DraggedItem.Type = "Property";
                        Globals.DraggedItem.PropertyModelSource = draggedItem as PropertyModel;
                        break;
                    case "HardIOTreeListView":
                        Globals.DraggedItem.Type = "HardIO";
                        Globals.DraggedItem.HardIOModelSource = draggedItem as HardIOModel;
                        break;
                    case "ControlPropertyTreeListView":
                        Globals.DraggedItem.Type = "ControlProperty";
                        Globals.DraggedItem.PropertyModelSource = draggedItem as PropertyModel;
                        break;
                    case "RequirementTreeListView":
                        Globals.DraggedItem.Type = "Requirement";
                        Globals.DraggedItem.RequirementModelSource = draggedItem as RequirementModel;
                        break;
                    case "ObjectAssociationTreeListView":
                        Globals.DraggedItem.Type = "ObjectAssociation";
                        Globals.DraggedItem.ObjectAssociationModelSource = draggedItem as ObjectAssociationModel;
                        break;
                    case "ObjectRequirementTreeListView":
                        Globals.DraggedItem.Type = "ObjectRequirement";
                        Globals.DraggedItem.ObjectRequirementModelSource = draggedItem as ObjectRequirementModel;
                        break;
                    case "AspectListView":
                        Globals.DraggedItem.Type = "Aspect";
                        Globals.DraggedItem.AspectModelSource = draggedItem as AspectModel;
                        break;
                    case "AttributeListView":
                        Globals.DraggedItem.Type = "Attribute";
                        Globals.DraggedItem.AttributeModelSource = draggedItem as AttributeModel;
                        break;
                    default:
                        break;
                };
            }
        }

        private void OnDragDropCompleted(object sender, DragDropCompletedEventArgs e)
        {
        }


        //TODO: fix hard code
        private void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            if (e.Data != null && e.AllowedEffects != DragDropEffects.None)
            {
                var treeListViewName = (sender as RadTreeListView).Name;
                TreeListViewRow destination = e.OriginalSource as TreeListViewRow ?? (e.OriginalSource as FrameworkElement).ParentOfType<TreeListViewRow>();
                switch (treeListViewName)
                {
                    case "ObjectTreeListView":
                        ObjectViewModelLocator.GetObjectVM().MoveSelection(destination);
                        break;
                    case "ControlObjectTreeListView":
                        ControlObjectViewModelLocator.GetControlObjectVM().MoveSelection(destination);
                        break;
                    case "TemplateTreeListView":
                        TemplateViewModelLocator.GetTemplateVM().MoveSelection(destination);
                        break;
                    case "PropertyTreeListView":
                        PropertyViewModelLocator.GetPropertyVM().MoveSelection(destination);
                        break;
                    case "ObjectAssociationTreeListView":
                        ObjectAssociationViewModel objectAssociationViewModel = ObjectAssociationViewModelLocator.GetObjectAssociationVM();
                        objectAssociationViewModel.AssociateWithObject(destination);
                        objectAssociationViewModel.FilteredObjectAssociations.View.Refresh();
                        break;
                    case "ObjectRequirementTreeListView":
                        ObjectRequirementViewModel objectRequirementViewModel = ObjectRequirementViewModelLocator.GetObjectRequirementVM();
                        objectRequirementViewModel.AssociateWithObject(destination);
                        objectRequirementViewModel.FilteredObjectRequirements.View.Refresh();
                        break;
                    case "ControlObjectAssociationTreeListView":
                        ControlObjectAssociationViewModel controlObjectAssociationViewModel = ControlObjectAssociationViewModelLocator.GetControlObjectAssociationVM();
                        controlObjectAssociationViewModel.AssociateWithControlObject(destination);
                        controlObjectAssociationViewModel.FilteredControlObjectAssociations.View.Refresh();
                        break;
                    case "TemplateAssociationTreeListView":
                        TemplateAssociationViewModel templateAssociationViewModel = TemplateAssociationViewModelLocator.GetTemplateAssociationVM();
                        templateAssociationViewModel.AssociateWithTemplate(destination);
                        templateAssociationViewModel.FilteredTemplateAssociations.View.Refresh();
                        break;
                    case "TemplateRequirementTreeListView":
                        TemplateRequirementViewModel templateRequirementViewModel = TemplateRequirementViewModelLocator.GetTemplateRequirementVM();
                        templateRequirementViewModel.AssociateWithTemplate(destination);
                        templateRequirementViewModel.FilteredTemplateRequirements.View.Refresh();
                        break;
                    case "RequirementTreeListView":
                        RequirementViewModelLocator.GetRequirementVM().MoveSelection(destination);
                        break;
                }
     
            }
        }

            private void OnDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            var dropTarget = e.OriginalSource as TreeListViewRow ?? (e.OriginalSource as FrameworkElement).ParentOfType<TreeListViewRow>();
            string destinationTree = (sender as RadTreeListView).Name;
            if (this.IsChildOf(dropTarget, this.originalSource) ||
                (destinationTree == "ObjectTreeListView" && Globals.DraggedItem.Type != "Object") ||
                (destinationTree == "TemplateTreeListView" && Globals.DraggedItem.Type != "Template" && Globals.DraggedItem.Type != "Property") ||
                (destinationTree == "PropertyTreeListView" && Globals.DraggedItem.Type != "Property") ||
                (destinationTree == "RequirementTreeListView" && Globals.DraggedItem.Type != "Requirement") ||
                (destinationTree == "ObjectAssociationTreeListView" && Globals.DraggedItem.Type != "Template" && Globals.DraggedItem.Type != "Property") ||
                (destinationTree == "ObjectRequirementTreeListView" && Globals.DraggedItem.Type != "Requirement") ||
                (destinationTree == "RequirementTreeListView" && Globals.DraggedItem.Type != "Requirement") ||
                Globals.DraggedItem.Type == "ObjectAssociation")
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private bool IsChildOf(TreeListViewRow dropTarget, object originalSource)
        {
            var currentElement = dropTarget;
            while (currentElement != null)
            {
                if (currentElement.Item == originalSource)
                {
                    return true;
                }

                currentElement = currentElement.ParentRow;
            }

            return false;
        }

        void RadTreeListView1_DataLoaded(object sender, EventArgs e)
        {
            this.AssociatedObject.DataLoaded -= new EventHandler<EventArgs>(RadTreeListView1_DataLoaded);
            this.AssociatedObject.ExpandAllHierarchyItems();
        }

    }
}
