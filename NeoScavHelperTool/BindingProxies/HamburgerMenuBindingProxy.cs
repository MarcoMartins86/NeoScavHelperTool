using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeoScavHelperTool.BindingProxies
{
    public class HamburgerMenuBindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuBindingProxy();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(DependencyObject),
            typeof(HamburgerMenuBindingProxy),
            new PropertyMetadata(null)
        );

        public DependencyObject Data
        {
            get { return (DependencyObject)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Slot for FirstExpanderDesiredWidth
        public static readonly DependencyProperty FirstExpanderDesiredWidthProperty =
            DependencyProperty.RegisterAttached(
                nameof(FirstExpanderDesiredWidth),
                typeof(double),
                typeof(HamburgerMenuBindingProxy),
                new PropertyMetadata(0.0)
            );
        public double FirstExpanderDesiredWidth
        {
            get { return (double)GetValue(FirstExpanderDesiredWidthProperty); }
            set { SetValue(FirstExpanderDesiredWidthProperty, value); }
        }

        // Slot for SecondExpanderDesiredWidth
        public static readonly DependencyProperty SecondExpanderDesiredWidthProperty =
            DependencyProperty.RegisterAttached(
                nameof(SecondExpanderDesiredWidth),
                typeof(double),
                typeof(HamburgerMenuBindingProxy),
                new PropertyMetadata(0.0)
            );

        public double SecondExpanderDesiredWidth
        {
            get { return (double)GetValue(SecondExpanderDesiredWidthProperty); }
            set { SetValue(SecondExpanderDesiredWidthProperty, value); }
        }
    }
}
