using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace NeoScavHelperTool.Behaviors
{
    public class ExpanderDesiredWidthBehavior : Behavior<Expander>
    {
        // The property to bind OneWayToSource
        public static readonly DependencyProperty DesiredWidthProperty =
            DependencyProperty.Register(
                nameof(DesiredWidth),
                typeof(double),
                typeof(ExpanderDesiredWidthBehavior),
                new PropertyMetadata(0.0)
            );

        public double DesiredWidth
        {
            get { return (double)GetValue(DesiredWidthProperty); }
            set { SetValue(DesiredWidthProperty, value); }
        }

        // Helper to track the content object so we can unsubscribe later
        private FrameworkElement _trackedContent;

        protected override void OnAttached()
        {
            base.OnAttached();

            // 1. Listen to the Expander itself resizing (covers Header changes)
            this.AssociatedObject.SizeChanged += OnExpanderSizeChanged;

            // 2. Listen for when the "Content" property is assigned a new object
            var dpd = DependencyPropertyDescriptor.FromProperty(
                ContentControl.ContentProperty,
                typeof(Expander)
            );
            if (dpd != null)
            {
                dpd.AddValueChanged(this.AssociatedObject, OnContentPropertyReplaced);
            }

            // 3. Handle the initial content if it exists
            HandleNewContent(this.AssociatedObject.Content);
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SizeChanged -= OnExpanderSizeChanged;

            var dpd = DependencyPropertyDescriptor.FromProperty(
                ContentControl.ContentProperty,
                typeof(Expander)
            );
            if (dpd != null)
            {
                dpd.RemoveValueChanged(this.AssociatedObject, OnContentPropertyReplaced);
            }

            CleanupOldContent();
            base.OnDetaching();
        }

        // Triggered when the Expander's Header or container resizes
        private void OnExpanderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateWidth();
        }

        // Triggered when "Expander.Content = new ListBox()" happens
        private void OnContentPropertyReplaced(object sender, EventArgs e)
        {
            HandleNewContent(this.AssociatedObject.Content);
        }

        // Setup listeners on the INNER content (e.g., the ListBox inside)
        private void HandleNewContent(object newContent)
        {
            CleanupOldContent();

            if (newContent is FrameworkElement newElement)
            {
                _trackedContent = newElement;
                // Listen to the content changing its OWN size (e.g. items added to list)
                _trackedContent.SizeChanged += OnContentSizeChanged;
            }

            // Force a calculation immediately
            CalculateWidth();
        }

        private void CleanupOldContent()
        {
            if (_trackedContent != null)
            {
                _trackedContent.SizeChanged -= OnContentSizeChanged;
                _trackedContent = null;
            }
        }

        private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateWidth();
        }

        private void CalculateWidth()
        {
            if (this.AssociatedObject == null)
                return;

            double actualWidth = this.AssociatedObject.ActualWidth;
            double contentDesiredWidth = 0;

            // 1. Inspect the content specifically
            if (this.AssociatedObject.Content is UIElement contentElement)
            {
                // 2. Ask the content how big it WANTS to be (unconstrained)
                // This works even if the Expander is collapsed or constrained!
                // However it only measures the content itself, so we need to add padding and header later.
                contentElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                contentDesiredWidth = contentElement.DesiredSize.Width;
            }

            // 3. Add a buffer for Expander borders/padding (standard Expander has slight padding)
            // Margin + Padding + Header
            double chromePadding = 40.0;

            // 4. The "Desired" width is the Max of the Header (current) and the Content (potential)
            this.DesiredWidth = Math.Max(actualWidth, contentDesiredWidth + chromePadding);
        }
    }
}
