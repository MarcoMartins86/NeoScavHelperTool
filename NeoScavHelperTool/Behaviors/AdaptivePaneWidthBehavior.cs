using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;
using NeoScavHelperTool.Models.DataTypeModels.Base;

namespace NeoScavHelperTool.Behaviors
{
    public class AdaptivePaneWidthBehavior : Behavior<HamburgerMenu>
    {
        private ItemsControl _targetItemsControl;
        private ItemContainerGenerator _itemContainerGenerator;
        private Type _lastItemsType = null;

        // Attached Dependency Property to expose the calculated width
        public static readonly DependencyProperty DesiredMaxWidthProperty =
            DependencyProperty.RegisterAttached(
                nameof(DesiredMaxWidth),
                typeof(double),
                typeof(AdaptivePaneWidthBehavior),
                new FrameworkPropertyMetadata(0.0)
            );

        public double DesiredMaxWidth
        {
            get { return (double)GetValue(DesiredMaxWidthProperty); }
            private set { SetValue(DesiredMaxWidthProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            // Subscribe to Loaded to ensure the template is applied and visual tree is built
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            // Unsubscribe from events to prevent memory leaks
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            if (_targetItemsControl != null)
            {
                _targetItemsControl.SizeChanged -= TargetItemsControl_SizeChanged;
                _targetItemsControl = null;
            }
            if (_itemContainerGenerator != null)
            {
                _itemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                _itemContainerGenerator = null;
            }
            // Clear the attached property value on detaching for good measure
            DesiredMaxWidth = 0.0;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe immediately to ensure this runs only once after the control is loaded
            AssociatedObject.Loaded -= AssociatedObject_Loaded;

            // Find the internal ItemsControl within the HamburgerMenu's visual tree
            // The MahApps HamburgerMenu uses an internal HamburgerMenuListBox (which is an ItemsControl)
            _targetItemsControl = FindChildWithCondition<HamburgerMenuListBox>(
                AssociatedObject,
                candidate =>
                {
                    // Check that the candidate has the "ItemsBorder" child visible (the other one will be always empty)
                    var ItemsBorder = FindChildWithCondition<Border>(
                        candidate,
                        border => border.Name == "ItemsBorder"
                    );
                    return ItemsBorder?.Visibility == Visibility.Visible;
                }
            );

            if (_targetItemsControl != null)
            {
                // Subscribe to StatusChanged of the container so that we can recalculate when the items collection changes
                _itemContainerGenerator = _targetItemsControl.ItemContainerGenerator;
                _itemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;

                RecalculatePaneWidth(); // Perform initial calculation
            }
        }

        private void TargetItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Recalculate width on layout changes of the internal ItemsControl
            RecalculatePaneWidth();
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            // This event fires multiple times during the generation process.
            // We only care when all containers have been fully generated and are ready.
            if (_itemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                RecalculatePaneWidth();
            }
        }

        private void RecalculatePaneWidth()
        {
            if (_targetItemsControl == null || _targetItemsControl.Items.IsEmpty)
            {
                DesiredMaxWidth = 0.0;
                return;
            }
            // Check if its still the same collection (since it's not dynamic this suffices, otherwise also check collection size)
            Type currentItemsType = _targetItemsControl.Items.CurrentItem.GetType();
            if (currentItemsType == _lastItemsType)
            {
                // No change in items, skip recalculation
                return;
            }
            _lastItemsType = currentItemsType;

            // Ensure ItemContainerGenerator has generated containers before attempting to measure
            // This handles cases where items are added dynamically or template is not fully applied yet.
            _targetItemsControl.ApplyTemplate();
            if (
                _targetItemsControl.ItemContainerGenerator.Status
                == GeneratorStatus.ContainersGenerated
            )
            {
                // Containers are already generated, perform measurement immediately
                this.DesiredMaxWidth = PerformMeasurementLogic() + 20; // Add padding for scrollbar/borders
            }
        }

        private double PerformMeasurementLogic()
        {
            DataTypeModelBase biggestItem = null;
            foreach (DataTypeModelBase item in _targetItemsControl.Items)
            {
                if (biggestItem == null || item.DisplayName.Length > biggestItem.DisplayName.Length)
                {
                    biggestItem = item;
                }
            }

            if (biggestItem == null)
            {
                return 0;
            }

            // Get the UI element (container) generated for item
            var container =
                _targetItemsControl.ItemContainerGenerator.ContainerFromItem(biggestItem)
                as FrameworkElement;

            if (container == null)
            {
                return 0;
            }

            // This allows 'Measure' to report the *true, unconstrained desired width*
            // instead of the width constrained by the ItemContainerStyle's binding to ListBox.ActualWidth.
            // The binding will be automatically re-applied by WPF's layout system during the Arrange pass.
            container.ClearValue(FrameworkElement.WidthProperty);
            // Force a measure pass on the container with infinite width
            // This ensures the container calculates its natural, unconstrained width.
            container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            // Update maxChildWidth with the maximum desired width found
            return container.DesiredSize.Width;
        }

        /// <summary>
        /// Helper method to find a child control of a specific type in the visual tree.
        /// Performs a depth-first search.
        /// </summary>
        /// <typeparam name="T">The type of the child control to find.</typeparam>
        /// <param name="parent">The parent DependencyObject to start the search from.</param>
        /// <param name="isCorrectChildVerifier">The function that asserts if the current child of the desired type is the one.</param>
        /// <returns>The first found child of the specified type, or null if not found.</returns>
        public static T FindChildWithCondition<T>(
            DependencyObject parent,
            Func<T, bool> isCorrectChildVerifier
        )
            where T : DependencyObject
        {
            if (parent == null)
                return null;

            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child itself is of the desired type, return it
                if (child is T typedChild)
                {
                    if (isCorrectChildVerifier.Invoke(typedChild))
                    {
                        foundChild = typedChild;
                        break;
                    }
                }

                // Otherwise, recursively search its children
                foundChild = FindChildWithCondition<T>(child, isCorrectChildVerifier);
                if (foundChild != null)
                {
                    break;
                }
            }
            return foundChild;
        }
    }
}
