using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Services
{
    public class ViewModelLocatorService
    {
        public static SplashScreenViewModel SplashScreenViewModel =>
            Ioc.Default.GetRequiredService<SplashScreenViewModel>();
        public static HamburgerPaneViewModel HamburgerPaneViewModel =>
            Ioc.Default.GetRequiredService<HamburgerPaneViewModel>();
    }
}
