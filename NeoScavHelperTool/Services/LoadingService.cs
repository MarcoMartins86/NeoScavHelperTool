using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Services
{
    public class LoadingService
    {
        private readonly ILogger<LoadingService> _logger;

        public LoadingService(ILogger<LoadingService> logger)
        {
            _logger = logger;
        }

        public void Start(SplashViewModel splashViewModel)
        {
            splashViewModel.Message = "Vamos cá testar";
            Thread.Sleep(5000);
        }
    }
}
