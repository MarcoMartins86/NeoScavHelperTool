using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Services
{
    public class LoadingService
    {
        private readonly ILogger<LoadingService> _logger;
        private readonly IOptionsMonitor<AppOptions> _optionsDelegate;

        public LoadingService(
            ILogger<LoadingService> logger,
            IOptionsMonitor<AppOptions> optionsDelegate
        )
        {
            _logger = logger;
            _optionsDelegate = optionsDelegate;
        }

        public void Start(SplashViewModel splashViewModel)
        {
            splashViewModel.Message = _optionsDelegate.CurrentValue.NeoScavExePath;
            Thread.Sleep(5000);
        }
    }
}
